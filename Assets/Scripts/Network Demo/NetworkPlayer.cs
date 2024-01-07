using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    private int index;
    public Transform cursor;
    public SpriteRenderer cursorSprite;
    public TextMeshPro playerNameText;

    private readonly NetworkVariable<PlayerNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);

    public NetworkVariable<Color> playerColour = new NetworkVariable<Color>(Color.blue,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
	public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	private Camera mainCamera;

    public SpawnedNetworkObject spawnObjectPrefab;

    private void Awake()
    {
        playerColour.OnValueChanged += OnColourChanged;
		playerName.OnValueChanged += OnNameChanged;
	}

    private void OnColourChanged(Color prev, Color next) => cursorSprite.color = next;
    private void OnNameChanged(FixedString128Bytes prev, FixedString128Bytes next)
    {
        playerNameText.text = next.ToString();
		playerNameText.color = playerColour.Value;
	}

	public override void OnNetworkSpawn()
    {
		mainCamera = Camera.main;

        if (IsOwner)
        {
			index = (int)OwnerClientId;
            CommitNetworkPlayerColourServerRPC(GameFlowController.playerColor);
            CommitNetworkPlayerNameServerRPC(GameFlowController.playerName);
		}
        else
        {
            cursorSprite.color = playerColour.Value;
            playerNameText.text = playerName.Value.ToString();

            playerNameText.color = playerColour.Value;
		}

		if (!IsOwner)
        {
            return;
        }

        cursorSprite.sortingLayerName = "Owner Player";

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    [ServerRpc]
    private void CommitNetworkPlayerColourServerRPC(Color color)
    {
        playerColour.Value = color;
    }

	[ServerRpc]
	private void CommitNetworkPlayerNameServerRPC(FixedString128Bytes playerName)
	{
        this.playerName.Value = playerName;

	}

	public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }


    // Update is called once per frame
    void Update()
    {
		MoveCursor();

		if (!IsOwner)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SpawnNetworkObjectServerRPC(GetMousePosition(), playerColour.Value);
        }
    }

    [ServerRpc]
    private void SpawnNetworkObjectServerRPC(Vector3 position,Color color)
    {
		SpawnedNetworkObject spawnObject = Instantiate(spawnObjectPrefab, position, Quaternion.identity);
        spawnObject.GetComponent<NetworkObject>().Spawn();

	}

    public void MoveCursor()
    {
        if (IsOwner)
        {
            netState.Value = new PlayerNetworkData()
            {
                Position = GetMousePosition()
            };
		}

        cursor.transform.position = netState.Value.Position;
	}

    private Vector3 GetMousePosition()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }
}

struct PlayerNetworkData : INetworkSerializable
{
    private float xPos, yPos;

    internal Vector3 Position
    {
        get => new Vector3(xPos, yPos, 0);
        set
        {
            xPos = value.x;
            yPos = value.y;
        }
    }

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
        serializer.SerializeValue(ref xPos);
		serializer.SerializeValue(ref yPos);
	}
}
