using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        if(socketConnection != null && socketConnection.ThereThingsToReadFromQueue()){
            byte[] dataFromAccountServer = socketConnection.ReadFromQueue();
            if(dataFromAccountServer != null){
                Debug.Log($"Message received: \"{Encoding.UTF8.GetString(dataFromAccountServer)}\"");
            }
        }

        if(currentStateFromBackgroundThread != null){
            ChangeState((AccountServerState) currentStateFromBackgroundThread);
            currentStateFromBackgroundThread = null;
        }
    }
}