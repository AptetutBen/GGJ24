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

	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		lobbyPanel.Initalise();

		AudioManager.instance.SwitchMusicClip(musicClip);
        optionsPanel.Hide();
        
		AccountServerManager.instance.RegisterStateChangeCallback(OnAccountServerStateChange);
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

		WeekendLogger.LogLobby($"New account server state: {newState}");

		if(newState == AccountServerState.Connected)
		{
			AccountServerManager.instance.UpdateUser(new UserData(playerPanel.GetName, playerPanel.GetColour));
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

		buttonsPanel.Hide();
		playerPanel.Hide();
		AttemptToConnectToAccountServer((bool wasSucessful, string message) =>
		{
			if (wasSucessful)
			{
				WeekendLogger.LogNetworkServer("Connected to account server");
				if (enterCode)
				{
					joinLobbyPanel.Show();
				}
				else
				{
					lobbyPanel.Show();
				}
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
}
