using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HenchmanController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            FindObjectOfType<FadeController>().StartFade(false);
            Destroy(gameObject);
        }
    }
}
