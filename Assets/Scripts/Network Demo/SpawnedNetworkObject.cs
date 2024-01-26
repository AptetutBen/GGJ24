using System;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Color = UnityEngine.Color;

public class SpawnedNetworkObject : NetworkBehaviour
{
	private NetworkPlayer owner;
	private NetworkObject networkObject;
	private float minMoveDistanceForNetwork = 0.1f;
	public NetworkPlayer Owner => owner;

	private readonly NetworkVariable<ClothingNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);

	public NetworkVariable<Color> playerColour = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	public Renderer rend;
	public Rigidbody rb;

	private void Awake()
	{
		playerColour.OnValueChanged += OnColourChanged;
	}

	private void OnColourChanged(Color prev, Color next) => rend.material.color = next;



	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			CommitNetworkColourServerRPC(GameFlowController.playerColor);
		}
		else
		{
			Destroy(rb);
			rend.material.color = playerColour.Value;
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
			transform.position = netState.Value.Position;
		}
	}



	[ServerRpc]
	private void CommitNetworkColourServerRPC(Color color)
	{
		playerColour.Value = color;
	}

	public void PickUpObject()
    {
		DestoryServerRPC();
	}


	[ServerRpc(RequireOwnership = false)]
	public void DestoryServerRPC()
	{
		networkObject.Despawn();
	}

	[ClientRpc]
	public void InitialiseClientRPC()
	{
		rend.material.color = playerColour.Value;
	}

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