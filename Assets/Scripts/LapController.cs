using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapController : MonoBehaviourPun
{
    List<GameObject> lapTriggers = new List<GameObject>();

    int finishOrder = 0;

    public enum RaiseEventCode
    {
        WhoFinishedEventCode
    }
        
    void Start()
    {
        foreach (GameObject lapTrigger in RacingModeManager.Instance.lapTriggers)
        {
            lapTriggers.Add(lapTrigger);
        } 
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            
            string finisherNickname = (string)data[0];

            finishOrder = (int)data[1];

            int viewID = (int)data[2];

            Debug.Log(finisherNickname + " " + finishOrder);

            GameObject orderUITextGameObject = RacingModeManager.Instance.FinishOrderUIGameObjects[finishOrder - 1];
            orderUITextGameObject.SetActive(true);
            Text finishOrderText = orderUITextGameObject.GetComponent<Text>();
            if (viewID == photonView.ViewID)
            {
                finishOrderText.text = finishOrder + ". " + finisherNickname + "(YOU)";
                finishOrderText.color = Color.red;
            }
            else
            {
                finishOrderText.text = finishOrder + ". " + finisherNickname;
            }


        }



    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;

    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject trigger = other.gameObject;
        if (!lapTriggers.Contains(trigger)) return;
        int indexOfTrigger = lapTriggers.IndexOf(trigger);
        lapTriggers[indexOfTrigger].SetActive(false);

        if(other.name == "FinishTrigger")
        {
            //game is finished
            GameFinished();
        }

    }

    private void GameFinished()
    {
        GetComponent<PlayerSetup>().GetCamera().transform.SetParent(null);
        GetComponent<CarMovement>().enabled = false;

        finishOrder++;

        int viewID = photonView.ViewID;

        string nickName = photonView.Owner.NickName;

        object[] data = new object[] {nickName,finishOrder,viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
