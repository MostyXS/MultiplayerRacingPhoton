using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviourPun
{
    [SerializeField] float startHealth = 100;
    float currentHealth;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject[] playerBodyObjects = null;

    [SerializeField] GameObject deathPanelUIPrefab;

    GameObject deathPanelUIGameObject;

    Rigidbody rb;
    CarMovement mover;
    Shooting shooter;
    Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        GameObject canvasGO = GameObject.Find("Canvas");
        deathPanelUIGameObject = Instantiate(deathPanelUIPrefab, canvasGO.transform);
        deathPanelUIGameObject.SetActive(false);
        currentHealth = startHealth;
        healthBar.fillAmount = currentHealth / startHealth;
        rb = GetComponent<Rigidbody>();
        mover = GetComponent<CarMovement>();
        shooter = GetComponent<Shooting>();

      
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / startHealth;
        if(currentHealth<=0)
        {
            Die();
        }
    }

    private void Die()
    {
        collider.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        foreach(GameObject go in playerBodyObjects)
        {
            go.SetActive(false);
        }
        if(photonView.IsMine)
        {
            StartCoroutine(Respawn());
        }
    }

    [PunRPC]
    public void Reborn()
    {
        currentHealth = startHealth;
        healthBar.fillAmount = currentHealth / startHealth;
        foreach(GameObject go in  playerBodyObjects)
        {
            go.SetActive(true);
        }
        collider.enabled = true;

    }

    private IEnumerator Respawn()
    {
        mover.enabled = false;
        shooter.enabled = false;
        deathPanelUIGameObject.SetActive(true);
        Text respawnText = deathPanelUIGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();
        float respawnTime = 8f;
        while(respawnTime >0f)
        {
            respawnTime -= Time.deltaTime;
            respawnText.text = ((int)respawnTime).ToString();
            yield return null;
        }
        deathPanelUIGameObject.SetActive(false);
        mover.enabled = true;
        shooter.enabled = true;
        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        photonView.RPC("Reborn", RpcTarget.AllBuffered);


    }
}
