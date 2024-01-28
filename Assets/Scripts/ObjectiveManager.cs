
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;



public class ObjectiveManager : MonoBehaviour
{
    private static ObjectiveManager _instance;
    private int currentObjectiveID = 0;

    List<ObjectiveArea> objectiveAreas = new List<ObjectiveArea>();

	public static ObjectiveManager instance{
        get{
            if(_instance == null){
                GameObject newObjectiveManager = Instantiate(Resources.Load("ObjectiveManager", typeof(GameObject))) as GameObject;
                newObjectiveManager.GetComponent<ObjectiveManager>();
            }

            return _instance;
        }
    }

	private void Awake()
	{
		_instance = this;
    }

    public void RegisterObjectiveArea(ObjectiveArea objective){
        objectiveAreas.Add(objective);
    }
    
    public void UnRegisterObjectiveArea(ObjectiveArea objective){
        objectiveAreas.Remove(objective);
    }
    private void GetNewObjective(){
        if(objectiveAreas.Count > 1){
            int newObjective = Random.Range(0, objectiveAreas.Count);

            while (currentObjectiveID == newObjective)
            {
                newObjective = Random.Range(0, objectiveAreas.Count);
            }

            currentObjectiveID = newObjective;
        }
    }

    public Vector3 GetRandomObjective(){
        WeekendLogger.Log("GetRandomObjective");

        if(objectiveAreas.Count == 0){
            WeekendLogger.LogError("Scene requires an OBJECTIVE area please");
            return new Vector3(0,0,0);
        }

        objectiveAreas[currentObjectiveID].setObjective(false);
        GetNewObjective();
        objectiveAreas[currentObjectiveID].setObjective(true);

        return objectiveAreas[currentObjectiveID].GetSpawnPoint();
    }
}