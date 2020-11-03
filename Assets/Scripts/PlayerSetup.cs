using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    [SerializeField] Camera playerCamera = null;
    [SerializeField] TextMeshProUGUI playerNameText = null;

    CarMovement mover = null;

    private void Awake()
    {
        mover = GetComponent<CarMovement>();
    }

    void Start()
    {
        ExitGames.Client.Photon.Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        bool isMePlayer = photonView.IsMine;

        if (roomProps.ContainsValue("rc"))
        {
            GetComponent<LapController>().enabled = isMePlayer;
        }
        else if(roomProps.ContainsValue("dr"))
        {
            mover.ControlsEnabled = true;  
        }

        GetComponent<CarMovement>().enabled = isMePlayer;
        playerCamera.enabled = isMePlayer;
        SetPlayerUI();
    }

    private void SetComponents()
    {
        
    }

    public Camera GetCamera()
    {
        return playerCamera;
    }

    public void SetPlayerUI()
    {
        if (playerNameText == null) return;
        playerNameText.text = photonView.Owner.NickName;
        playerNameText.gameObject.SetActive(!photonView.IsMine);
    }


}
