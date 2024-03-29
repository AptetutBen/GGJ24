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

    public LayerMask groundLayerMask;

    [Space]

    public Transform player;
    public Rigidbody rb;
    public SpriteRenderer[] playerSkinSprites;
    public TextMeshPro playerNameText;
    public GameObject ownerOnlyObject;
    public PlayerAffectSystem playerAffects = new PlayerAffectSystem();

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
    public Animator animator;
    private bool jumpPressed;
    private bool dashPressed;
    private bool isDashing;
    public int jumpCount = 0;
    private bool facingLeft;
    private float nextDash = 0;
    private float? dashUntil = 0;
    private Vector3 dashDireection;

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

    private void SpawnPickUpParticles(float height)
    {
        GameObject particle = ParticleManager.instance.PickUpClothesSpawn();
        particle.transform.SetParent(player);
        particle.transform.localPosition = Vector3.up * height;
    }

    private void ChangeHat(string id)
    {
        SpawnPickUpParticles(2);

        AudioManager.instance.PlaySFX(id + "_hat", player.position);

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
        SpawnPickUpParticles(1);

        // AudioManager.instance.PlaySFX(id + "_top",player.position);
        AudioManager.instance.PlaySFX(id + "_top");

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
            CommitNetworkSkinColourServerRPC(GameFlowController.playerColor);
            player.tag = "OwnerPlayer";
            player.transform.position = SpawnManager.instance.GetRandomSpawn(SpawnManager.SpawnType.Player);
            player.gameObject.AddComponent<AudioListener>();
        }
        else
        {
            foreach (SpriteRenderer item in playerSkinSprites)
            {
                item.color = skinColour.Value;
            }
           
            playerNameText.text = playerName.Value.ToString();

            playerNameText.color = playerColour.Value;

            if(!string.IsNullOrEmpty(shirtID.ToString()))
            {
                ChangeShirt(shirtID.Value.ToString());
            }

            if(!string.IsNullOrEmpty(hatID.ToString()))
            {
                ChangeHat(hatID.Value.ToString());
            }

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


    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashPressed = true;
        }

    }

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
            if(dashPressed){
                dashPressed = false;
                PlayerDashed();
            }


            if(dashUntil != null){
                if(Time.time < dashUntil){
                    rb.velocity = dashDireection * playerAffects.GetDashSpeed();
                    return;
                }else{
                    dashUntil = null;
                }
            }
            
            float speed = playerAffects.GetMoveSpeed();
            float jumpForce = playerAffects.GetJumpForce();
            float maxJumpCount = playerAffects.GetJumpCount();

            Vector3 pInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            if(pInput.x > 0)
            {
                littleGuy.transform.localScale = new Vector3(-1, 1, 1);
                facingLeft = false;
            }
            else if (pInput.x < 0)
            {
                littleGuy.transform.localScale = new Vector3(1, 1, 1);
                facingLeft = true;
            }

            bool isGrounded = IsGrounded();

            if (isGrounded)
            {
                jumpCount = 1; // DOn't look at this

                if(nextDash == float.MaxValue) // Hi, Luis too
                    nextDash = Time.time + playerAffects.GetDashCooldown();
            }

            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("Speed", pInput.magnitude);
            pInput *= IsGrounded()?speed: playerAffects.GetAirMoveSpeed();

            if (jumpPressed && (isGrounded||jumpCount < maxJumpCount))
            {
                jumpCount++;
                pInput.y = jumpForce;
                animator.SetTrigger("Jump");
            }
            else
            {
                pInput.y = rb.velocity.y - playerAffects.GetGravity();
            }

            jumpPressed = false;


            rb.velocity = pInput;

            //if(rb.velocity < 0 )

            netState.Value = new PlayerNetworkData()
            {
                Position = player.transform.position,
                FacingLeft = facingLeft
            };
        }
        else
        {
            player.transform.position = netState.Value.Position;
            littleGuy.transform.localScale = netState.Value.FacingLeft ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
	}

    public void PlayerDashed(){
        if(rb.velocity.x == 0 && rb.velocity.z == 0)
            return; // Player is not moving
        if(nextDash > Time.time)
            return; // On cooldown

        nextDash = float.MaxValue;
        dashUntil = Time.time + playerAffects.GetDashDuration();
        dashDireection = rb.velocity.normalized;
    }

    public void SteppedOnJumpPad(){
        Vector3 temp = rb.velocity;
        
        temp.y = playerAffects.GetjumpPadForce();
        
        rb.velocity = temp;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }

    /*
    // A little debug as a treat
    public void OnDrawGizmos(){
        WeekendLogger.Log($"Drawing Gizmo: {player.position - new Vector3(0, 5f, 0)}");
        Gizmos.DrawSphere(player.position + new Vector3(0, 0.3f, 0), 0.2f);
    }
    */

    public bool IsGrounded()
    {
        // Adjust the position of the sphere to be just below the character's feet
        Vector3 spherePosition = player.position + new Vector3(0, 0.3f, 0);

        // Perform a sphere cast to check for ground
        bool grounded = Physics.SphereCast(spherePosition, 0.2f, Vector3.down, out RaycastHit hitInfo, 0.7f, groundLayerMask);

        // If the sphere cast hits something within the specified layer, consider it grounded
        return grounded;
    }

    public void PickUpClothing(ClothingPickupNetworkObject pickedUpItem)
    {
        if(pickedUpItem.clothing == null)
            return;

        // This is bad
        FindObjectOfType<ClothesPanel>().UpdateClothes(pickedUpItem.clothing);

        switch (pickedUpItem.clothing.type)
        {
            case Clothing.ClothingType.Hat:
                if(hatSlotClothing != null){
                    playerAffects.RemoveAffect(hatSlotClothing.id);
                }

                hatSlotClothing = pickedUpItem.clothing;
                ChangeHatServerRPC(pickedUpItem.clothing.spriteName);
                playerAffects.AddAffect(hatSlotClothing.id);
                break;
            case Clothing.ClothingType.Top:
                if(topSlotClothing != null){
                    playerAffects.RemoveAffect(topSlotClothing.id);
                }

                topSlotClothing = pickedUpItem.clothing;
                ChangeShirtServerRPC(pickedUpItem.clothing.spriteName);
                playerAffects.AddAffect(topSlotClothing.id);
                break;
            default:
                break;
        }
    }

    [ServerRpc]
    private void ChangeHatServerRPC(string id)
    {
        hatID.Value = id;
    }

    [ServerRpc]
    private void ChangeShirtServerRPC(string id)
    {
        shirtID.Value = id;
    }
}



struct PlayerNetworkData : INetworkSerializable
{
    private float xPos, yPos, zPos;
    private bool facingLeft;

    internal bool FacingLeft {
        get => facingLeft;
        set
        {
            facingLeft = value;
        }
    }

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
        serializer.SerializeValue(ref facingLeft);

    }
}
