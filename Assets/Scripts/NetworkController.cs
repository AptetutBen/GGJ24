using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;
    private NetworkManager networkManager;
    private UnityTransport unityTransport;

    public string IPAddress => unityTransport.ConnectionData.Address;
    public string Port => unityTransport.ConnectionData.Port.ToString();
    public bool IsConnectedClient => networkManager.IsConnectedClient;
    public bool IsConnectedHost => networkManager.IsHost;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        unityTransport = GetComponent<UnityTransport>();
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    public void StartHost()
    {
        networkManager.StartHost();
    }

    public void StartClient()
    {
        StartClient(GameFlowController.ipAddress, GameFlowController.host);
    }

    public void StartClient(string ip, ushort port)
    {
        unityTransport.ConnectionData.Address = ip;
        unityTransport.ConnectionData.Port = port;
        networkManager.StartClient();
    }


    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(10, 10, 300, 300));

    //    var networkManager = NetworkManager.Singleton;
    //    if (!networkManager.IsClient && !networkManager.IsServer)
    //    {
    //        if (GUILayout.Button("Host"))
    //        {

    //        }

    //        if (GUILayout.Button("Client"))
    //        {
    //            networkManager.StartClient();
    //        }

    //        if (GUILayout.Button("Server"))
    //        {
    //            networkManager.StartServer();
    //        }
    //    }
    //    else
    //    {
    //        GUILayout.Label($"Mode: {(networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client")}");

    //        // "Random Teleport" button will only be shown to clients
    //        if (networkManager.IsClient)
    //        {
    //            if (GUILayout.Button("Random Teleport"))
    //            {
    //                if (networkManager.LocalClient != null)
    //                {
    //                    // Get `BootstrapPlayer` component from the player's `PlayerObject`
    //                    if (networkManager.LocalClient.PlayerObject.TryGetComponent(out BootstrapPlayer bootstrapPlayer))
    //                    {
    //                        // Invoke a `ServerRpc` from client-side to teleport player to a random position on the server-side
    //                        bootstrapPlayer.RandomTeleportServerRpc();
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    GUILayout.EndArea();
    //}
}

