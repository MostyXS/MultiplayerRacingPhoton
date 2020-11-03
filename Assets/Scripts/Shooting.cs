using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviourPun
{ 
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] Transform firePosition = null;
    [SerializeField] DeathRacePlayer playerProps;
    [SerializeField] LineRenderer lineRenderer;

    Camera playerCamera;

    float fireRate;
    float fireTimer = Mathf.Infinity;
    
    private bool isLaser = false;
    

    private void Awake()
    {
        fireRate = playerProps.fireRate;
        playerCamera = GetComponent<PlayerSetup>().GetCamera();
        isLaser = playerProps.weaponName == "Laser Gun";
        


    }
    void Update()
    {
        if (!photonView.IsMine) return;
        if (Input.GetKey(KeyCode.Space)&& fireTimer>fireRate)
        {
            photonView.RPC("Fire", RpcTarget.All, firePosition.position);
            fireTimer = 0;
        }
        fireTimer += Time.deltaTime;
    } 
    [PunRPC]
    public void Fire(Vector3 firePosition)
    {

        if (isLaser)
        {
            RaycastHit hit;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
            if(Physics.Raycast(ray,out hit, 200f))
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                lineRenderer.startWidth = .3f;
                lineRenderer.endWidth = .1f;

                lineRenderer.SetPosition(0, firePosition);
                lineRenderer.SetPosition(1, hit.point );
                
                StopAllCoroutines();
                StartCoroutine(DisableLaserAfterSec(2f));

                var target = hit.collider;
                if(target.CompareTag("Player") && target.GetComponent<PhotonView>().IsMine)
                {
                    target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, playerProps.damage);
                }
            }
        }
        else
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f));
            GameObject bulletGameObject = Instantiate(bulletPrefab, firePosition, Quaternion.identity);
            bulletGameObject.GetComponent<BulletScript>().InitializeBullet(ray.direction, playerProps.bulletSpeed, playerProps.damage);

        }
    }
    IEnumerator DisableLaserAfterSec(float time)
    {
        yield return new WaitForSeconds(time);
        lineRenderer.enabled = false;
    }
}
