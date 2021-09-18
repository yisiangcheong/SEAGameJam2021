using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBalls : MonoBehaviour
{
    [SerializeField] Rigidbody leftBall = null;
    [SerializeField] Rigidbody rightBall = null;
    [SerializeField] float minForce = 1f;
    [SerializeField] float maxForce = 10f;
    [SerializeField] float forceChargeRate = 1f;
    [SerializeField] float forceMultiplier = 1f;
    float currentForce = 0f;
    [SerializeField] Transform cameraFocus = null;
    // Start is called before the first frame update
    void Start()
    {
        ResetForce();
    }

    // Update is called once per frame
    void Update()
    {
        print(currentForce);
        if (Input.GetMouseButton(0))
        {
            ChargeForce();
        }
        if (Input.GetMouseButtonUp(0))
        {
            LaunchBall(leftBall, currentForce);
            ResetForce();
        }
    }

    void LaunchBall(Rigidbody ball, float force)
    {
        ball.AddForce(cameraFocus.forward * force * forceMultiplier, ForceMode.Impulse);
    }

    void ChargeForce()
    {
        currentForce += forceChargeRate * Time.deltaTime;
    }

    void ResetForce()
    {
        currentForce = minForce;
    }
}
