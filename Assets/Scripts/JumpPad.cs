using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private void Start(){
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OwnerPlayer"))
        {
			NetworkPlayer player = other.transform.parent.GetComponent<NetworkPlayer>();
            player.SteppedOnJumpPad();
		}
    }
}
