using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private AudioClip musicClip;

    // Panels
	[SerializeField] private OptionsPanel optionsPanel;
	[SerializeField] private NetworkConnectionPanel networkConnectionPanel;
	[SerializeField] private NetworkErrorPanel networkErrorPanel;
	[SerializeField] private ButtonsPanel buttonsPanel;
	[SerializeField] private LobbyPanel lobbyPanel;

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
		AccountServerManager.instance.ConnectToAccountServer((wasSuccessful, message) => {
			andThen?.Invoke(wasSuccessful,message);
		});
	}

    private void OnCloseNetworkErrorPannel()
    {
		buttonsPanel.Show();
	}

	private void OnAccountServerStateChange(AccountServerState newState){

		WeekendLogger.LogNetworkServer($"New account server state: {newState}");
	}

 //   // When the player presses the start button
 //   public void OnStartButtonPress()
 //   {
 //       // Load in the Main Game scene
	//	GameFlowController.LoadScene("Main Game", false);
	//}

    // When the player presses the exit button
    public void OnExitButtonPress()
    {
        // Game quits
        Application.Quit();
    }

    // Player Starts a game
    public void OnStartGameButtonPress()
    {
		buttonsPanel.Hide();
		AttemptToConnectToAccountServer((bool wasSucessful, string message) =>
		{
			if (wasSucessful)
			{
				WeekendLogger.LogNetworkServer("Connected to account server");
				lobbyPanel.Show();
			}
			else
			{
				WeekendLogger.LogNetworkServerError("Failed to connect to account server");
				networkErrorPanel.Show("Failed to connect to account server", OnCloseNetworkErrorPannel);
			}
		});
	}

    public void OnJoinFriendsButtonPress()
    {

    }

    public void OnExitMultiplayerButtonPress()
    {
		networkConnectionPanel.Hide(
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
