using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
	public static GameController instince; 
	public AudioClip musicClip;
	//public GameObject pausePanel;
	//public OptionsPanel optionsPanel;
	public List<Color> playerColours = new List<Color>();
	private NetworkController networkController;
	public ClothingPickupNetworkObject clothingPickupPrefab;


	// Debug Options
	[Space]
	public bool Debug_Use_Debug = false;
	public GameObject Debug_devPlayer;

	private void Awake()
	{
		instince = this;

#if !UNITY_EDITOR
		Debug_Use_Debug = false;
#endif

	}

	// Start is called before the first frame update
	void Start()
    {
        AudioManager.instance.SwitchMusicClip(musicClip);
		//pausePanel.SetActive(false);
		networkController = NetworkController.instance;

		Invoke("ConnectNetwork", 1);

		if(Debug_devPlayer != null)
        {
			Destroy(Debug_devPlayer);
        }
    }

	private void ConnectNetwork()
    {
		
		// For testing in editor 
        if (!GameFlowController.loadedOtherScene)
        {
            if(DedicatedServer.isDedicatedServer){
				networkController.StartServer();
			}else if (!networkController.IsDefaultServer())
            {
				Debug.Log("Editor Loading Starting Client");
				GameFlowController.SetClientQuickStart();
				networkController.StartClient();
			}
            else
            {
				Debug.Log("Editor Loading Starting Host");
				GameFlowController.SetHostQuickStart();
				networkController.StartHost();
			}
			return;
        }

		if (networkController.IsDefaultServer()){
				networkController.StartHost();
		}else{
				networkController.StartClient();
		}
	}

    public override void OnNetworkSpawn()
    {
		if (IsServer)
		{
			StartCoroutine(SpawnClothesPickupCoroutine());
		}
	}

    private IEnumerator SpawnClothesPickupCoroutine()
    {
        while (true)
        {
			SpawnNetworkObjectServerRPC((Random.insideUnitSphere * 10) + Vector3.up * 10, Color.blue);

			yield return new WaitForSeconds(5);
		}
    }



	[ServerRpc]
	private void SpawnNetworkObjectServerRPC(Vector3 position, Color color)
	{
		ClothingPickupNetworkObject spawnObject = Instantiate(clothingPickupPrefab, position, Quaternion.identity);
		spawnObject.GetComponent<NetworkObject>().Spawn();
		spawnObject.clothingId.Value = ClothingManager.instance.GetRandomItem().id;
	}



	//public void PauseGame()
	//{
	//	Time.timeScale = 0;
	//	pausePanel.SetActive(true);
	//}

	//public void UnPauseGame()
	//{
	//	Time.timeScale = 1;
	//	pausePanel.SetActive(false);
	//	optionsPanel.Hide(true);
	//}

	public void OnResumeButtonPress()
	{
		//UnPauseGame();
	}

	public void OnMainMenuButtonPress()
	{
		//UnPauseGame();
		GameFlowController.LoadScene("Main Menu", false);
	}

	public void OnOptionsButtonPress()
	{
		//optionsPanel.Show();
	}

	public void OnExitButtonPress()
	{
		Application.Quit();
	}

}