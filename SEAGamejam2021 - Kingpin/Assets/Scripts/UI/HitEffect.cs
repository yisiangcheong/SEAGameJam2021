using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ludus.Math;

public class HitEffect : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite[] hitSprites = null;

    [Header("Settings")]
    [SerializeField] float enlargeDuration = 0.1f;
    [SerializeField] Vector3 enlargeScale = new Vector3(0.4f, 0.4f, 0.4f);
    [SerializeField] float settleDuration = 0.3f;
    [SerializeField] Vector3 settleScale = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] float disappearPrepDuration = 0.1f;
    [SerializeField] Vector3 disappearPrepScale = new Vector3(0.36f, 0.36f, 0.36f);
    [SerializeField] float disappearDelay = 0.1f;
    [SerializeField] float disappearDuration = 0.1f;

    [SerializeField] float appearDrift = 1.5f;
    [SerializeField] float displayDrift = 0.3f;
    [SerializeField] float disappearDrift = 1.0f;

    Coroutine faceCameraRoutine = null;
    Coroutine displayRoutine = null;

    public void Initialize(Vector3 pos)
    {
        transform.position = pos;

        spriteRenderer.sprite = hitSprites[Random.Range(0, hitSprites.Length)];

        if (faceCameraRoutine != null) StopCoroutine(faceCameraRoutine);
        faceCameraRoutine = StartCoroutine(FaceCameraRoutine());

        if (displayRoutine != null) StopCoroutine(displayRoutine);
        displayRoutine = StartCoroutine(DisplayRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.zero;
        spriteRenderer.sprite = null;

        gameObject.SetActive(false);
    }

    IEnumerator FaceCameraRoutine()
    {
        while (true)
        {
            transform.LookAt(Camera.main.transform);
            yield return null;
        }
    }

    IEnumerator DisplayRoutine()
    {
        float timer = 0.0f;

        while (timer < enlargeDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, enlargeScale, Easing.EaseInCubic(timer / enlargeDuration));
            transform.position = new Vector3(transform.position.x, transform.position.y + appearDrift * Time.deltaTime, transform.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < settleDuration)
        {
            transform.localScale = Vector3.Lerp(enlargeScale, settleScale, Easing.EaseInCubic(timer / settleDuration));
            transform.position = new Vector3(transform.position.x, transform.position.y + displayDrift * Time.deltaTime, transform.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < disappearPrepDuration)
        {
            transform.localScale = Vector3.Lerp(settleScale, disappearPrepScale, Easing.EaseInCubic(timer / disappearPrepDuration));
            transform.position = new Vector3(transform.position.x, transform.position.y + displayDrift * Time.deltaTime, transform.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < disappearDelay)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + displayDrift * Time.deltaTime, transform.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < disappearDuration)
        {
            transform.localScale = Vector3.Lerp(disappearPrepScale, Vector3.zero, Easing.EaseInCubic(timer / disappearDuration));
            transform.position = new Vector3(transform.position.x, transform.position.y + disappearDrift * Time.deltaTime, transform.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        Deactivate();
    }
}
