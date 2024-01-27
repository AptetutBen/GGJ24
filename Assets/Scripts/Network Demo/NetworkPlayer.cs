using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.U2D.Animation;

public class NetworkPlayer : NetworkBehaviour
{
    private int index;

    public float speed = 3;
    public float jumpForce = 5;
    public LayerMask groundLayerMask;

    [Space]

    public Transform player;
    public Rigidbody rb;
    public SpriteRenderer[] playerSkinSprites;
    public Gradient skinColours;
    public TextMeshPro playerNameText;
    public GameObject ownerOnlyObject;

    private readonly NetworkVariable<PlayerNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);

    public NetworkVariable<Color> playerColour = new NetworkVariable<Color>(Color.blue,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<Color> skinColour = new NetworkVariable<Color>(Color.blue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString128Bytes> hatID = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString128Bytes> shirtID = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Transform littleGuy;
    public Clothing hatSlotClothing;
    public Clothing topSlotClothing;
    public Clothing pantsSlotClothing;

    public Dictionary<string, List<GameObject>> hatLookup = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> shirtLookup = new Dictionary<string, List<GameObject>>();

    private Camera mainCamera;

    private void Awake()
    {
        playerColour.OnValueChanged += OnColourChanged;
        skinColour.OnValueChanged += OnSkinColourChanged;
        hatID.OnValueChanged += OnHatIDChanged;
        shirtID.OnValueChanged += OnShirtIDChange;

        playerName.OnValueChanged += OnNameChanged;

        foreach (Transform child in littleGuy)
        {
            string childName = child.name;
            if (childName.Contains("hat_"))
            {
                int lastUnderscoreIndex = childName.LastIndexOf('_');
                string id = childName.Substring(lastUnderscoreIndex + 1);

                if (!hatLookup.ContainsKey(id))
                {
                    hatLookup[id] = new List<GameObject>();
                }

                hatLookup[id].Add(child.gameObject);
                child.gameObject.SetActive(false);
            }

            if (childName.Contains("sleeve") || childName.Contains("shirt_"))
            {
                int lastUnderscoreIndex = childName.LastIndexOf('_');
                string id = childName.Substring(lastUnderscoreIndex + 1);

                if (!shirtLookup.ContainsKey(id))
                {
                    shirtLookup[id] = new List<GameObject>();
                }

                shirtLookup[id].Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }
    }


    private void ChangeHat(string id)
    {
        foreach (var key in hatLookup.Keys)
        {
            foreach (var item in hatLookup[key])
            {
                item.SetActive(key == id);
            }
        }
    }


    private void ChangeShirt(string id)
    {
        foreach (var key in shirtLookup.Keys)
        {
            foreach (var item in shirtLookup[key])
            {
                item.SetActive(key == id);
            }
        }
    }


    private void OnColourChanged(Color prev, Color next)
    {
        playerNameText.color = next;
    }

    private void OnSkinColourChanged(Color prev, Color next)
    {
        foreach (SpriteRenderer item in playerSkinSprites)
        {
            item.color = next;
        }
    }

    private void OnShirtIDChange(FixedString128Bytes prev, FixedString128Bytes next)
    {
        ChangeShirt(next.ToString());
    }

    private void OnHatIDChanged(FixedString128Bytes prev, FixedString128Bytes next)
    {
        ChangeHat(next.ToString());
    }

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
            CommitNetworkSkinColourServerRPC(skinColours.Evaluate(Random.Range(0, 1f)));
            player.tag = "OwnerPlayer";
        }
        else
        {
            foreach (SpriteRenderer item in playerSkinSprites)
            {
                item.color = skinColour.Value;
            }
           
            playerNameText.text = playerName.Value.ToString();

            playerNameText.color = playerColour.Value;

            ChangeShirt(shirtID.ToString());
            ChangeShirt(hatID.ToString());

            Destroy(rb);
            Destroy(ownerOnlyObject);
        }

		if (!IsOwner)
        {
            return;
        }

        //playerSprite.sortingLayerName = "Owner Player";

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    [ServerRpc]
    private void CommitNetworkPlayerColourServerRPC(Color color)
    {
        playerColour.Value = color;
    }

    [ServerRpc]
    private void CommitNetworkSkinColourServerRPC(Color color)
    {
        skinColour.Value = color;
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

    public void Move()
    {
        if (IsOwner)
        {
            Vector3 pInput = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);

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

    public void PickUpClothing(ClothingPickupNetworkObject pickedUpItem)
    {
        Debug.Log(pickedUpItem.clothing.clothingName);

        switch (pickedUpItem.clothing.type)
        {
            case Clothing.ClothingType.Hat:
                hatSlotClothing = pickedUpItem.clothing;
                ChangeHat(pickedUpItem.clothing.id);
                break;
            case Clothing.ClothingType.Top:
                topSlotClothing = pickedUpItem.clothing;
                ChangeShirt(pickedUpItem.clothing.id);
                break;
            default:
                break;
        }
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
