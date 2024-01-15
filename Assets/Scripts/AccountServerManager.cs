using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public enum MessageType{
    UserInfo        = 1,  // MessageUserInfo
    LobbyInfo       = 2,  // MessageLobbyInfo
    JoinLobby       = 3,  // (send back a LobbyInfo)
    LeaveLobby      = 4,  // (send back a LobbyInfo)
    KickPlayer      = 5,  // (send back a LobbyInfo)
    UpdateUser      = 6,  // (1. submitting person get's a MessageUserInfo)(2. everyone including you get's a LobbyInfo)
    StartGame       = 7,  // Sent by the game client when it's ready (automatically or maybe when the player clicks start etc)
    Ready           = 8,  // everyone get's a MessasgeReady 
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

    private static Dictionary<MessageType, List<Action<AccountServerMessage>>> messageCallbackLookup = new();

	private static AccountServerManager _instance; 
    private string registerURL = "https://ggj24.games.luisvsm.com/server/account/guest";
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

    // Register for callbacks when a message of a certian type gets recieved
    public void RegisterRecieveMessageCallback(Action<AccountServerMessage> action, MessageType messageType)
    {
		//If there are no callbacks
		if (!messageCallbackLookup.ContainsKey(messageType))
		{
            messageCallbackLookup[messageType] = new();
		}

        if (messageCallbackLookup[messageType].Contains(action))
        {
            WeekendLogger.LogNetworkServer("RecieveData Callback Action is trying to be registered twice");
            return;
        }

        messageCallbackLookup[messageType].Add(action);
	}

	// UnRegister for message callbacks 
	public void UnregisterRecieveMessageCallback(Action<AccountServerMessage> action, MessageType messageType)
	{
		//If there are no callbacks
		if (!messageCallbackLookup.ContainsKey(messageType) || !messageCallbackLookup[messageType].Contains(action))
		{
			WeekendLogger.LogNetworkServer("RecieveData Callback Action is trying to be unregistered but isn't in the list");
            return;
		}

		messageCallbackLookup[messageType].Remove(action);
	}

	public void ConnectToAccountServer(Action<bool, string> onComplete)
	{
		if (currentState == AccountServerState.NotConnected)
		{
			ChangeState(AccountServerState.Authenticating);
			StartCoroutine(RegisterGuestAccount((wasSuccessful, errorMessage) => {
				if (wasSuccessful)
				{
					// Do TCP connection
					socketConnection = new AccountServerSocketConnection();
					socketConnection.ConnectToAccountServer(sessionToken, ChangeStateBackgroundThread);
					onComplete(wasSuccessful, errorMessage);
				}
				else
				{
					ChangeState(AccountServerState.NotConnected);
					onComplete(wasSuccessful, errorMessage);
				}
			}));
		}
	}

	IEnumerator RegisterGuestAccount(Action<bool, string> onComplete)
	{
		using (UnityWebRequest webRequest = UnityWebRequest.Get(registerURL))
		{
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			bool wasSuccessful = false;
			string error = "";

			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
					WeekendLogger.LogNetworkServer("Error: " + webRequest.error);
                    error = webRequest.error;

					break;
				case UnityWebRequest.Result.ProtocolError:
					WeekendLogger.LogNetworkServer("HTTP Error: " + webRequest.error);
					error = webRequest.error;

					break;
				case UnityWebRequest.Result.Success:
					WeekendLogger.LogNetworkServer("Received: " + webRequest.downloadHandler.text);
					sessionToken = webRequest.downloadHandler.text;
					wasSuccessful = true;
					break;
			}

			onComplete(wasSuccessful, error);
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
			    MessageType messageType = (MessageType)BitConverter.ToUInt16(dataFromAccountServer, 0);
                string messageData = Encoding.UTF8.GetString(dataFromAccountServer, 2, dataFromAccountServer.Length - 2);
            
                AccountServerMessage message;
                WeekendLogger.LogNetworkServer($"Message type {messageType} received: \"{messageData}\"");


				    switch (messageType)
				    {
					    case MessageType.UserInfo:
						    message = JsonUtility.FromJson<MessageUserInfo>(messageData);
						    break;
					    case MessageType.LobbyInfo:
						    message = JsonUtility.FromJson<MessageLobbyInfo>(messageData);
						    break;
					    case MessageType.Ready:
						    message = JsonUtility.FromJson<MessageReady>(messageData);
						    break;
					    case MessageType.Chat:
						    message = JsonUtility.FromJson<MessageChat>(messageData);
						    break;

					    default:
						    WeekendLogger.LogNetworkServer($"Unknown message type {messageType} received: \"{messageData}\"");
						    message = null;
						    break;
				    }

					// If message is not null and the message callback lookup has registered callbacks, then do your thing
					if (message != null && messageCallbackLookup.ContainsKey(messageType))
					{
						foreach (Action<AccountServerMessage> callback in messageCallbackLookup[messageType])
						{
							callback?.Invoke(message);
						}
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

    public bool KickPlayer(string userID){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestKickPlayer(userID));
        return true;
    }

    public bool UpdateUser(UserData userData){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestUpdateUser(userData));
        return true;
    }

    public bool Ready(bool ready){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestReady(ready));
        return true;
    }

    public bool Chat(string chatData){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestChat(chatData));
        return true;
    }

    public bool StartGame(){
        if(currentState != AccountServerState.Connected)
            return false;

        socketConnection.SendMessage(new RequestStartGame());
        return true;
    }
}