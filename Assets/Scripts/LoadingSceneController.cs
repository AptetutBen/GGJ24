using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    // Const
    private const float FADE_DURATION = 1; // The duration of fadding the screen in and out

	// References
	public CanvasGroup fadePanel;   // The main panel that fades in and out
    public Image loadingBarFill;    // The loading bar for the scene
	public CanvasGroup loadingGraphics; // A group that consists of the LOADING text and loading bar


    // On Awake
	private void Awake()
	{
        StartCoroutine(loadLevelCorourine());
        loadingGraphics.alpha = 0;

		// Main Level Loading coroutine.
		IEnumerator loadLevelCorourine()
		{

			// Fade the fade panel in
			yield return StartCoroutine(FadePanel(true));

			// Unload the old scene
			SceneManager.UnloadSceneAsync(GameFlowController.previousScene);

			//Load in the new scene.
			AsyncOperation loadLevel = SceneManager.LoadSceneAsync(GameFlowController.desiredSceneToLoad, LoadSceneMode.Additive);

            // Show the loading graphics if requested
            loadingGraphics.alpha = GameFlowController.loadUsingLoadingBar ? 1 : 0;

			// Wait until the level is loaded
			while (!loadLevel.isDone)
			{
				// If requested fill up the loading bar
				if (GameFlowController.loadUsingLoadingBar)
				{
					loadingBarFill.fillAmount = Mathf.Clamp01(loadLevel.progress / 0.9f);
				}
				yield return null;
			}


			SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameFlowController.desiredSceneToLoad));

			// Hide the loading graphics
			loadingGraphics.alpha = 0;

			//Fade in the fade panel;
			yield return StartCoroutine(FadePanel(false));

			// Unload this scene
			SceneManager.UnloadSceneAsync("Loading Scene");
		}
	}


	// Fade the fade panel in/out
    private IEnumerator FadePanel(bool fadeIn)
    {
        float startValue = fadePanel.alpha;
        float endValue = fadeIn ? 1 : 0;

        float duration = Mathf.Abs(startValue - endValue) * FADE_DURATION;
		float timer = 0;

        while (timer < 1)
        {
            yield return null;
            timer += Time.deltaTime / duration;

            fadePanel.alpha = Mathf.Lerp(startValue, endValue, timer);
        }

        fadePanel.alpha = endValue;
	}
}
