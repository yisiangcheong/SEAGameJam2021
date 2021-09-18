using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBalls : MonoBehaviour
{
    [SerializeField] Rigidbody leftBall = null;
    [SerializeField] Rigidbody rightBall = null;
    [SerializeField] float forceMultiplier = 1f;
    [SerializeField] Transform cameraFocus = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            LaunchBall(leftBall);
        }
    }

    void LaunchBall(Rigidbody ball)
    {
        ball.AddForce(cameraFocus.forward * forceMultiplier, ForceMode.Impulse);
    }
}
