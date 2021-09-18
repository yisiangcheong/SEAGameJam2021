using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandCollider : MonoBehaviour
{
    [SerializeField] LaunchBalls launchBalls = null;

    public bool IsAttacking { get { return launchBalls.isAttacking; } }
}
