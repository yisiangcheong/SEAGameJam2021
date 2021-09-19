using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HenchmanController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            AudioManager.Instance.Play2DSFX(AudioManager.Instance.SussyBakaEvent);
            FindObjectOfType<FadeController>().StartFade(false);
            Destroy(gameObject);
        }
    }
}
