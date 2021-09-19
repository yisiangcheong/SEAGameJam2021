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
    [SerializeField] RawImage videoDisplay = null;
    [SerializeField] VideoPlayer videoPlayer = null;
    [SerializeField] Text skipLabel = null;

    [Header("Settings")]
    [SerializeField] float playbackDelay = 0.5f;
    [SerializeField] float fadeOutDelay = 1f;
    [SerializeField] float fadeDuration = 1.0f;
    [SerializeField] string sceneToLoad = "";
    [SerializeField] float skipConfirmation = 2.5f;

    Color transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Color fullAlpha = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    Color fullWhite = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    Coroutine checkSkipCutsceneRoutine = null;
    Coroutine splashScreenRoutine = null;

    public bool isTransitioning { get; private set; } = false;

    private void OnEnable()
    {
        skipLabel.color = transparent;

        if (checkSkipCutsceneRoutine != null) StopCoroutine(checkSkipCutsceneRoutine);
        checkSkipCutsceneRoutine = StartCoroutine(CheckSkipCutsceneRoutine());

        if (splashScreenRoutine != null) StopCoroutine(splashScreenRoutine);
        splashScreenRoutine = StartCoroutine(SplashscreenRoutine());
    }

    void TransitionToNextScene()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        StopAllCoroutines();
        StartCoroutine(TransitionToNextSceneRoutine());
    }

    IEnumerator CheckSkipCutsceneRoutine()
    {
        float timer = 0.0f;

        while (true)
        {
            if (timer <= 0.0f)
            {
                skipLabel.color = transparent;

                if (Input.anyKeyDown)
                {
                    timer = skipConfirmation;
                    skipLabel.color = fullWhite;
                }
            }
            else
            {
                timer -= Time.deltaTime;

                if (Input.anyKeyDown) break;
            }

            yield return null;
        }

        TransitionToNextScene();
    }

    IEnumerator SplashscreenRoutine()
    {
        yield return new WaitForSeconds(playbackDelay);

        videoDisplay.color = fullWhite;

        AudioManager.Instance.PlaySFX("event:/SFX/IntroTransatlantic", gameObject);
        videoPlayer.Play();

        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        yield return new WaitForSeconds(fadeOutDelay);

        TransitionToNextScene();
    }

    IEnumerator TransitionToNextSceneRoutine()
    {
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
