using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteButton : MonoBehaviour
{
    [SerializeField] private Image image;
	private Action<string> onClickAction;
	private string id;

	public void Initalise(Sprite sprite, string id, Action<string> onClickAction)
	{
		image.sprite = sprite;
		this.id = id;
		this.onClickAction = onClickAction;
	}

	public void OnClick()
	{
		onClickAction(id);
	}
}
