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

    public SpriteRenderer spriteRenderer;

    public Color objectColour;

    public void Initialise(NetworkPlayer owner,Color color)
    {
        this.owner = owner;
		objectColour = color;
		networkObject = GetComponent<NetworkObject>();
        networkObject.Spawn();

        StartCoroutine(timedDestory());
        InitialiseClientRPC();
        IEnumerator timedDestory()
        {
            yield return new WaitForSeconds(5);
            DestoryServerRPC();
        }
	}

    [ServerRpc(RequireOwnership = false)]
    public void DestoryServerRPC()
    {
        networkObject.Despawn();
    }

    [ClientRpc]
    public void InitialiseClientRPC()
    {
		spriteRenderer.color = objectColour;
	}

	public override void OnNetworkSpawn()
	{
		spriteRenderer.color = objectColour;
	}
}
