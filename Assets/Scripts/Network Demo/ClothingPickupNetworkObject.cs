using System;
using System.Collections;
using System.Drawing;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Color = UnityEngine.Color;

public class ClothingPickupNetworkObject : NetworkBehaviour
{
	private NetworkPlayer owner;
	private NetworkObject networkObject;
	private float minMoveDistanceForNetwork = 0.1f;
	public NetworkPlayer Owner => owner;
	public float smoothingFactor = 0.2f;

	private readonly NetworkVariable<ClothingNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);

	public NetworkVariable<FixedString128Bytes> clothingId = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	public Renderer rend;
	public Rigidbody rb;
	public Clothing clothing;
	public SpriteRenderer spriteImage;

	private void Awake()
	{
		clothingId.OnValueChanged += OnIdChanged;
		networkObject = GetComponent<NetworkObject>();
	}

	private void OnIdChanged(FixedString128Bytes oldClothingId, FixedString128Bytes newClothingId )
	{
		clothing = ClothingManager.instance.GetItemByID(newClothingId.ToString());
		spriteImage.sprite = ClothingManager.instance.GetPickupSpriteFromId(clothing.spriteName, clothing.type);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OwnerPlayer"))
        {
			other.transform.parent.GetComponent<NetworkPlayer>().PickUpClothing(this);
			gameObject.SetActive(false);
			DestoryServerRPC();
		}
    }

	public void Initialise(string id)
    {
		clothingId.Value = id;
	}

    public override void OnNetworkSpawn()
	{
		if(clothing != null)
			spriteImage.sprite = ClothingManager.instance.GetPickupSpriteFromId(clothing.spriteName, clothing.type);

		if (IsOwner)
		{
			//CommitNetworkClothingIdServerRPC(clothingId.Value);
		}
		else
		{
			Destroy(rb);
			clothing = ClothingManager.instance.GetItemByID(clothingId.Value.ToString());
		}

		if (!IsOwner)
		{
			return;
		}
	}

    private void FixedUpdate()
    {
		Move();
	}


    public void Move()
	{
		if (IsOwner)
		{
			if(Vector3.Distance(transform.position, netState.Value.Position) < minMoveDistanceForNetwork)
            {
				return;
            }

			netState.Value = new ClothingNetworkData()
			{
				Position = transform.position
			};
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, netState.Value.Position, smoothingFactor); 
		}
	}



	[ServerRpc]
	private void CommitNetworkClothingIdServerRPC(String id)
	{
		clothingId.Value = id;
	}

	private bool isBeingDistroyed;

	public void PickUpObject()
    {
        if (isBeingDistroyed)
        {
			return;
        }
		isBeingDistroyed = true;
		Debug.Log("bye");
		DestoryServerRPC();
	}


	[ServerRpc(RequireOwnership = false)]
	public void DestoryServerRPC()
	{
		networkObject.Despawn();
	}

	//[ClientRpc]
	//public void InitialiseClientRPC()
	//{
	//	clothing = ClothingManager.instance.GetItemByID(newClothingId);
	//}

	struct ClothingNetworkData : INetworkSerializable
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

}