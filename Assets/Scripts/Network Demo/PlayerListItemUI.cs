using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using noWeekend;

public class PlayerListItemUI : MonoBehaviour
{

	[SerializeField] GameObject crown;
	[SerializeField] TextMeshProUGUI nameText;
	[SerializeField] Image colourImage;
	[SerializeField] TouchButton kickButton;
	[SerializeField] WeekendTween tickImage;

	private string userId;
	private Action<string> kickAction;

	public void Initalise(string userId, UserData userData, bool isOwner, bool showKick, Action<string> kickAction, bool isReady)
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

		if (isReady)
		{
			tickImage.SetActivated();
		}
	}

	public void SetReady(bool isReady)
	{
		if (isReady)
		{
			if (!tickImage.IsActive)
			{
				tickImage.Activate();
			}

		}
		else
		{
			if (tickImage.IsActive)
			{
				tickImage.Deactivate();
			}
		}
	}

	public void OnKickButtonPress()
	{
		kickAction.Invoke(userId);
	}
}
