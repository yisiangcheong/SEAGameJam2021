using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPinAnimator : MonoBehaviour
{
    [SerializeField] BigPinController pinController = null;
    [SerializeField] Animation anim = null;

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
}
