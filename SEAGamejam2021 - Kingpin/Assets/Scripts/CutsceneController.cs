using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using Ludus.Math;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Image fade = null;
    [SerializeField] VideoPlayer videoPlayer = null;

    [Header("Settings")]
    [SerializeField] float playbackDelay = 0.5f;
    [SerializeField] float fadeOutDelay = 1f;
    [SerializeField] float fadeDuration = 1.0f;
    [SerializeField] string sceneToLoad = "";

    Color transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Color fullAlpha = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    Coroutine splashScreenRoutine = null;

    private void OnEnable()
    {
        if (splashScreenRoutine != null) StopCoroutine(splashScreenRoutine);
        splashScreenRoutine = StartCoroutine(SplashscreenRoutine());
    }

    IEnumerator SplashscreenRoutine()
    {
        yield return new WaitForSeconds(playbackDelay);

        videoPlayer.Play();

        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        yield return new WaitForSeconds(fadeOutDelay);

        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            fade.color = Color.Lerp(transparent, fullAlpha, Easing.EaseInCubic(timer / fadeDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        fade.color = fullAlpha;

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}
