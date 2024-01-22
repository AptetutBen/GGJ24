using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class DedicatedServer : MonoBehaviour
{
    bool shouldStartGameServer = false;
    public static bool isDedicatedServer = false;

    private PotatoWebServer webServer;
    
    private CancellationTokenSource cancelSource;

    // Start is called before the first frame update
    void Start()
    {
        // Ohhhhhhhhhhhhhhhh boi I'm a dedicated server weeeeeeeeeewwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww
        // Let's goooooooooooooooooooooooooooooooooooooooooooooo
        isDedicatedServer = true;
        DontDestroyOnLoad(this.gameObject);


        Debug.Log("Loading Main Game scene");
        SceneManager.LoadScene("Main Game");
        shouldStartGameServer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldStartGameServer == true){
            if(NetworkController.instance != null){
                Debug.Log("Starting game server");
                NetworkController.instance.StartHost();
                shouldStartGameServer = false;
                
                //HTeeeTeePeeeee time
                webServer = new PotatoWebServer();
                cancelSource = new CancellationTokenSource();
                webServer.StartWebServerWeeewwwww(cancelSource.Token);
            }else{
                Debug.Log("Waiting for NetworkController.instance");
            }
                
        }
    }

    void OnDestroy()
    {
        Debug.Log("Clean up that http server :o");
        cancelSource.Cancel();
    }
}
