using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    Collider collider;
    private void Awake(){
        SpawnManager.instance.RegisterSpawnArea(this);
        collider = gameObject.GetComponent<Collider>();
    }

    private void OnDestroy(){
        SpawnManager.instance.UnRegisterSpawnArea(this);
    }

    public Vector3 GetSpawnPoint(){
        
        float minX = collider.bounds.size.x * -0.5f;
        float minY = collider.bounds.size.y * -0.5f;
        float minZ = collider.bounds.size.z * -0.5f;

        return (Vector3)gameObject.transform.TransformPoint(
            new Vector3(Random.Range (minX, -minX),
                Random.Range (minY, -minY),
                Random.Range (minZ, -minZ))
        );
    }
}