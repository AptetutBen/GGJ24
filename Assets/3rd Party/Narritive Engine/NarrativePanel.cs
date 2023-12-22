using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using noWeekend;

public class NarrativePanel : MonoBehaviour
{
	[Tooltip("Characters per Second")]
	public float speed = 1;
	public float waitingBlinkSpeed = 0.5f;
	public TextMeshProUGUI outputText;
	public WeekendTween weekendTween;
	public GameObject waitingImage;
	public AudioClip letterClick;

	[TextArea]
	public string[] conversation;

	private bool hasClicked;

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			hasClicked = true;
		}
	}

	private void Awake()
	{
		outputText.text = "";
		
		waitingImage.SetActive(false);
	}

	public void Start()
	{
		StartCoroutine(DisplayText(conversation));
	}

	IEnumerator DisplayText(string[] text)
	{
		if (!weekendTween.IsActive)
		{
			yield return StartCoroutine(weekendTween.ActivateCoroutine());
		}

		for (int textCounter = 0; textCounter < text.Length; textCounter++)
		{
			waitingImage.SetActive(false);
			hasClicked = false;
			outputText.maxVisibleCharacters = 0;
			outputText.text = text[textCounter];

			float timeBetweenChars = 1 / speed;

			for (int i = 0; i < text[textCounter].Length; i++)
			{
				if (hasClicked)
				{
					hasClicked = false;
					outputText.maxVisibleCharacters = text[textCounter].Length;
					break;
				}
				outputText.maxVisibleCharacters = i + 1;

				AudioManager.instance.PlaySFX(letterClick);

				if (text[textCounter][i] != ' ')
				{
					yield return new WaitForSeconds(timeBetweenChars);
				}
			}

			while (hasClicked == false)
			{
				yield return new WaitForSeconds(waitingBlinkSpeed);
				waitingImage.SetActive(!waitingImage.activeSelf);
			}
		}

		waitingImage.SetActive(false);
		outputText.text = "";

		weekendTween.Deactivate();
	}
}

