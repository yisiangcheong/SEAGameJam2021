using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinModelCollider : MonoBehaviour
{
    [SerializeField] float decorationDelay = 2.0f;
    [SerializeField] int decorationLayer = 10;

    Coroutine decorationRoutine = null;

    public bool IsDecoration { get; private set; } = false;

    private void OnDisable() { StopAllCoroutines(); }

    public void TurnIntoDecoration()
    {
        IsDecoration = true;
        gameObject.layer = decorationLayer;
    }

    IEnumerator DecorationRoutine()
    {
        float timer = 0.0f;

        while (timer < decorationDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        TurnIntoDecoration();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") && !IsDecoration)
        {
            if (decorationRoutine != null) StopCoroutine(decorationRoutine);
            decorationRoutine = StartCoroutine(DecorationRoutine());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") && !IsDecoration)
        {
            if (decorationRoutine != null) StopCoroutine(decorationRoutine);
        }
    }
}
