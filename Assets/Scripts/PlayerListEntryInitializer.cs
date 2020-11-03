using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntryInitializer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Text playerName = null;
    [SerializeField] Button playerReadyButton = null;
    [SerializeField] Image playerReadyImage = null;

    bool isPlayerReady = false;

    public void Initialize(int playerID, string playerName)
    {
        this.playerName.text = playerName;

        if(PhotonNetwork.LocalPlayer.ActorNumber !=playerID)
        {
            playerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable() { { CustomPropsKeeper.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            playerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);
                ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable() { { CustomPropsKeeper.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);
            });
        }
    }
    public void SetPlayerReady(bool value)
    {
        playerReadyImage.enabled = value;
        if (isPlayerReady)
            playerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
        else
            playerReadyButton.GetComponentInChildren<Text>().text = "Ready?";

    }

}
