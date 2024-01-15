using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using noWeekend;

public class LobbyPanel : MonoBehaviour
{
	//[SerializeField] private TextMeshProUGUI addressText,addressShadowText;

	[SerializeField] private PlayerListItemUI playerListPrefab;
	[SerializeField] private Transform playerListParent;

	[SerializeField] private Transform emotePopupParent;
	[SerializeField] private EmotePopup emotePopupPrefab;

	private bool isApplicationQuitting = false;

	[SerializeField] private WeekendTween tween;
	[SerializeField] private Transform lobbyCodeParent;
	[SerializeField] private GameObject[] lobbyIdIcons;

	private Dictionary<string, PlayerListItemUI> playerListItemLookup = new();

	private string myID;
	private int currentLobbyId;

	public void Initalise()
	{
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveUserInfo, MessageType.UserInfo);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveLobbyInfo, MessageType.LobbyInfo);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveReadyMessage, MessageType.Ready);
		AccountServerManager.instance.RegisterRecieveMessageCallback(ReceiveChat, MessageType.Chat);
	}

    public void OnDisable()
    {
		if (isApplicationQuitting) { return; }

		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveUserInfo, MessageType.UserInfo);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveLobbyInfo, MessageType.LobbyInfo);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveReadyMessage, MessageType.Ready);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(ReceiveChat, MessageType.Chat);
	}

	public void ReceiveUserInfo(AccountServerMessage accountServerMessage)
    {
        MessageUserInfo messageUserInfo = (MessageUserInfo)accountServerMessage;

        WeekendLogger.LogLobby(messageUserInfo.userData);

		myID = messageUserInfo.userID;
	}

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

	public void ReceiveLobbyInfo(AccountServerMessage accountServerMessage)
	{
		MessageLobbyInfo messageLobbyInfo = (MessageLobbyInfo)accountServerMessage;

		// Remove old player list items
		foreach (Transform child in lobbyCodeParent)
		{
			Destroy(child.gameObject);
		}

		playerListItemLookup = new();

		foreach (MessageUserLobbyInfo user in messageLobbyInfo.users)
		{
			PlayerListItemUI newPlayer = Instantiate(playerListPrefab, playerListParent);
			playerListItemLookup[user.userID] = newPlayer;
		}

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
				// Build lobby code
				for (int i = 0; i < 4; i++)
				{
					int num = messageLobbyInfo.lobbyID[i] - '0';

					Instantiate(GetIconPrefabFromNumber(num), lobbyCodeParent);
				}
			}
		}
	}

	public void SendChat(string message)
	{
		AccountServerManager.instance.Chat(message);
	}

	public void ReceiveReadyMessage(AccountServerMessage accountServerMessage)
	{
		MessageReady messageReady = (MessageReady)accountServerMessage;
	}

	private bool AreAllPlayersReady()
	{
		return false;
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

}
