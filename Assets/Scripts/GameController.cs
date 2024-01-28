using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
	public static GameController instince; 
	public AudioClip musicClip;
	public GameObject pausePanel;
	public OptionsPanel optionsPanel;
	public List<Color> playerColours = new List<Color>();
	private NetworkController networkController;
	public ClothingPickupNetworkObject clothingPickupPrefab;
    public NetworkVariable<int> currentObjective = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

	// Debug Options
	[Space]
	public bool Debug_Use_Debug = false;
	public GameObject Debug_devPlayer;
	public List<AudioClip> littleGuysAudioClips = new List<AudioClip>();
	public float minAudioDelay = 4, maxAudioDelay = 10;

	private void Awake()
	{
		currentObjective.OnValueChanged += OnObjectiveChanged;
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

		StartCoroutine(AudioLittleGuys());
    }

	IEnumerator AudioLittleGuys()
	{
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(minAudioDelay, maxAudioDelay));
			AudioManager.instance.PlaySFX(littleGuysAudioClips[UnityEngine.Random.Range(0, littleGuysAudioClips.Count)]);

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
				WeekendLogger.Log("StartClient");
				networkController.StartClient();
			}
            else
            {
				Debug.Log("Editor Loading Starting Host");
				GameFlowController.SetHostQuickStart();
				WeekendLogger.Log("StartHost");
				networkController.StartHost();
			}
			return;
        }

		if(DedicatedServer.isDedicatedServer){
				networkController.StartServer();
		}else if(networkController.IsDefaultServer()){
			WeekendLogger.Log("StartHost");
			networkController.StartHost();
		}else{
			WeekendLogger.Log("StartClient");
			networkController.StartClient();
		}
	}

	private void OnObjectiveChanged(int prev, int next)
    {
        if (IsOwner)
        {
			FindObjectOfType<Compass>().UpdateTarget(ObjectiveManager.instance.UpdateObjective(next));
        }
    }

    public void SetNewObjective(){
		// Servers only!
		if(IsServer)
        	currentObjective.Value = ObjectiveManager.instance.GetNewObjective();
    }


    public override void OnNetworkSpawn()
    {
		if (IsServer)
		{
			StartCoroutine(SpawnClothesPickupCoroutine());
			SetNewObjective();
		}else{
			FindObjectOfType<Compass>().UpdateTarget(ObjectiveManager.instance.UpdateObjective(currentObjective.Value));
		}
	}

    private IEnumerator SpawnClothesPickupCoroutine()
    {
        while (IsServer)
        {
			ClothingPickupNetworkObject spawnObject = Instantiate(
				clothingPickupPrefab,
				SpawnManager.instance.GetRandomSpawn(SpawnManager.SpawnType.Clothing),
				Quaternion.identity
			);

			spawnObject.GetComponent<NetworkObject>().Spawn();
			spawnObject.clothingId.Value = ClothingManager.instance.GetRandomItem().id;

			yield return new WaitForSeconds(5);
		}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
				UnPauseGame();

			}
            else
            {
				PauseGame();

			}
        }
    }

    public void PauseGame()
    {
        //Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void UnPauseGame()
    {
        //Time.timeScale = 1;
        pausePanel.SetActive(false);
        optionsPanel.Hide(true);
    }

    public void OnResumeButtonPress()
	{
		UnPauseGame();
	}

	public void OnMainMenuButtonPress()
	{
		UnPauseGame();
		GameFlowController.LoadScene("Main Menu", false);
	}

	public void OnOptionsButtonPress()
	{
		optionsPanel.Show();
	}

	public void OnExitButtonPress()
	{
		Application.Quit();
	}

}