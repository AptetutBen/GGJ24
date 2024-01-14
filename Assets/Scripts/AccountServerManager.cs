using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public enum MessageType{
    UserInfo = 1,
    LobbyInfo = 2
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

    private static Dictionary<string, List<Action<string>>> networkEventCallback = new();

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

    // Register for callbacks when a message of a certian type gets recieved
    public void RegisterRecieveMessageCallback(Action<string> action, MessageType messageType)
    {
        string messageTypeString = ((int)messageType).ToString();

		//If there are no callbacks
		if (!networkEventCallback.ContainsKey(messageTypeString))
		{
			networkEventCallback[messageTypeString] = new List<Action<string>>();
		}

        if (networkEventCallback[messageTypeString].Contains(action))
        {
            WeekendLogger.LogNetworkServer("RecieveData Callback Action is trying to be registered twice");
            return;
        }

        networkEventCallback[messageTypeString].Add(action);
	}

	// UnRegister for message callbacks 
	public void UnregisterRecieveMessageCallback(Action<string> action, MessageType messageType)
	{
		string messageTypeString = ((int)messageType).ToString();

		//If there are no callbacks
		if (!networkEventCallback.ContainsKey(messageTypeString) || !networkEventCallback[messageTypeString].Contains(action))
		{
			WeekendLogger.LogNetworkServer("RecieveData Callback Action is trying to be unregistered but isn't in the list");
            return;
		}

		networkEventCallback[messageTypeString].Remove(action);
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
                    WeekendLogger.LogNetworkServer("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
					WeekendLogger.LogNetworkServer("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
					WeekendLogger.LogNetworkServer("Received: " + webRequest.downloadHandler.text);
                    sessionToken = webRequest.downloadHandler.text;
                    wasSuccessful = true;
                    break;
            }

            onComplete(wasSuccessful);
        }
    }

    void Update(){
        if(socketConnection != null && socketConnection.ThereThingsToReadFromQueue()){
            byte[] dataFromAccountServer = socketConnection.ReadFromQueue();
            if(dataFromAccountServer != null){

                string message = Encoding.UTF8.GetString(dataFromAccountServer);
                //{"type":1,"data":{"userID":"d7b431a5-a3e5-41ac-ba05-207a5dccc493"}}

				Match m = Regex.Match(message, @"{""type"":(.*),""data"":(.*)}");
				string messageTypeId = m.Groups[1].Value;
				string data = m.Groups[2].Value;

                if (networkEventCallback.ContainsKey(messageTypeId))
                {
                    foreach (Action<string> callback in networkEventCallback[messageTypeId])
                    {
                        callback?.Invoke(data);
					}
				}

				WeekendLogger.LogNetworkServer($"Message received: \"{message}\"");
            }
        }

        if(currentStateFromBackgroundThread != null){
            ChangeState((AccountServerState) currentStateFromBackgroundThread);
            currentStateFromBackgroundThread = null;
        }
    }
}