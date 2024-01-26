using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using noWeekend;
using System.Linq;

public class LobbyPanel : MonoBehaviour
{
	//[SerializeField] private TextMeshProUGUI addressText,addressShadowText;
	private readonly int Min_Connected_Players = 1;

	[SerializeField] private PlayerListItemUI playerListPrefab;
	[SerializeField] private Transform playerListParent;

	[SerializeField] private Transform emotePopupParent;
	[SerializeField] private EmotePopup emotePopupPrefab;

	private bool isApplicationQuitting = false;

	[SerializeField] private WeekendTween tween;
	[SerializeField] private Transform lobbyCodeParent;
	[SerializeField] private GameObject[] lobbyIdIcons;
	[SerializeField] private TouchButton startGameButton;

	private Dictionary<string, PlayerListItemUI> playerListItemLookup = new();
	private bool AllPlayersReady;
	private bool isReady;

	private string myID;
	private int currentLobbyId;

	private bool isLobbyOwner;

	public void Initalise()
	{
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveUserInfo, MessageType.UserInfo);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveLobbyInfo, MessageType.LobbyInfo);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveReadyMessage, MessageType.Ready);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveChat, MessageType.Chat);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveStartGame, MessageType.StartGame);
		//AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveStartSession, MessageType.StartSession);
	}

    public void OnDisable()
    {
		if (isApplicationQuitting) { return; }

		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveUserInfo, MessageType.UserInfo);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveLobbyInfo, MessageType.LobbyInfo);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveReadyMessage, MessageType.Ready);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveChat, MessageType.Chat);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveStartGame, MessageType.StartGame);
		//AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveStartSession, MessageType.StartSession);
	}

	//private void ReceiveStartSession(AccountServerMessage accountServerMessage)
	//{
	//	AccountServerManager.instance.StartSession(MainMenuController.instance.UserData);
	//}


	// Receive User Info Message
	public void ReceiveUserInfo(AccountServerMessage accountServerMessage)
    {
        MessageUserInfo messageUserInfo = (MessageUserInfo)accountServerMessage;

		myID = messageUserInfo.userID;
	}

	// Receive Start Game
	public void ReceiveStartGame(AccountServerMessage accountServerMessage)
    {
        MessageStartGame messageUserInfo = (MessageStartGame)accountServerMessage;
	
		GameFlowController.SetClient(messageUserInfo.ip, (ushort) messageUserInfo.port);
	}

	// Receive Chat Message
	public void ReceiveChat(AccountServerMessage accountServerMessage)
	{
		MessageChat messageChat = (MessageChat)accountServerMessage;

		//WeekendLogger.LogLobby(messageChat.chatMessage);

		if (!playerListItemLookup.ContainsKey(messageChat.userID))
		{
			WeekendLogger.LogLobbyError($"Player not found: {messageChat.userID}");
			return;
		}

		EmotePopup newEmotePopup = Instantiate(emotePopupPrefab, emotePopupParent);

		newEmotePopup.Initialise(EmoteManager.GetEmote(messageChat.chatMessage));

		newEmotePopup.transform.position = playerListItemLookup[messageChat.userID].transform.position;
	}


	// Receive Lobby Info Message
	public void ReceiveLobbyInfo(AccountServerMessage accountServerMessage)
	{
		MessageLobbyInfo messageLobbyInfo = (MessageLobbyInfo)accountServerMessage;

		foreach (Transform child in playerListParent)
		{
			Destroy(child.gameObject);
		}

		// Check if this player is the lobby owner 
		isLobbyOwner = messageLobbyInfo.users[0].userID == myID;

		// Clear the player lookup table
		playerListItemLookup = new();

		// Instantiate the players in the list
		for (int i = 0; i < messageLobbyInfo.users.Length; i++)
		{
			MessageUserLobbyInfo user = messageLobbyInfo.users[i];
			PlayerListItemUI newPlayer = Instantiate(playerListPrefab, playerListParent);
			playerListItemLookup[user.userID] = newPlayer;
			newPlayer.Initalise(user.userID, user.userData, i == 0, isLobbyOwner && user.userID != myID, KickPlayer,user.ready);
		}

		// Set the state of the start game button
		SetStartGameButtonState();

		// Build the lobby id code
		int lobbyID;

		if(!int.TryParse(messageLobbyInfo.lobbyID,out lobbyID))
		{
			WeekendLogger.LogLobbyError($"Can't Parse lobby code to Int: {messageLobbyInfo.lobbyID}");
		}
		else
		{
			if(currentLobbyId != lobbyID)
			{
				// Remove old player list items
				foreach (Transform child in lobbyCodeParent)
				{
					Destroy(child.gameObject);
				}

				// Build lobby code
				for (int i = 0; i < 4; i++)
				{
					int num = messageLobbyInfo.lobbyID[i] - '0';

					Instantiate(GetIconPrefabFromNumber(num), lobbyCodeParent);
				}

				currentLobbyId = lobbyID;
			}
		}
	}

	// Send a Chat Message
	public void SendChat(string message)
	{
		AccountServerManager.instance.Chat(message);
	}

	// Set a Ready Message
	public void ReceiveReadyMessage(AccountServerMessage accountServerMessage)
	{
		MessageReady messageReady = (MessageReady)accountServerMessage;

		// Loop through each ready message and set 
		foreach (MessageUserLobbyReady user in messageReady.users)
		{
			if (!playerListItemLookup.ContainsKey(user.userID)){
				WeekendLogger.LogLobbyError($"User not found: {user.userID}");
				continue;
			}
			playerListItemLookup[user.userID].SetReady(user.ready);

			if(user.userID == myID)
			{
				isReady = user.ready;
			}
		}

		AllPlayersReady = messageReady.users.All(user => user.ready == true);

		SetStartGameButtonState();
	}

	// Check if the start game button should be shown and if it is enabled/disabled

	private void SetStartGameButtonState()
	{
		if (!isLobbyOwner)
		{
			// Hide the connect button
			startGameButton.gameObject.SetActive(false);
			return;
		}

		startGameButton.gameObject.SetActive(true);

		if (playerListItemLookup.Keys.Count >= Min_Connected_Players && AllPlayersReady)
		{
			// Enable the start game button
			startGameButton.Enabled = true;
		}
		else
		{
			//Disable the start game button
			startGameButton.Enabled = false;
		}
	}

	private void KickPlayer(string userId)
	{
		AccountServerManager.instance.KickPlayer(userId);
	}

	public void Show()
	{
		gameObject.SetActive(true);
		tween.Activate();
	}

	public void Hide()
	{
		tween.Deactivate();
	}

	// Mark the appllication is Quitting to stop random errors
	void OnApplicationQuit()
	{
		isApplicationQuitting = true;
	}

	// Get an icon from a given id
	private GameObject GetIconPrefabFromNumber(int num)
	{
		return lobbyIdIcons[num];
	}

	public void OnReadyButtonPress()
	{
		AccountServerManager.instance.Ready(!isReady);
	}

	public void OnStartGameButtonPress()
	{
		if (!isLobbyOwner)
		{
			return;
		}

		AccountServerManager.instance.StartGame(GameFlowController.GameMode.MMO);
	}

	public void OnLeaveButtonPress()
	{
		AccountServerManager.instance.LeaveLobby();

		MainMenuController.instance.ReturnToStart();
		Hide();
	}
}
