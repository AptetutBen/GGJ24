using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TouchButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	private bool interactabe = true;
	[SerializeField] private Sprite buttonPressedImage, buttonUnPressedImage;

	[SerializeField] private UnityEvent onClickAction;
	[SerializeField] private TextMeshProUGUI butonText;
	[SerializeField] private Image background;
	[SerializeField] private AudioClip clickSound;
	[SerializeField] private Shadow dropShadow;
	[SerializeField] private float shadowLengthOut = 5;
	[SerializeField] private float shadowLengthIn = 2;

	private Transform[] buttonItems;
	private Dictionary<Transform, Vector3> buttonItemPositions;
	private Vector3 itemDownPos = Vector3.down * 3;
	private bool pointerHasLeft = false;

	private void Awake ()
	{
		buttonItems = GetComponentsInChildren<Transform>(true);
		buttonItems = buttonItems.Where(item => item != transform).ToArray();
		buttonItemPositions = new();

		foreach (Transform item in buttonItems)
		{
			buttonItemPositions[item] = item.localPosition;
		}
	}

	private void SetItemPositions(bool setUp)
	{
		foreach (Transform item in buttonItems)
		{
			SetPosition(item, setUp ? Vector3.zero: itemDownPos);
		}
	}

	private void SetPosition(Transform item, Vector3 toPositoin)
	{
		item.localPosition = buttonItemPositions[item] + toPositoin;
	}

	public bool Enabled
	{
		get { return interactabe; }
		set
		{
			if (value)
			{
				interactabe = true;
			}
			else
			{
				interactabe = false;
				background.color = Color.gray;
			}
		}
	}

	public void UpdateText(string newText)
	{
		butonText.text = newText;
	}

	public void UpdateBackgroundColour(Color color)
	{
		background.color = color;
	}

	public void OnPointerClick(PointerEventData eventData)
	{

	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!interactabe)
		{
			return;
		}

		pointerHasLeft = false;
		SetButtonDown();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!interactabe)
		{
			return;
		}

		if (pointerHasLeft)
		{
			pointerHasLeft = false;
			return;
		}

		SetButtonUp();

		onClickAction?.Invoke();

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SetButtonUp();
		pointerHasLeft = true;
	}

	private void SetButtonDown()
	{
		background.sprite = buttonPressedImage;
		AudioManager.instance.PlaySFX(clickSound);

		SetItemPositions(false);

		if (dropShadow != null)
		{
			dropShadow.effectDistance = new Vector2(shadowLengthIn, -shadowLengthIn);
		}
	}

	private void SetButtonUp()
	{
		background.sprite = buttonUnPressedImage;

		SetItemPositions(true);

		if (dropShadow != null)
		{
			dropShadow.effectDistance = new Vector2(shadowLengthOut, -shadowLengthOut);
		}
	}
}
