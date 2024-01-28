using UnityEngine;

public class ObjectiveArea : MonoBehaviour
{
    Collider collider;
    public bool activeObjective = false;

    private void Awake(){
        ObjectiveManager.instance.RegisterObjectiveArea(this);
        collider = gameObject.GetComponent<Collider>();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        WeekendLogger.Log("Awake!");
    }

    private void OnDestroy(){
        ObjectiveManager.instance.UnRegisterObjectiveArea(this);
    }

    public Vector3 GetSpawnPoint(){
        
        float minX = collider.bounds.size.x * -0.5f;
        float minY = collider.bounds.size.y * -0.5f;
        float minZ = collider.bounds.size.z * -0.5f;

        return new Vector3(Random.Range (minX, -minX) + transform.position.x,
                Random.Range (minY, -minY)+ transform.position.y,
                Random.Range (minZ, -minZ)+ transform.position.z
        );

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(activeObjective == false)
            return;

        if (other.CompareTag("OwnerPlayer") || other.CompareTag("Player"))
        {
            // Thing!
            NetworkPlayer player = other.GetComponent<NetworkPlayer>();
            if(player == null || player.IsServer == false){
                return;
            }

            //Servers only!
            GameController.instince.SetNewObjective();
		}
    }
}