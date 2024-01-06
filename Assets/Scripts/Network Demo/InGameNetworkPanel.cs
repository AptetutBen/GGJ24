using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InGameNetworkPanel : NetworkBehaviour
{

    public TextMeshProUGUI playerCountText;
    private NetworkVariable<int> playerNumber = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone);
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;

        networkManager.OnClientConnectedCallback += ClientConnected;
        networkManager.OnClientDisconnectCallback += ClientDisconnected;
    }

    private void ClientConnected(ulong id)
    {
        if (!IsServer)
        {
            return;
        }
        UpdatePlayerCountClientRPC(networkManager.ConnectedClients.Count);
    }

    private void ClientDisconnected(ulong id)
    {
        if (!IsServer)
        {
            return;
        }
        UpdatePlayerCountClientRPC(networkManager.ConnectedClients.Count);
    }

    [ClientRpc]
    private void UpdatePlayerCountClientRPC(int value)
    {
        playerCountText.text = value.ToString();
    }


    private void Update()
    {
        
    }
}
