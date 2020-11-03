using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;

public class DeathRaceModeManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] playerPrefabs;

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;

        object playerSelectionNumber;
        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropsKeeper.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        {
            int randomPoint = Random.Range(-30, 30);
            Vector3 randomPosition = new Vector3(randomPoint, 0,randomPoint);

            PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, randomPosition, Quaternion.identity);
        }
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    public void OnQuitMatchButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

}
