using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using noWeekend;

public class LobbyPanel : MonoBehaviour
{
	//[SerializeField] private TextMeshProUGUI addressText,addressShadowText;
	//[SerializeField] private GameObject playerListPrefab;
	//[SerializeField] private Transform playerListParent;
	private bool isApplicationQuitting = false;

	[SerializeField] private WeekendTween tween;
	[SerializeField] private Transform lobbyCodeParent;
	[SerializeField] private GameObject[] lobbyIdIcons;
	[SerializeField] private GameObject startButton;

	public void Initalise()
	{
		AccountServerManager.instance.RegisterRecieveMessageCallback(RecieveUserInfo, MessageType.UserInfo);
		AccountServerManager.instance.RegisterRecieveMessageCallback(RecieveLobbyInfo, MessageType.LobbyInfo);
	}

    public void OnDisable()
    {
		if (isApplicationQuitting) return;

		AccountServerManager.instance.UnregisterRecieveMessageCallback(RecieveUserInfo, MessageType.UserInfo);
		AccountServerManager.instance.UnregisterRecieveMessageCallback(RecieveLobbyInfo, MessageType.LobbyInfo);
	}

    public void RecieveUserInfo(AccountServerMessage accountServerMessage)
    {
        MessageUserInfo messageUserInfo = (MessageUserInfo)accountServerMessage;

        Debug.Log(messageUserInfo.userData);
	}

	public void RecieveLobbyInfo(AccountServerMessage accountServerMessage)
	{
		MessageLobbyInfo messageLobbyInfo = (MessageLobbyInfo)accountServerMessage;

		Debug.Log(messageLobbyInfo.lobbyID);

		foreach (Transform child in lobbyCodeParent)
		{
			Destroy(child.gameObject);
		}

		int lobbyID;

		if(!int.TryParse(messageLobbyInfo.lobbyID,out lobbyID))
		{
			WeekendLogger.LogLobbyError($"Can't Parse lobby code to Int: {messageLobbyInfo.lobbyID}");
		}

		// Build lobby code
		for (int i = 0; i < 4; i++)
		{
			int num = messageLobbyInfo.lobbyID[i] - '0';

			Instantiate(GetIconPrefabFromNumber(num), lobbyCodeParent);
		}
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

	void OnApplicationQuit()
	{
		isApplicationQuitting = true;
	}

	//public void AddPlayer(NetworkPlayer networkPlayer)
	//{
	//    if (!IsServer)
	//    {
	//        return;
	//    }
	//    GameObject newListItem = Instantiate(playerListPrefab, playerListParent);

	//    lobbyListLookup[networkPlayer] = newListItem;
	//}

	private GameObject GetIconPrefabFromNumber(int num)
	{
		return lobbyIdIcons[num];
	}

}
