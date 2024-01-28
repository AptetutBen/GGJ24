using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using Unity.Netcode;

[System.Serializable]
public class PotatoServerInfo{
    public bool shouldTerminate;
    public int numberOfPlayers;
    public float serverUptimeInSeconds;
    public string gameMode;
    public string level;

    public PotatoServerInfo(){
        this.shouldTerminate = false;
        this.numberOfPlayers = -1;
        this.serverUptimeInSeconds = -1;
        this.gameMode = "";
        this.level = "";
    }

    public void UpdateInfo(){

        if(NetworkManager.Singleton){
            if(NetworkManager.Singleton.IsServer == false){
                this.numberOfPlayers = 0;
                return;
            }
            this.numberOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;

            if(Time.time > 30 && this.numberOfPlayers < 1){
                DedicatedServer.shouldTerminate = true;
                this.shouldTerminate = DedicatedServer.shouldTerminate;
            }
        }else{
            this.numberOfPlayers = 0;
        }
        this.serverUptimeInSeconds = Time.time;
        this.gameMode = "What is life";
        this.level = "Late Stage Capitalism";
    }
}

public class DedicatedServer : MonoBehaviour
{
    public static GameFlowController.GameMode? newGameMode = null;
    public static bool isDedicatedServer = false;
    public static bool shouldTerminate = false;
    public bool serverIsRunning = false;   
    public static PotatoServerInfo serverInfo;
    private PotatoWebServer webServer;
    
    private CancellationTokenSource cancelSource;

    void Awake(){
        // Ohhhhhhhhhhhhhhhh boi I'm a dedicated server weeeeeeeeeewwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww
        // Let's goooooooooooooooooooooooooooooooooooooooooooooo
        isDedicatedServer = true;
        serverIsRunning = false;
        DontDestroyOnLoad(this.gameObject);
        serverInfo = new PotatoServerInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Set targetFrameRate");
        Application.targetFrameRate = 30;
        
        //HTeeeTeePeeeee time
        Debug.Log("Starting web server");
        webServer = new PotatoWebServer();
        cancelSource = new CancellationTokenSource();
        webServer.StartWebServerWeeewwwww(cancelSource.Token);
    }

    private float lastServerInfoLog;
    // Update is called once per frame
    void Update()
    {
        serverInfo.UpdateInfo();

        if(lastServerInfoLog + 5 < Time.time){
            lastServerInfoLog = Time.time;
            WeekendLogger.Log($"[{Mathf.Round(Time.time)}] Players: {serverInfo.numberOfPlayers} Clothes: {ClothingPickupNetworkObject.clothingCount}");
        }

        if(newGameMode != null){
            GameFlowController.ChangeGameMode((GameFlowController.GameMode) newGameMode);
            newGameMode = null;
        }
    }

    void OnDestroy()
    {
        Debug.Log("Clean up that http server :o");
        cancelSource.Cancel();
    }
}
