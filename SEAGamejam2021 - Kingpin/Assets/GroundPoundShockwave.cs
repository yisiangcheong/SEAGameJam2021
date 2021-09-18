using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundShockwave : MonoBehaviour
{
    float duration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(KillSelfRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator KillSelfRoutine()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
