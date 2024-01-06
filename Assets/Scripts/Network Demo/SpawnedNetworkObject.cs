using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnedNetworkObject : NetworkBehaviour
{
    private NetworkPlayer owner;
    private NetworkObject networkObject;

    public NetworkPlayer Owner => owner;

    public SpriteRenderer spriteRenderer;

    public void Initialise(NetworkPlayer owner)
    {
        this.owner = owner;
        networkObject = GetComponent<NetworkObject>();
        networkObject.Spawn();

        StartCoroutine(timedDestory());
        InitialiseClientRPC(Color.red);
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
    public void InitialiseClientRPC(Color color)
    {
        spriteRenderer.color = color;
    }
}
