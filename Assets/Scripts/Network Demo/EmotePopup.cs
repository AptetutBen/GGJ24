using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotePopup : MonoBehaviour
{
	[SerializeField] Image image;

	public void Initialise(Sprite sprite)
	{
		image.sprite = sprite;
		Destroy(gameObject, 2);
	}
}
