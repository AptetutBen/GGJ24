using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker instance;

    private Coroutine shakeCoroutine;
	private Transform cameraChild;

	private void Awake()
	{
		cameraChild = transform.GetChild(0);
		instance = this;
	}

	public void SmallShake()
	{
		ShakeCamera(0.2f, 0.2f);
	}

	public void MediumShake()
	{
		ShakeCamera(0.5f, 0.5f);
	}

	public void LargeShake()
	{
		ShakeCamera(0.7f, 0.7f);
	}

	// Simple camera shake,

	public void ShakeCamera(float duration, float strength)
    {
		if(shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
			shakeCoroutine = null;
		}

		shakeCoroutine = StartCoroutine(shakeCameraCoroutine());
		
		IEnumerator shakeCameraCoroutine()
		{
			float timer = 1;

			while (timer > 0)
			{
				cameraChild.transform.localPosition = Random.insideUnitSphere * strength * timer;
				timer -= Time.deltaTime / duration;
				yield return null;
			}

			cameraChild.transform.localPosition = Vector3.zero;
		}
    }
}
