using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController instince; 
	public AudioClip musicClip;
	public GameObject pausePanel;
	public OptionsPanel optionsPanel;
	public List<Color> playerColours = new List<Color>();
	private NetworkController networkController;

	// Debug Options
	[Space]
	public bool Debug_Use_Debug = false;


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
		pausePanel.SetActive(false);
		networkController = NetworkController.instance;

		Invoke("ConnectNetwork", 1);
    }

	private void ConnectNetwork()
    {
		// For testing in editor 
        if (!GameFlowController.loadedOtherScene)
        {
            if (!networkController.IsDefaultServer())
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
		Time.timeScale = 0;
		pausePanel.SetActive(true);
	}

	public void UnPauseGame()
	{
		Time.timeScale = 1;
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