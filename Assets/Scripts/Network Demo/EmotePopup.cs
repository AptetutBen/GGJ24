using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

public class EmotePopup : MonoBehaviour
{
	[SerializeField] private UIParticle particles;

	public void Initialise(Sprite sprite)
	{
		particles.material.SetColor("_Color", Color.red);

		//particles.material.SetTexture("_MainTex",sprite.texture);
		Destroy(gameObject, 2);
	}
}
