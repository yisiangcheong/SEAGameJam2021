using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBalls : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] Transform cameraFocus = null;
    [SerializeField] Rigidbody leftBall = null;
    [SerializeField] Rigidbody rightBall = null;
    [SerializeField] float minForce = 1f;
    [SerializeField] float maxForce = 10f;
    [SerializeField] float forceChargeRate = 1f;
    [SerializeField] float forceMultiplier = 1f;
    float currentForce = 0f;
    [SerializeField] float ballMaxDistance = 5f;
    [SerializeField] bool rightHandMode = false;
    [SerializeField] float kinematicFreezeDuration = 1.0f;
    [SerializeField] bool leftBallWasKinematic = true;
    [SerializeField] bool rightBallWasKinematic = true;
    [SerializeField] Vector3 leftBallPosition = Vector3.zero;
    [SerializeField] Vector3 rightBallPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        ResetForce();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ChargeForce();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (rightHandMode)
            {
                LaunchBall(rightBall, currentForce);
                rightHandMode = false;
                rightBallWasKinematic = false;
            }
            else
            {
                LaunchBall(leftBall, currentForce);
                rightHandMode = true;
                leftBallWasKinematic = false;
            }
            ResetForce();
        }
        //SetKinematic();
        //SetBallMaxDistance();
        //leftBallPosition = leftBall.transform.position;
        //rightBallPosition = rightBall.transform.position;
    }

    void LaunchBall(Rigidbody ball, float force)
    {
        ball.AddForce(cameraFocus.forward * force * forceMultiplier, ForceMode.Impulse);
    }

    void ChargeForce()
    {
        if (currentForce < maxForce) currentForce += forceChargeRate * Time.deltaTime;
    }

    void ResetForce()
    {
        currentForce = minForce;
    }

    void SetKinematic()
    {
        if(!leftBallWasKinematic)
        {
            if (Vector3.Distance(player.position, leftBallPosition) > Vector3.Distance(player.position, leftBall.transform.position))
            {
                leftBall.transform.position = leftBallPosition;
                StartCoroutine(SetKinematicRoutine(leftBall));
                leftBallWasKinematic = true;
            }
        }
        if(!rightBallWasKinematic)
        {
            if (Vector3.Distance(player.position, rightBallPosition) > Vector3.Distance(player.position, rightBall.transform.position))
            {
                rightBall.transform.position = rightBallPosition;
                StartCoroutine(SetKinematicRoutine(rightBall));
                rightBallWasKinematic = true;
            }
        }
    }

    IEnumerator SetKinematicRoutine(Rigidbody ball)
    {
        ball.isKinematic = true;
        yield return new WaitForSeconds(kinematicFreezeDuration);
        ball.isKinematic = false;
        yield break;
    }

    void SetBallMaxDistance()
    {
        if(!rightHandMode)
        {
            if (Vector3.Distance(player.position, rightBall.transform.position) > ballMaxDistance) rightBall.transform.position = rightBallPosition;
        }
        else if (Vector3.Distance(player.position, leftBall.transform.position) > ballMaxDistance) leftBall.transform.position = leftBallPosition;
    }
}
