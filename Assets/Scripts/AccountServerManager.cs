using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public enum MessageType{
    UserInfo        = 1,  // UserInfoMessasge
    LobbyInfo       = 2,  // LobbyInfoMessasge
    JoinLobby       = 3,  // (send back a LobbyInfo)
    LeaveLobby      = 4,  // (send back a LobbyInfo)
    KickPlayer      = 5,  // (send back a LobbyInfo)
    UpdateUser      = 6,  // (1. submitting person get's a UserInfoMessasge)(2. everyone including you get's a LobbyInfo)
    StartGame       = 7,  // Sent by the game client when it's ready (automatically or maybe when the player clicks start etc)
    Ready           = 8,  // (1. submitting person get's a UserInfoMessasge)(2. everyone including you get's a LobbyInfo)
    Chat            = 9,  // Sends back MessasgeChat to everyone
    GameSettings    = 10, // 
    ServerStatus    = 11, // Eg finding server, looking for players to match with, etc
    ServerInfo      = 12, // Eg where should the players join
}

public enum AccountServerState{
    NotConnected,
    Authenticating,
    Connecting,
    Connected,
    Reconnecting,
    WaitingToReconnect
}

public delegate void stateChangeCallback(AccountServerState newState);

public class AccountServerManager : MonoBehaviour
{
	public static AccountServerManager instance{
        get{
            if(_instance == null){
                GameObject newAccountServerManager = Instantiate(Resources.Load("AccountServerManager", typeof(GameObject))) as GameObject;
                newAccountServerManager.GetComponent<AccountServerManager>();
            }

            return _instance;
        }
    }

	private static AccountServerManager _instance; 
    private string registerURL = "http://localhost:3000/server/account/guest";
    public AccountServerState currentState;
    public AccountServerState? currentStateFromBackgroundThread;
    private AccountServerSocketConnection socketConnection = null;
    private string sessionToken;

	private void Awake()
	{
		_instance = this;
        currentState = AccountServerState.NotConnected;
    }

    void OnDestroy()
    {
        if(socketConnection != null){
            socketConnection.CloseConnectionToAccountServer();
            socketConnection = null;
        }
    }

    private stateChangeCallback onStateChange = (AccountServerState newState)=>{};
    private void ChangeStateBackgroundThread(AccountServerState newState){
        currentStateFromBackgroundThread = newState;
    }

    private void ChangeState(AccountServerState newState){
        currentState = newState;
        onStateChange(newState);
    }

    public void RegisterStateChangeCallback(stateChangeCallback newOnChange){
        onStateChange += newOnChange;
    }

    public void UnregisterStateChangeCallback(stateChangeCallback newOnChange){
        onStateChange -= newOnChange;
    }

    public void ConnectToAccountServer(Action<bool> onComplete){
        if(currentState == AccountServerState.NotConnected){
            ChangeState(AccountServerState.Authenticating);
            StartCoroutine(RegisterGuestAccount((wasSuccessful)=>{
                if(wasSuccessful){
                    // Do TCP connection
                    socketConnection = new AccountServerSocketConnection();
                    socketConnection.ConnectToAccountServer(sessionToken, ChangeStateBackgroundThread);
                    onComplete(wasSuccessful);
                }else{
                    ChangeState(AccountServerState.NotConnected);
                    onComplete(wasSuccessful);
                }
            }));
        }
    }
     
    IEnumerator RegisterGuestAccount(Action<bool> onComplete)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(registerURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            bool wasSuccessful = false;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    sessionToken = webRequest.downloadHandler.text;
                    wasSuccessful = true;
                    break;
            }

            onComplete(wasSuccessful);
        }
    }

    void Update(){
    if(currentStateFromBackgroundThread != null){
        ChangeState((AccountServerState) currentStateFromBackgroundThread);
        currentStateFromBackgroundThread = null;
    }

    if(socketConnection != null && socketConnection.ThereThingsToReadFromQueue()){
        byte[] dataFromAccountServer = socketConnection.ReadFromQueue();
        if(dataFromAccountServer != null){
            // First two bytes are a uint16 to denote what type of message it is
            // The rest of the message is JSON data of the message
            UInt16 messageType = BitConverter.ToUInt16(dataFromAccountServer, 0);
            string messageData = Encoding.UTF8.GetString(dataFromAccountServer, 2, dataFromAccountServer.Length - 2);
            
            AccountServerMessage message;
            Debug.Log($"Message type {messageType} received: \"{messageData}\"");

            switch ((MessageType) messageType){
                case MessageType.UserInfo:
                    message = JsonUtility.FromJson<MessasgeUserInfo>(messageData);
                    break;

                case MessageType.LobbyInfo:
                    message = JsonUtility.FromJson<MessasgeLobbyInfo>(messageData);
                    break;

                default:
                    Debug.Log($"Unknown message type {messageType} received: \"{messageData}\"");
                    message = null;
                    break;
            }

            if(message != null){
                // TODO BEN: Put callback call here
            }
        }
    }
}

    // TCP Communications Weeeewwwww :3
    public bool JoinLobby(string lobbyID){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestJoinLobby(lobbyID));
        return true;
    }
    
    public bool LeaveLobby(){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestLeaveLobby());
        return true;
    }
}