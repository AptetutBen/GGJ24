using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameFlowController 
{
	/*
	This file manages the game flowing from one part of it to another.
	Primaraly it just handles loading and unloading of scenes.
	But can be expanded to store the last scene the player was is in last time they were playing etc.
	*/

	public enum GameMode { 
		None = -1,
		MMO = 1, // lol
		Group = 2 // lol
	}

	public static GameMode gameMode = GameMode.None;
	public static bool shouldJoinRemoteServer = false;
	public static string ipAddress = "127.0.0.1";
	public static ushort host = 7777;
	public static string playerName = "Not Set Up";
	public static Color playerColor = Color.red;

	public static string desiredSceneToLoad;	// The next scene requested to load
	public static string previousScene;	// The current scene that is loaded
	public static bool loadUsingLoadingBar; // Sets if the loading screen should use a loading bar (for long loads)
	public static bool loadedOtherScene;

	// When called will switch scenes using the loading screen
	public static void LoadScene(string sceneToLoad,bool useLoadingBar = false)
	{
		loadedOtherScene = true;

		// Gets the current scene name for later use
		previousScene = SceneManager.GetActiveScene().name;

		// Sets the desired scene name to load for later use
		desiredSceneToLoad = sceneToLoad;

		// Sets if the loading screen should use a loading bar or not
		loadUsingLoadingBar = useLoadingBar;

		// Loads the loading scene which takes over the rest of the process
		SceneManager.LoadSceneAsync("Loading Scene",LoadSceneMode.Additive);

	}

	public static void ChangeGameMode(GameMode newMode){
        gameMode = newMode;
        SceneManager.LoadScene("Main Game");
    }

	public static void SetHostQuickStart()
	{
		playerName = Tools.GenerateRandomName();
		playerColor = Tools.RandomColour();
	}

	public static void SetHost(string name, Color color, bool startNetwork = true)
    {
		playerName = name;
		playerColor = color;

		if (startNetwork)
		{
			LoadScene("Main Game");
		}
	}

	public static void SetClientQuickStart()
	{
		ipAddress = "127.0.0.1";
		playerName = Tools.GenerateRandomName();
		host = 7777;
		playerColor = Tools.RandomColour();
	}

	public static void SetClient(string address, ushort hostNumber)
    {
		ipAddress = address;
		host = hostNumber;
		shouldJoinRemoteServer = true;
		LoadScene("Main Game");
	}
	
	public static void SetClient(string address, ushort hostNumber,string name,Color color)
    {
		ipAddress = address;
		playerName = name;
		host = hostNumber;
		playerColor = color;
		shouldJoinRemoteServer = true;
		LoadScene("Main Game");
	}
}
