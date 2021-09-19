using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    [SerializeField] CameraFollow camFollow = null;
    [SerializeField] float minVelocityForPound = 10f;
    [SerializeField] Rigidbody playerRigidbody = null;
    [SerializeField] GameObject groundPoundShockwavePrefab = null;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("collided with something");
        float currentVelocity = collision.relativeVelocity.y;
        print(collision.relativeVelocity);
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground") && currentVelocity >= minVelocityForPound)
        {
            print("Hit the ground");
            GameObject temp = Instantiate(groundPoundShockwavePrefab);
            temp.transform.position = collision.GetContact(0).point;

            AudioManager.Instance.Play2DSFX(AudioManager.Instance.ExplosionEvent);
            
            temp.transform.localScale = new Vector3(temp.transform.localScale.x * currentVelocity, temp.transform.localScale.y, temp.transform.localScale.z * currentVelocity);
            camFollow.StartCamShake(0.2f);
        }
    }
}
