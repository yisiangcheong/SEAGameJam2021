using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb = null;
    [SerializeField] float movementMultiplier = 1f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 finalMovement = new Vector3(Input.GetAxis("Horizontal") * movementMultiplier, 0, Input.GetAxis("Vertical") * movementMultiplier);
        rb.AddForce(finalMovement * Time.deltaTime);
        
    }
}
