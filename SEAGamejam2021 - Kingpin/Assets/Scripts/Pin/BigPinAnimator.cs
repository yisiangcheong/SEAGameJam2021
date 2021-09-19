using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPinAnimator : MonoBehaviour
{
    [SerializeField] BigPinController pinController = null;
    [SerializeField] Animation anim = null;
    [SerializeField] float shakeDuration = 0.2f;

    CameraFollow camFollow = null;

    public void CheckIsAllowedToLoop()
    {
        if (!pinController.isWithinHuntingRange)
        {
            anim.Stop();
        }
    }

    public void SetMoveable()
    {
        pinController.isMovementAllowed = true;
    }

    public void SetImmovable()
    {
        pinController.isMovementAllowed = false;
    }

    public void ShakeCamera()
    {
        if (camFollow == null) camFollow = FindObjectOfType<CameraFollow>();
        if (camFollow != null) camFollow.StartCamShake(shakeDuration);
    }
}
