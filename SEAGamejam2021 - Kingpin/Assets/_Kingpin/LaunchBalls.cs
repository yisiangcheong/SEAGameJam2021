using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBalls : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] Rigidbody playerRigidbody = null;
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
    [SerializeField] Vector3 leftBallInitialLocalPosition = Vector3.zero;
    [SerializeField] Vector3 rightBallInitialLocalPosition = Vector3.zero;
    public bool breakEarly = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetForce();//set the launcher force to min value at start

        //remember local positions of balls for ResetBallPositions
        leftBallInitialLocalPosition = leftBall.transform.localPosition;
        rightBallInitialLocalPosition = rightBall.transform.localPosition;
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
            ResetAllVelocities();//makes the player feel more in control of their movement
            ResetBallPosition();//brings the ball back to the player's side before launching it again
            if (rightHandMode)
            {
                LaunchBall(rightBall, currentForce);
                StartCoroutine(SetKinematicBoolsRoutine(0.0f));
                rightHandMode = false;//swaps hand to launch
            }
            else
            {
                LaunchBall(leftBall, currentForce);
                StartCoroutine(SetKinematicBoolsRoutine(0.0f));
                rightHandMode = true;//swaps hand to launch
            }
            ResetForce();
        }
        //SetKinematic();
        //leftBallPosition = leftBall.transform.position;//remembers current ball position to compare to next frame
        //rightBallPosition = rightBall.transform.position;//remembers current ball position to compare to next frame
    }

    private void LateUpdate()
    {
        //SetKinematic();
        //leftBallPosition = leftBall.transform.position;
        //rightBallPosition = rightBall.transform.position;
    }

    void LaunchBall(Rigidbody ball, float force)
    {
        ball.AddForce(cameraFocus.forward * force * forceMultiplier, ForceMode.Impulse);//apply final charged force to the ball, multiplied by forceMultiplier
    }

    void ChargeForce()
    {
        if (currentForce < maxForce) currentForce += forceChargeRate * Time.deltaTime;//if currentForce is less than maxForce, add force to currentForce
    }

    void ResetForce()
    {
        currentForce = minForce;//resets currentForce
    }

    void SetKinematic()
    {
        //check if the distance value of the ball from the player last frame
        // is greater than the distance of ball from player this frame (eg. it's moving back to you)
        //if true, set the ball's position back to last frame's position and set kinematic
        if (!leftBallWasKinematic)
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
        //remember velocities of the relevant rigidbodies
        Vector3 ballVelocity = ball.velocity;
        Vector3 playerVelocity = playerRigidbody.velocity;
        //set isKinematic
        ball.isKinematic = true;
        playerRigidbody.isKinematic = true;
        //initialize timer
        float timer = 0;
        while(timer < kinematicFreezeDuration && !breakEarly)//breakEarly is currently unused, will be used as a workaround for the ground pound if necessary
        {
            timer += Time.deltaTime;
            //simulates movement for the rigidbodies
            ball.MovePosition(ball.transform.position + (playerVelocity * Time.deltaTime));
            playerRigidbody.MovePosition(playerRigidbody.transform.position + (playerVelocity * Time.deltaTime));
            yield return null;
        }
        breakEarly = false;
        //set isKinematic again
        ball.isKinematic = false;   
        playerRigidbody.isKinematic = false;
        //return their original velocities, except ball velocity was too high most times so I opted to use player's velocity so they move in the same direction at the same speed
        ball.velocity = playerVelocity;
        playerRigidbody.velocity = playerVelocity;
        yield break;
    }

    void ResetAllVelocities()
    {
        //resets all velocities so the player's launch shoots them in the direction they're aiming at
        playerRigidbody.velocity = Vector3.zero;
        leftBall.velocity = Vector3.zero;
        rightBall.velocity = Vector3.zero;
    }

    IEnumerator SetKinematicBoolsRoutine(float delay)
    {
        //delays the setting of the kinematic bools so that the current position of the balls has time to save in the update loop
        yield return new WaitForSeconds(delay);
        if(rightHandMode) leftBallWasKinematic = false;
        else rightBallWasKinematic = false;
        yield break;
    }

    void ResetBallPosition()
    {
        //brings balls back to player's side
        if (rightHandMode)
        {
            print(rightHandMode);
            print(rightBall.transform.localPosition);
            //for some godforsaken reason their values invert the moment the player starts moving, so they have to load each other's positions
            rightBall.transform.localPosition = leftBallInitialLocalPosition;
            print(rightBall.transform.localPosition);
            rightBallPosition = rightBall.transform.position;
        }

        else
        {
            print(rightHandMode);
            print(leftBall.transform.localPosition);
            //for some godforsaken reason their values invert the moment the player starts moving, so they have to load each other's positions
            leftBall.transform.localPosition = rightBallInitialLocalPosition;
            print(leftBall.transform.localPosition);
            leftBallPosition = leftBall.transform.position;
        }
    }
}
