using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioLogo : MonoBehaviour
{
	public float loadSceneDelay = 3;
	public AudioClip soundClip;
	private bool sceneLoading;

    public void PlaySound()
	{
		AudioManager.instance.PlaySFX(soundClip);
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(loadSceneDelay);
		if (sceneLoading)
		{
			yield break;
		}
		GameFlowController.LoadScene("Main Menu");
	}

	private void Update()
	{
		if(sceneLoading)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
		{
			sceneLoading = true;
			GameFlowController.LoadScene("Main Menu");
		}
	}
}
