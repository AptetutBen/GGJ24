
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;



public class SpawnManager : MonoBehaviour
{
    public enum SpawnType{
        Clothing,
        Player
    }

    private static SpawnManager _instance;

    List<SpawnArea> playerSpawnAreas = new List<SpawnArea>();

    List<SpawnArea> clothingSpawnAreas = new List<SpawnArea>();

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

    public void RegisterSpawnArea(SpawnArea spawn, SpawnType type){
        if(type == SpawnType.Player){
            playerSpawnAreas.Add(spawn);
        }else if(type == SpawnType.Clothing){
            clothingSpawnAreas.Add(spawn);
        }else{
            WeekendLogger.Log("Unknown spawn type please extend me.");
        }
    }
    
    public void UnRegisterSpawnArea(SpawnArea spawn, SpawnType type){
        if(type == SpawnType.Player){
            playerSpawnAreas.Remove(spawn);
        }else if(type == SpawnType.Clothing){
            clothingSpawnAreas.Remove(spawn);
        }else{
            WeekendLogger.Log("Unknown spawn type please extend me.");
        }
    }

    public Vector3 GetRandomSpawn(SpawnType type){
        
        if(type == SpawnType.Player){
            if(playerSpawnAreas.Count == 0){
                WeekendLogger.LogError("Scene requires a PLAYER spawn area please");
                return new Vector3(0,0,0);
            }

            return playerSpawnAreas[Random.Range(0, playerSpawnAreas.Count)].GetSpawnPoint();
        }else if(type == SpawnType.Clothing){
            if(clothingSpawnAreas.Count == 0){
                WeekendLogger.LogError("Scene requires a CLOTHING spawn area please");
                return new Vector3(0,0,0);
            }

            return clothingSpawnAreas[Random.Range(0, clothingSpawnAreas.Count)].GetSpawnPoint();
        }else{
            WeekendLogger.Log("Unknown spawn type please extend me.");
            return new Vector3(0,0,0);
        }

        
    }
}