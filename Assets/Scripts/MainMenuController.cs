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

    public WeekendTween buttonsPanelTween, networkPanelTween;

    public AudioClip musicClip;
    public OptionsPanel optionsPanel;
    private NetworkController networkController;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.SwitchMusicClip(musicClip);
        networkController = NetworkController.instance;
        optionsPanel.Hide();
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
        GameFlowController.SetHost();
    }

    public void OnMultiplayerButtonPress()
    {
        buttonsPanelTween.Deactivate(
            () => networkPanelTween.Activate()
        );
    }

    public void OnExitMultiplayerButtonPress()
    {
        networkPanelTween.Deactivate(
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
