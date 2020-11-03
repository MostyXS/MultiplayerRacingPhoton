using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacingModeManager : MonoBehaviour
{
    [SerializeField] GameObject[] playerPrefabs = null;
    [SerializeField] Transform[] instantiatePositions = null;


    public GameObject[] FinishOrderUIGameObjects = null;
    public Text TimeUIText;

    public List<GameObject> lapTriggers = new List<GameObject>();


    public static RacingModeManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance !=this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }



    void Start()
    {
        JoinRoom();
        foreach(GameObject go in FinishOrderUIGameObjects)
        {
            go.SetActive(false);
        }

    }

    private void JoinRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropsKeeper.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                Vector3 instantiatePosition = instantiatePositions[actorNumber - 1].position;

                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }
}
