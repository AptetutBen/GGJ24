using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectTrigger : MonoBehaviour
{
    public PlayerAffectSystem.AffectName affectName;

    private void Start(){
        if (affectName == PlayerAffectSystem.AffectName.Default){
            WeekendLogger.LogError($"Please don't use Default affect on a trigger. ({this.gameObject.name})");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OwnerPlayer"))
        {
			NetworkPlayer player = other.transform.parent.GetComponent<NetworkPlayer>();
            player.playerAffects.AddAffect(affectName);
		}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OwnerPlayer"))
        {
			NetworkPlayer player = other.transform.parent.GetComponent<NetworkPlayer>();
            player.playerAffects.RemoveAffect(affectName);
		}
    }
}
