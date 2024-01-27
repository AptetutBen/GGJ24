
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;



public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;

    List<SpawnArea> spawnAreas = new List<SpawnArea>();
	public static SpawnManager instance{
        get{
            if(_instance == null){
                GameObject newSpawnManager = Instantiate(Resources.Load("SpawnManager", typeof(GameObject))) as GameObject;
                newSpawnManager.GetComponent<SpawnManager>();
            }

            return _instance;
        }
    }

	private void Awake()
	{
		_instance = this;
    }

    public void RegisterSpawnArea(SpawnArea spawn){
        spawnAreas.Add(spawn);
    }
    
    public void UnRegisterSpawnArea(SpawnArea spawn){
        spawnAreas.Remove(spawn);
    }

    public SpawnArea GetRandomSpawn(){
        return spawnAreas[Random.Range(0, spawnAreas.Count)];
    }
}