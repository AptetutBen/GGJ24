using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class MainMenuController : MonoBehaviour
{
	public static MainMenuController instance;
	[SerializeField] private AudioClip musicClip;

    // Panels
	[SerializeField] private OptionsPanel optionsPanel;
	[SerializeField] private PlayerPanel playerPanel;
	[SerializeField] private NetworkErrorPanel networkErrorPanel;
	[SerializeField] private ButtonsPanel buttonsPanel;
	[SerializeField] private LobbyPanel lobbyPanel;
	[SerializeField] private JoinLobbyPanel joinLobbyPanel;
	[SerializeField] private GameObject creditsPanel;
	private bool enterCode;
	
	public List<AudioClip> littleGuysAudioClips = new List<AudioClip>();
	public float minAudioDelay = 4, maxAudioDelay = 10;
	public UserData UserData
	{
		get
		{
			return new UserData(playerPanel.GetName, playerPanel.GetColour);
		}
	}

	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		lobbyPanel.Initalise();
		creditsPanel.SetActive(false);

		AudioManager.instance.SwitchMusicClip(musicClip);
        optionsPanel.Hide();
        
		AccountServerManager.instance.RegisterStateChangeCallback(OnAccountServerStateChange);

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

	private void AttemptToConnectToAccountServer(Action<bool,string> andThen)
    {
		if (AccountServerManager.instance.IsConnected)
		{
			andThen?.Invoke(true, "");
			return;
		}

		AccountServerManager.instance.ConnectToAccountServer((wasSuccessful, message) => {
			andThen?.Invoke(wasSuccessful,message);
		});
	}

    private void OnCloseNetworkErrorPannel()
    {
		buttonsPanel.Show();
	}

	private void OnAccountServerStateChange(AccountServerState newState){


		if (newState == AccountServerState.Connected)
		{
			if (enterCode)
			{
				joinLobbyPanel.Show();
			}
			else
			{
				lobbyPanel.Show();
			}
			AccountServerManager.instance.StartSession(UserData);
		}
	}


	// When the player presses the exit button
	public void OnExitButtonPress()
    {
        // Game quits
        Application.Quit();
    }

    // Player Starts a game
    public void OnStartGameButtonPress(bool enterCode)
    {
		this.enterCode = enterCode;
		buttonsPanel.Hide();
		playerPanel.Hide();

		GameFlowController.playerName = playerPanel.playerNameInputField.text;
        GameFlowController.playerColor = playerPanel.colourPicker.color;

		AttemptToConnectToAccountServer((bool wasSucessful, string message) =>
		{
			if (wasSucessful)
			{
			}
			else
			{
				WeekendLogger.LogNetworkServerError("Failed to connect to account server");
				networkErrorPanel.Show("Failed to connect to account server", OnCloseNetworkErrorPannel);
			}
		});
	}


	public void ReturnToStart()
	{
		buttonsPanel.Show();
		playerPanel.Show();
	}

	public void JoinLobby(string lobbyCode)
    {
		AccountServerManager.instance.JoinLobby(lobbyCode);
		lobbyPanel.Show();
	}

    public void OnExitMultiplayerButtonPress()
    {
		playerPanel.Hide(
            () => buttonsPanel.Show()
        );
    }

    // When the player presses the options button
    public void OnOptionsButtonPress()
    {
        // Show the options panel
        optionsPanel.Show();
	}

	public void OnCreditsButtonPress()
    {
		creditsPanel.SetActive(true);
	}

	public void OnCloseCreditsButtonPress()
    {
		creditsPanel.SetActive(false);
	}
}
