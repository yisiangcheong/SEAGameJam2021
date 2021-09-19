using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInstantKillCollider : MonoBehaviour
{
    [SerializeField] BigPinController bigPinController = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && bigPinController.pinstate != PinState.Dead)
        {
            if (other.transform.GetComponent<Rigidbody>() != null)
            {
                FindObjectOfType<HealthBarMenu>().InstantDeath();

                AudioManager.Instance.PlaySFX(AudioManager.Instance.PlayerHurtEvent, gameObject);
            }
        }
    }
}
