using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

public class EmotePopup : MonoBehaviour
{
	public void Initialise(Sprite sprite)
	{
		gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_MainTex", sprite.texture);

		float ratio = sprite.rect.width / sprite.rect.height;

		ParticleSystem ps = GetComponent<ParticleSystem>();
		var main = ps.main;

		main.startSizeX = ratio * 10;
	}
}
