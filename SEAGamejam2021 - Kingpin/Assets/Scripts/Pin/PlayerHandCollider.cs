using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandCollider : MonoBehaviour
{
    [SerializeField] LaunchBalls launchBalls = null;
    [SerializeField] SpringJoint springJoint = null;
    [SerializeField] FixedJoint fixedJoint = null;
    [SerializeField] Rigidbody rigidBody = null;

    [SerializeField] float empoweredDuration = 0.5f;
    [SerializeField] float powerDampen = 2.0f;
    [SerializeField] int empoweredLimit = 20;

    Coroutine checkInputRoutine = null;
    Coroutine releasePowerRoutine = null;

    public bool IsAttacking { get { return launchBalls.isAttacking; } }

    private void OnEnable()
    {
        ResetFistPower();

        if (checkInputRoutine != null) StopCoroutine(checkInputRoutine);
        checkInputRoutine = StartCoroutine(CheckInputRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetFistPower();
    }

    void ResetFistPower()
    {
        springJoint.spring = 100.0f;

        fixedJoint.massScale = 1.0f;
        fixedJoint.connectedMassScale = 1.0f;

        rigidBody.mass = 1.0f;
    }

    void EmpowerTheFist()
    {
        float totalPower = (MultiplierMenu.currentMultiplierPower <= empoweredLimit) ? MultiplierMenu.currentMultiplierPower : empoweredLimit;
        totalPower = totalPower / powerDampen;
        if (totalPower < 1.0) totalPower = 1.0f;

        springJoint.spring = 100.0f - (totalPower * 4.0f);

        fixedJoint.massScale = totalPower;
        fixedJoint.connectedMassScale = totalPower;

        rigidBody.mass = totalPower;

        if (releasePowerRoutine != null) StopCoroutine(releasePowerRoutine);
        releasePowerRoutine = StartCoroutine(ReleasePowerRoutine());
    }

    IEnumerator CheckInputRoutine()
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                EmpowerTheFist();
            }
                
            yield return null;
        }    
    }

    IEnumerator ReleasePowerRoutine()
    {
        float timer = 0.0f;

        while (timer < empoweredDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ResetFistPower();
    }
}
