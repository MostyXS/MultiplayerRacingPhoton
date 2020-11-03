using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float damage;

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;

        if(target.CompareTag("Player") && target.GetComponent<PhotonView>().IsMine)
        {
            target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        }
        Destroy(gameObject);



    }

    public void InitializeBullet(Vector3 direction,float speed, float damage)
    {
        this.damage = damage;
        transform.forward = direction;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }
}
