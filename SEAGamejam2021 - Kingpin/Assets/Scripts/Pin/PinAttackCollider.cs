using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinAttackCollider : MonoBehaviour
{
    [SerializeField] PinController pinController = null;
    [SerializeField] float deactivationDelay = 2.5f;

    public bool isAttacking { get; private set; } = false;

    Coroutine deactivationRoutine = null;

    public void StartAttackDeactivation()
    {
        isAttacking = true;

        if (deactivationRoutine != null) StopCoroutine(deactivationRoutine);
        deactivationRoutine = StartCoroutine(DeactivationRoutine());
    }

    IEnumerator DeactivationRoutine()
    {
        float timer = 0.0f;

        while (timer < deactivationDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = true;
        pinController.Die(0.0f, Vector3.zero, false, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && pinController.pinstate != PinState.Dead)
        {
            if (collision.transform.GetComponent<Rigidbody>() != null)
            {
                StopAllCoroutines();

                Vector3 direction = collision.transform.parent.position - transform.position;
                direction = -direction.normalized;

                FindObjectOfType<HealthBarMenu>().ReduceHeart();

                AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerHurtEvent, gameObject);

                pinController.Die(collision.transform.GetComponent<Rigidbody>().velocity.magnitude * pinController.CascadeMultiplier, direction, false, true);
            }
        }
    }
}
