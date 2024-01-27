using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameTrigger : NetworkBehaviour
{
    public float delay;
    public float resetTime;
    
    public UnityEvent onTrigger;

    private float nextAllowedTrigger = 0;
    private float? invokeAfterTime = null;



    private void Start(){
        
    }

    private void Update(){
        if(invokeAfterTime != null){
            if (Time.time > invokeAfterTime){
                invokeAfterTime = null;
                onTrigger.Invoke();
            }
        }
    }
    

    [ClientRpc]
    private void FireEventClientRPC(float delay)
    {
        if(Time.time > nextAllowedTrigger){
            nextAllowedTrigger = Time.time + resetTime;
            invokeAfterTime = Time.time + delay;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OwnerPlayer"))
        {
            if(Time.time > nextAllowedTrigger){
                FireEventClientRPC(delay);
            }
		}
    }

}
