using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] GameObject[] selectablePlayers = null;
    
     int playerSelectionNumber = 0;

    private void Start()
    {
        playerSelectionNumber = 0;
        ActivatePlayer(playerSelectionNumber);
    }

    private void ActivatePlayer(int x)
    {
        if (x > selectablePlayers.Length-1)
        {
            x = 0;
        }
        else if(x < 0)
        {
            x = selectablePlayers.Length - 1;
        }

        for (int i = 0;i < selectablePlayers.Length; i++)
        {

            selectablePlayers[i].SetActive(i == x);
        }
        playerSelectionNumber = x;
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { {CustomPropsKeeper.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }
    public void NextPlayer()
    {
        ActivatePlayer(playerSelectionNumber + 1);
    }

    public void PreviousPlayer()
    {
        ActivatePlayer(playerSelectionNumber - 1);

    }
}
