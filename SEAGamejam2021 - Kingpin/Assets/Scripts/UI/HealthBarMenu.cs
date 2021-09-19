using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBarMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> hearts = new List<GameObject>();
    
    public void InstantDeath()
    {
        for (int i = 0; i < hearts.Count; i++) hearts[i].SetActive(false);

        hearts.Clear();

        FindObjectOfType<FadeController>().StartFade(true);
    }

    public void ReduceHeart()
    {
        if (hearts.Count == 0) return;

        hearts[hearts.Count - 1].SetActive(false);
        hearts.RemoveAt(hearts.Count - 1);

        if (hearts.Count == 0)
        {
            FindObjectOfType<FadeController>().StartFade(true);
        }
    }
}
