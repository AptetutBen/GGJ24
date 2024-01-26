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

    public float speed = 3;
    public float jumpForce = 5;
    public LayerMask groundLayerMask;

    [Space]

    public Transform player;
    public Rigidbody rb;
    public SpriteRenderer playerSprite;
    public TextMeshPro playerNameText;
    public GameObject playerCamera;

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

    private void OnColourChanged(Color prev, Color next) => playerSprite.color = next;
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
            playerSprite.color = playerColour.Value;
            playerNameText.text = playerName.Value.ToString();

            playerNameText.color = playerColour.Value;

            Destroy(rb);
            Destroy(playerCamera);
        }

		if (!IsOwner)
        {
            return;
        }

        playerSprite.sortingLayerName = "Owner Player";

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
    void FixedUpdate()
    {
		Move();

		if (!IsOwner)
        {
            return;
        }
    }

    [ServerRpc]
    private void SpawnNetworkObjectServerRPC(Vector3 position,Color color)
    {
		SpawnedNetworkObject spawnObject = Instantiate(spawnObjectPrefab, position, Quaternion.identity);
        spawnObject.GetComponent<NetworkObject>().Spawn();
        spawnObject.playerColour.Value = color;

	}

    public void Move()
    {
        if (IsOwner)
        {
            Vector3 pInput = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));

            pInput *= speed;

            if (Input.GetKey(KeyCode.Space) && IsGrounded())
            {
                Debug.Log("here");
                pInput.y = jumpForce;
            }
            else
            {
                pInput.y = rb.velocity.y;
            }

            rb.velocity = pInput;

            netState.Value = new PlayerNetworkData()
            {
                Position = player.transform.position
            };
        }
        else
        {
            player.transform.position = netState.Value.Position;
        }
	}

    private Vector3 GetMousePosition()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }

    public bool IsGrounded()
    {
        // Adjust the position of the sphere to be just below the character's feet
        Vector3 spherePosition = player.position - new Vector3(0, 0.55f, 0);

        // Perform a sphere cast to check for ground
        bool grounded = Physics.SphereCast(spherePosition, 0.3f, Vector3.down, out RaycastHit hitInfo, 0.4f, groundLayerMask);

        // If the sphere cast hits something within the specified layer, consider it grounded
        return grounded;
    }
}


struct PlayerNetworkData : INetworkSerializable
{
    private float xPos, yPos, zPos;

    internal Vector3 Position
    {
        get => new Vector3(xPos, yPos, zPos);
        set
        {
            xPos = value.x;
            yPos = value.y;
            zPos = value.z;
        }
    }

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
        serializer.SerializeValue(ref xPos);
		serializer.SerializeValue(ref yPos);
        serializer.SerializeValue(ref zPos);

    }
}
