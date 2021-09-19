using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinAnimator : MonoBehaviour
{
    [SerializeField] PinController pinController = null;
    [SerializeField] Animation anim = null;
    
    public void CheckIsAllowedToLoop()
    {
        if (pinController.pinstate == PinState.Attack)
        {
            anim.Stop();
            anim.enabled = false;
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
