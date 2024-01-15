using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyPanel : MonoBehaviour
{
	private List<int> inputNumbers = new();
	[SerializeField] private List<Image> outputImages = new();
	[SerializeField] private List<Sprite> icons = new();
	[SerializeField] private MainMenuController mainMenuController;

	public void PressButton(int id)
	{
		if(inputNumbers.Count >= 4)
		{
			return;
		}

		inputNumbers.Add(id);

		outputImages[inputNumbers.Count - 1].sprite = icons[id];

		if(inputNumbers.Count < 4)
		{
			return;
		}

		mainMenuController.JoinLobby(string.Join("", inputNumbers));
		Hide();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
