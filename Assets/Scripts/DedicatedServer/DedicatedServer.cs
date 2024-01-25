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
        if(Time.time > 30 && this.numberOfPlayers <= 1)
            DedicatedServer.shouldTerminate = true;

        this.shouldTerminate = DedicatedServer.shouldTerminate;
        this.numberOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        this.serverUptimeInSeconds = Time.time;
        this.gameMode = "What is life";
        this.level = "Late Stage Capitalism";
    }
}

public class DedicatedServer : MonoBehaviour
{
    bool shouldStartGameServer = false;
    public static bool isDedicatedServer = false;
    public static bool shouldTerminate = false;
    public static PotatoServerInfo serverInfo;
    private PotatoWebServer webServer;
    
    private CancellationTokenSource cancelSource;

    void Awake(){
        // Ohhhhhhhhhhhhhhhh boi I'm a dedicated server weeeeeeeeeewwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww
        // Let's goooooooooooooooooooooooooooooooooooooooooooooo
        isDedicatedServer = true;
        DontDestroyOnLoad(this.gameObject);
        serverInfo = new PotatoServerInfo();
    }

    // Start is called before the first frame update
    void Start()
    {


        Debug.Log("Loading Main Game scene");
        SceneManager.LoadScene("Main Game");
        shouldStartGameServer = true;

        Application.targetFrameRate = 30;
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldStartGameServer == true){
            if(NetworkController.instance != null){
                Debug.Log("Starting game server");
                // NetworkController.instance.StartHost();
                shouldStartGameServer = false;
                
                //HTeeeTeePeeeee time
                webServer = new PotatoWebServer();
                cancelSource = new CancellationTokenSource();
                webServer.StartWebServerWeeewwwww(cancelSource.Token);
            }else{
                Debug.Log("Waiting for NetworkController.instance");
            }
                
        }

        serverInfo.UpdateInfo();
    }

    void OnDestroy()
    {
        Debug.Log("Clean up that http server :o");
        cancelSource.Cancel();
    }
}
