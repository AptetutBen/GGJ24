using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using noWeekend;
using TMPro;
using System.Text.RegularExpressions;

public class MainMenuController : MonoBehaviour
{
    /*
     * Scene for the main menu
     * 
     */

    public WeekendTween buttonsPanelTween;
    public NetworkConnectionPanel networkConnectionPanel;
    public AudioClip musicClip;
    public OptionsPanel optionsPanel;
    private NetworkController networkController;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.SwitchMusicClip(musicClip);
        networkController = NetworkController.instance;
        optionsPanel.Hide();
        
		AccountServerManager.instance.RegisterStateChangeCallback(OnAccountServerStateChange);
		AccountServerManager.instance.ConnectToAccountServer((wasSuccessful)=>{
			if(wasSuccessful){
				Debug.Log("Connected to account server");
			}else{
				Debug.Log("Failed to connect to account server");
			}
		});
	}

	private void OnAccountServerStateChange(AccountServerState newState){
		Debug.Log($"New account server state: {newState.ToString()}");
	}

    // When the player presses the start button
    public void OnStartButtonPress()
    {
        // Load in the Main Game scene
		GameFlowController.LoadScene("Main Game", false);
	}

    // When the player presses the exit button
    public void OnExitButtonPress()
    {
        // Game quits
        Application.Quit();
    }

    public void OnHostButtonPress()
    {
		buttonsPanelTween.Deactivate(
			() => networkConnectionPanel.Show(true)
		);
	}

    public void OnClientButtonPress()
    {
        buttonsPanelTween.Deactivate(
            () => networkConnectionPanel.Show(false)
        );
    }

    public void OnExitMultiplayerButtonPress()
    {
		networkConnectionPanel.Hide(
            () => buttonsPanelTween.Activate()
        );
    }

    // When the player presses the options button
    public void OnOptionsButtonPress()
    {
        // Show the options panel
        optionsPanel.Show();
	}


}
