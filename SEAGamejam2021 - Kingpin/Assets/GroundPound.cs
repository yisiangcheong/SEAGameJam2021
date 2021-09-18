using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    [SerializeField] float minVelocityForPound = 10f;
    [SerializeField] Rigidbody playerRigidbody = null;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground") && playerRigidbody.velocity.magnitude >= minVelocityForPound)
        {
            print("Hit the ground");
        }
    }
}
