using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Ludus.Math;

public class FadeController : MonoBehaviour
{
    [SerializeField] string sceneToReload = "MainScene";
    [SerializeField] Image fade = null;
    [SerializeField] float fadeDuration = 3.0f;

    Color transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Color fullAlpha = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    Coroutine fadeOutRoutine = null;
    public bool isFading { get; private set; } = false;

    private void OnEnable()
    {
        if (FindObjectsOfType<FadeController>().Length >= 2)
        {
            gameObject.SetActive(false);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void StartFade()
    {
        if (isFading) return;
        isFading = true;

        if (fadeOutRoutine != null) StopCoroutine(fadeOutRoutine);
        fadeOutRoutine = StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            fade.color = Color.Lerp(transparent, fullAlpha, Easing.EaseInCubic(timer / fadeDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        fade.color = fullAlpha;

        SceneManager.LoadScene(sceneToReload, LoadSceneMode.Single);

        timer = 0.0f;

        while (timer < fadeDuration)
        {
            fade.color = Color.Lerp(fullAlpha, transparent, Easing.EaseInCubic(timer / fadeDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        fade.color = transparent;
        isFading = false;
    }
}
