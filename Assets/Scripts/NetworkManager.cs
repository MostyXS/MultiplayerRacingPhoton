using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    [SerializeField] GameObject loginPanel = null;

    [Header("Connection status UI")]
    [SerializeField] GameObject connectingPanel = null;

    [Header("Game Options UI")]
    [SerializeField] GameObject gameOptionsPanel = null;

    [Header("Create room UI")]
    [SerializeField] GameObject createRoomPanel = null;
    [SerializeField] InputField roomNameInputField = null;
    string gameMode = null;

    [Header("Creating status UI")]
    [SerializeField] GameObject creatingRoomInfoPanel = null;


    [Header("Inside room UI")]
    [SerializeField] GameObject insideRoomPanel = null;
    [SerializeField] Text roomInfoText = null;
    [SerializeField] GameObject playerListPrefab = null;
    [SerializeField] Transform playerListContent = null;
    [SerializeField] GameObject startGameButton = null;
    [SerializeField] Text gameModeText = null;
    [SerializeField] Image panelBackground = null;
    [SerializeField] Sprite racingBackground = null;
    [SerializeField] Sprite deathRaceBackground = null;
    [SerializeField] GameObject[] playerSelectionUIGameObjects = null;
    [SerializeField] DeathRacePlayer[] deathRacePlayers = null;
    [SerializeField] RacingPlayer[] racingPlayers = null;

    [Header("Join random room UI")]
    [SerializeField] GameObject joinRandomRoomPanel = null;

    Dictionary<int, GameObject> playerListGameObjects = null;


    #region Unity Methods
    void Start()
    {
        ActivatePanel(loginPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region UI Callbacks

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnCreateGameButtonClicked()
    {
        ActivatePanel(createRoomPanel.name);
    }
    public void OnLoginButtonClicked()
    {
        if (PhotonNetwork.IsConnected) return;

        ActivatePanel(connectingPanel.name);
        PhotonNetwork.ConnectUsingSettings();
    }
    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsPanel.name);
    }
    public void OnCreateRoomButtonClicked()
    {
        if (gameMode == null)
        {
            Debug.Log("Game mode is null");
            return;
        }
        CreateRoom();
    }
    public void OnBackButtonClicked()
    {
        ActivatePanel(gameOptionsPanel.name);
    }
    public void OnJoinGameButtonClicked()
    {
        ActivatePanel(joinRandomRoomPanel.name);
    }
    public void OnJoinRandomRoomButtonClicked(string gameMode)
    {
        this.gameMode = gameMode;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);

    }
    public void OnStartGameButtonClicked()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                //Racing mode
                PhotonNetwork.LoadLevel("RacingScene");
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                PhotonNetwork.LoadLevel("DeathRaceScene");
                //Death race mode

            }
        }
    }
    #endregion

    #region Pun Callbacks
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " room." + "Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        ActivatePanel(insideRoomPanel.name);
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
            {
                SetupRoomProps();

                if (playerListGameObjects == null)
                {
                    playerListGameObjects = new Dictionary<int, GameObject>();
                }
                UpdateRoomInfoText();
                FillPlayersList();
            }
        }
        startGameButton.SetActive(false);
        
    }

    

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfoText();
        DestroyPlayerFromRoom(otherPlayer);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerReady(targetPlayer, changedProps);
        startGameButton.SetActive(CheckPlayersReady());
    }

    

    public override void OnLeftRoom()
    {
        ActivatePanel(gameOptionsPanel.name);
        foreach(GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfoText();
        AddPlayerToList(newPlayer);
        startGameButton.SetActive(CheckPlayersReady());
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message + " creating own room");
        CreateRoom();
    }
    public override void OnConnected()
    {
        Debug.Log("Connected to internet");
    }
    public override void OnConnectedToMaster()
    {
        ActivatePanel(gameOptionsPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to photon servers.");
    }

    #endregion
    #region Public Methods
    public void SetGameMode(string gameMode)
    {
        this.gameMode = gameMode;
    }

    #endregion

    #region Private Methods
    private void SetupRoomProps()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            SetupRaceRoom();
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            SetupDeathRaceRoom();
        }
    }

    private void SetupRaceRoom()
    {
        gameModeText.text = "LET'S RACE!";
        panelBackground.sprite = racingBackground;
        for (int i = 0; i < playerSelectionUIGameObjects.Length; i++)
        {
            GameObject selectionObject = playerSelectionUIGameObjects[i];
            RacingPlayer rPlayer = racingPlayers[i];

            selectionObject.transform.Find("PlayerName").GetComponent<Text>().text = deathRacePlayers[i].playerName;
            selectionObject.GetComponent<Image>().sprite = rPlayer.playerSprite;
            selectionObject.transform.Find("PlayerProperty").GetComponent<Text>().text = "";
        }
    }
    private void SetupDeathRaceRoom()
    {
        gameModeText.text = "DEATH RACE!";
        panelBackground.sprite = deathRaceBackground;
        //Death race game mode
        for (int i = 0; i < playerSelectionUIGameObjects.Length; i++)
        {
            GameObject selectionObject = playerSelectionUIGameObjects[i];
            DeathRacePlayer drPlayer = deathRacePlayers[i];

            selectionObject.transform.Find("PlayerName").GetComponent<Text>().text = deathRacePlayers[i].playerName;
            selectionObject.GetComponent<Image>().sprite = drPlayer.playerSprite;
            selectionObject.transform.Find("PlayerProperty").GetComponent<Text>().text = drPlayer.weaponName + ": Damage: " + drPlayer.damage + " FireRate: " + drPlayer.fireRate;
        }
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient) return false;

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if(player.CustomProperties.TryGetValue(CustomPropsKeeper.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady) return false;
            }
            else
            {
                return false;
            }
        }
        return true;

    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(CheckPlayersReady());
    }
    private void UpdateRoomInfoText()
    {
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name +
                            " Players/Max players: " + PhotonNetwork.CurrentRoom.PlayerCount +
                            "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    private void UpdatePlayerReady(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if (playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(CustomPropsKeeper.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
            }
        }
    }
    private void FillPlayersList()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(playerListPrefab, playerListContent);
            playerListGameObject.transform.localScale = Vector3.one;
            PlayerListEntryInitializer initializer = playerListGameObject.GetComponent<PlayerListEntryInitializer>();          
            initializer.Initialize(player.ActorNumber, player.NickName);
            playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
            object isPlayerReady;
            if(player.CustomProperties.TryGetValue(CustomPropsKeeper.PLAYER_READY, out isPlayerReady))
            {
                initializer.SetPlayerReady((bool)isPlayerReady);
            }
        }
    }
    
    private void AddPlayerToList(Player newPlayer)
    {
        GameObject playerListGameObject = Instantiate(playerListPrefab, playerListContent);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
    }

    private void DestroyPlayerFromRoom(Player otherPlayer)
    {
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }
    private void ActivatePanel(string panelName)
    {
        loginPanel.SetActive(panelName.Equals(loginPanel.name));
        connectingPanel.SetActive(panelName.Equals(connectingPanel.name));
        gameOptionsPanel.SetActive(panelName.Equals(gameOptionsPanel.name));
        createRoomPanel.SetActive(panelName.Equals(createRoomPanel.name));
        creatingRoomInfoPanel.SetActive(panelName.Equals(creatingRoomInfoPanel.name));
        insideRoomPanel.SetActive(panelName.Equals(insideRoomPanel.name));
        joinRandomRoomPanel.SetActive(panelName.Equals(joinRandomRoomPanel.name));
    }
    
    private void CreateRoom()
    {
        ActivatePanel(creatingRoomInfoPanel.name);
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;

        string[] roomPropsInLobby = { "gm" }; //gm == game mode
        //2 game modes
        //1. racing = "rc"
        //2. death race = "dr"


        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };

        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion
}
