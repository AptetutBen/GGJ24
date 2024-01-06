using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform cursor;
    public SpriteRenderer cursorSprite;
    public TextMeshPro playerNameText;
    public Color playerColour;

    private Camera mainCamera;

    public SpawnedNetworkObject spawnObjectPrefab;
    [SerializeField] private NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("Player Name", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    private void Awake()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        mainCamera = Camera.main;

        if (!IsOwner)
        {
            return;
        }

        cursorSprite.sortingLayerName = "Owner Player";

        SetPlayerClientRPC(GameFlowController.playerColor, GameFlowController.playerName);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    [ClientRpc]
    public void SetPlayerClientRPC(Color color, string playerName)
    {
        cursorSprite.color = color;
        playerNameText.color = new Color(color.r, color.g, color.b, 0.5f);
        playerNameText.text = playerName;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        MoveCursor(GetMousePosition());

        if (Input.GetMouseButtonDown(0))
        {
            SpawnNetworkObjectServerRPC(GetMousePosition(), playerColour);
        }
    }

    [ServerRpc]
    private void SpawnNetworkObjectServerRPC(Vector3 position,Color color)
    {
        SpawnedNetworkObject spawnObject = Instantiate(spawnObjectPrefab, position, Quaternion.identity);
        spawnObject.Initialise(this);
    }

    public void MoveCursor(Vector3 pos)
    {
        cursor.transform.position = pos;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }
}
