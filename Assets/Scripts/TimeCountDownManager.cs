using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountDownManager : MonoBehaviourPun
{
    Text timeUIText;
    private float timeToStartRace = 5f;

    private void Start()
    {
        timeUIText = RacingModeManager.Instance.TimeUIText;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if(timeToStartRace>=0)
        {
            timeToStartRace -= Time.deltaTime;
            photonView.RPC("UpdateTimer", RpcTarget.AllBuffered, timeToStartRace);
        }
        else
        {
            photonView.RPC("StartRace", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void StartRace()
    {
        GetComponent<CarMovement>().ControlsEnabled = true;
        enabled = false;

    }

    [PunRPC]
    private void UpdateTimer(float time)
    {
        if (time >= 0)
        {
            timeUIText.text = time.ToString();
        }
        else
        {
            timeUIText.text = "";
        }
    }
}
