using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameFlowController 
{
	/*
	This file manages the game flowing from one part of it to another.
	Primaraly it just handles loading and unloading of scenes.
	But can be expanded to store the last scene the player was is in last time they were playing etc.
	*/

	public enum GameMode { Client, Host, Server, Solo}

	public static GameMode gameMode = GameMode.Solo;
	public static string ipAddress;
	public static ushort host;
	public static string playerName = "Test Player";
	public static Color playerColor = Color.blue;

	public static string desiredSceneToLoad;	// The next scene requested to load
	public static string previousScene;	// The current scene that is loaded
	public static bool loadUsingLoadingBar; // Sets if the loading screen should use a loading bar (for long loads)
	public static bool loadedOtherScene;

	// When called will switch scenes using the loading screen
	public static void LoadScene(string sceneToLoad,bool useLoadingBar = false)
	{
		// Gets the current scene name for later use
		previousScene = SceneManager.GetActiveScene().name;

		// Sets the desired scene name to load for later use
		desiredSceneToLoad = sceneToLoad;

		// Sets if the loading screen should use a loading bar or not
		loadUsingLoadingBar = useLoadingBar;

		// Loads the loading scene which takes over the rest of the process
		SceneManager.LoadSceneAsync("Loading Scene",LoadSceneMode.Additive);

		loadedOtherScene = true;

	}

	public static void SetHost()
    {
		gameMode = GameMode.Host;
		LoadScene("Main Game");
	}

	public static void SetClient(string address, ushort hostNumber,string name)
    {
		gameMode = GameMode.Client;
		ipAddress = address;
		playerName = name;
		host = hostNumber;
		LoadScene("Main Game");
	}
}
