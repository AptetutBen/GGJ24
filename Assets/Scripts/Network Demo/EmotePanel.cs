using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using noWeekend;
using TMPro;


public class EmotePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tagText;
	[SerializeField] private WeekendTween tween;
    [SerializeField] private LobbyPanel lobbyPanel;
	[SerializeField] private Transform emoteParent;
	[SerializeField] private EmoteButton buttonPrefab;

	public void OnTagClick()
    {
        if (tween.IsActive)
        {
			Hide();
			tagText.text = ":)";
		}
        else
        {
            Show();
			tagText.text = "<:";
		}
    }

    public void Show()
    {
        tween.Activate();
	}

    public void Hide()
    {
        tween.Deactivate();
    }

    public void OnEmoteSelect(string emoteID)
    {
        lobbyPanel.SendChat(emoteID);
	}

	public void Awake()
	{
        Sprite[] emoteSprites = EmoteManager.GetEmoteSprites();

        foreach (Sprite sprite in emoteSprites)
        {
            EmoteButton emoteButton = Instantiate(buttonPrefab, emoteParent);
            emoteButton.Initalise(sprite, sprite.name, OnEmoteSelect);
        }
	}
}
