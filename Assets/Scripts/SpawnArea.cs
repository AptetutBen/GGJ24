using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    Collider collider;
    public SpawnManager.SpawnType type;

    private void Awake(){
        SpawnManager.instance.RegisterSpawnArea(this, type);
        collider = gameObject.GetComponent<Collider>();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        WeekendLogger.Log("Awake!");
    }

    private void OnDestroy(){
        SpawnManager.instance.UnRegisterSpawnArea(this, type);
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
}