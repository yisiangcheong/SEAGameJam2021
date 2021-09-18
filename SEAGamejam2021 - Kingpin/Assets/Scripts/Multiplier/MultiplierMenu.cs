﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ludus.Math;

public class MultiplierMenu : MonoBehaviour
{
    [SerializeField] Text multiplierLabel = null;

    [Header("Settings")]
    [SerializeField] float enlargeDuration = 0.1f;
    [SerializeField] Vector3 enlargedScale = new Vector3(1.35f, 1.35f, 1.35f);
    [SerializeField] float settleDuration = 0.15f;
    [SerializeField] Vector3 settleScale = new Vector3(0.9f, 0.9f, 0.9f);

    [SerializeField] float regularDuration = 0.15f;
    [SerializeField] Vector3 regularScale = new Vector3(1.05f, 1.05f, 1.05f);
    [SerializeField] float shrinkDuration = 0.15f;
    [SerializeField] Vector3 shrinkScale = new Vector3(0.95f, 0.95f, 0.95f);

    [SerializeField] float disappearPrepDuration = 0.1f;
    [SerializeField] Vector3 disappearPrepScale = new Vector3(1.15f, 1.15f, 1.15f);
    [SerializeField] float disappearDelay = 0.05f;
    [SerializeField] float disappearDuration = 0.1f;

    public bool isIdle { get; private set; } = false;
    public int totalHitCount { get; private set; } = 0;

    Coroutine actionRoutine = null;

    private void OnEnable()
    {
        //multiplierLabel.text = "";

        Invoke("ShrinkLabel", 10.0f);
    }

    public void AddScore()
    {
        totalHitCount += 1;

        multiplierLabel.text = (totalHitCount == 1) ? $"x{totalHitCount} Pin" : $"x{totalHitCount} Pins";

        if (!isIdle) StartBounceLabel(true);
        else BounceLabel(true);
    }

    public void StartBounceLabel(bool startFromZero)
    {
        if (actionRoutine != null) StopCoroutine(actionRoutine);
        actionRoutine = StartCoroutine(IntroBounceRoutine(startFromZero));
    }

    public void BounceLabel(bool startUpward = false)
    {
        if (actionRoutine != null) StopCoroutine(actionRoutine);
        actionRoutine = StartCoroutine(BounceRoutine(startUpward));
    }

    public void ShrinkLabel()
    {
        totalHitCount = 0;

        if (actionRoutine != null) StopCoroutine(actionRoutine);
        actionRoutine = StartCoroutine(ShrinkRoutine());
    }

    IEnumerator IntroBounceRoutine(bool startFromZero)
    {
        float timer = 0.0f;

        Vector3 startScale = (startFromZero) ? Vector3.zero : multiplierLabel.transform.localScale;

        while (timer < enlargeDuration)
        {
            multiplierLabel.transform.localScale = Vector3.Lerp(startScale, enlargedScale, Easing.EaseInCubic(timer / enlargeDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < settleDuration)
        {
            multiplierLabel.transform.localScale = Vector3.Lerp(enlargedScale, settleScale, Easing.EaseInCubic(timer / settleDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        isIdle = true;

        while (timer < regularDuration)
        {
            multiplierLabel.transform.localScale = Vector3.Lerp(settleScale, regularScale, Easing.EaseInCubic(timer / regularDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        BounceLabel();
    }

    IEnumerator BounceRoutine(bool startUpward = false)
    {
        bool bounceUpward = startUpward;
        float timer = 0.0f;

        while (true)
        {
            if (bounceUpward)
            {
                if (timer < regularDuration)
                {
                    timer += Time.deltaTime;
                    multiplierLabel.transform.localScale = Vector3.Lerp(shrinkScale, regularScale, Easing.EaseInCubic(timer / regularDuration));
                }
                else
                {
                    timer = 0.0f;
                    bounceUpward = false;
                }
            }
            else
            {
                if (timer < shrinkDuration)
                {
                    timer += Time.deltaTime;
                    multiplierLabel.transform.localScale = Vector3.Lerp(regularScale, shrinkScale, Easing.EaseInCubic(timer / shrinkDuration));
                }
                else
                {
                    timer = 0.0f;
                    bounceUpward = true;
                }
            }

            yield return null;
        }
    }

    IEnumerator ShrinkRoutine()
    {
        float timer = 0.0f;
        Vector3 startScale = multiplierLabel.transform.localScale;

        while (timer < disappearPrepDuration)
        {
            multiplierLabel.transform.localScale = Vector3.Lerp(startScale, disappearPrepScale, Easing.EaseInCubic(timer / disappearPrepDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < disappearDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < disappearDuration)
        {
            multiplierLabel.transform.localScale = Vector3.Lerp(disappearPrepScale, Vector3.zero, Easing.EaseInCubic(timer / disappearDuration));

            timer += Time.deltaTime;
            yield return null;
        }

        multiplierLabel.transform.localScale = Vector3.zero;
    }
}