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

	public void Initalise(string userId, string name, Color color, bool isOwner, bool showKick, Action<string> kickAction)
	{
		crown.SetActive(isOwner);
		colourImage.color = color;
		nameText.text = name;
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
