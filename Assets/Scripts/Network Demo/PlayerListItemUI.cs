using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerListItemUI : MonoBehaviour
{

	[SerializeField] GameObject crown;
	[SerializeField] TextMeshProUGUI nameText;
	[SerializeField] Image colourImage;
	[SerializeField] TouchButton kickButton;

	private bool isReady;
	private string userId;
	private Action<string> kickAction;
	public bool IsReady => isReady;

	public void Initalise(string userId, UserData userData, bool isOwner, bool showKick, Action<string> kickAction)
	{
		crown.SetActive(isOwner);

		if(!string.IsNullOrEmpty(userData.name))
		{
			if (ColorUtility.TryParseHtmlString(userData.color, out Color parsedColor))
			{
				colourImage.color = parsedColor;
			}
			else
			{
				WeekendLogger.LogLobbyError($"Can't parse colour : {userData.color}");
			}
			nameText.text = userData.name;
		}
		else
		{
			colourImage.color = Color.black;
			nameText.text = "New Player";
		}

		kickButton.gameObject.SetActive(showKick);
		this.userId = userId;
		this.kickAction = kickAction;
	}

	public void SetReady(bool isReady)
	{
		this.isReady = isReady;
	}

	public void OnKickButtonPress()
	{
		kickAction.Invoke(userId);
	}
}
