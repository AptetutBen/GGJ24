using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LobbyPanel : NetworkBehaviour
{
    public TextMeshProUGUI addressText,addressShadowText;
    public GameObject playerListPrefab;
    public Transform playerListParent;

    private Dictionary<NetworkPlayer, GameObject> lobbyListLookup = new Dictionary<NetworkPlayer, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        switch (GameFlowController.gameMode)
        {
            case GameFlowController.GameMode.Client:
                break;
            case GameFlowController.GameMode.Host:
                SetUpHostLobby();
                break;
            case GameFlowController.GameMode.Server:
                SetUpHostLobby();
                break;
            case GameFlowController.GameMode.Solo:
                SetUpHostLobby();
                break;
            default:
                break;
        }
    }

    private void SetUpHostLobby()
    {
        string ipAddress = NetworkController.instance.LocalIP;
        addressText.text = ipAddress;
        addressShadowText.text = ipAddress;
    }

    public void AddPlayer(NetworkPlayer networkPlayer)
    {
        if (!IsServer)
        {
            return;
        }
        GameObject newListItem = Instantiate(playerListPrefab, playerListParent);

        lobbyListLookup[networkPlayer] = newListItem;
    }

    public void RemovePlayer(NetworkPlayer networkPlayer)
    {
        if (!lobbyListLookup.ContainsKey(networkPlayer))
        {
            Debug.LogError("Player not found in lobby lookup");
            return;
        }

        Destroy(lobbyListLookup[networkPlayer]);
        lobbyListLookup.Remove(networkPlayer);
    }

}
