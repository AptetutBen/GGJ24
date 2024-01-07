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

    public NetworkPlayer Owner => owner;

	public NetworkVariable<Color> playerColour = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public SpriteRenderer spriteRenderer;
    public float duration;

	public SpriteRenderer spriteRenderer;

	private void Awake()
	{
		playerColour.OnValueChanged += OnColourChanged;
	}

	private void OnColourChanged(Color prev, Color next) => spriteRenderer.color = next;

	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			CommitNetworkColourServerRPC(GameFlowController.playerColor);
		}
		else
		{
			spriteRenderer.color = playerColour.Value;
		}

		if (!IsOwner)
		{
			return;
		}
	}

	[ServerRpc]
	private void CommitNetworkColourServerRPC(Color color)
	{
		playerColour.Value = color;
	}

	//public void Initialise(NetworkPlayer owner,Color color)
 //   {
 //       this.owner = owner;
	//	objectColour = color;
	//	networkObject = GetComponent<NetworkObject>();
 //       networkObject.Spawn();

 //       StartCoroutine(timedDestory());
 //       InitialiseClientRPC();
 //       IEnumerator timedDestory()
 //       {
 //           yield return new WaitForSeconds(5);
 //           DestoryServerRPC();
 //       }
	//}

    [ServerRpc(RequireOwnership = false)]
    public void DestoryServerRPC()
    {
        networkObject.Despawn();
    }

    [ClientRpc]
    public void InitialiseClientRPC()
    {
        spriteRenderer.color = color;
    }

    public void Awake()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / duration;
            spriteRenderer.material.SetFloat("_Value", timer);

            Debug.Log(timer);
            yield return null;
        }

        timer = 0;
    }
		spriteRenderer.color = objectColour;
	}

}
