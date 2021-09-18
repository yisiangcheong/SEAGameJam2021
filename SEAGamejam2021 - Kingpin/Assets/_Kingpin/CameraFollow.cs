using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] bool isEnabled = true;
    [SerializeField] Transform player = null;
    [SerializeField] Camera cam = null;
    [SerializeField] Transform focusPoint = null;
    [SerializeField] Vector3 focusPointOffset = Vector3.zero;
    [SerializeField] float cameraDistance = 10f;
    [SerializeField] float xSensitivity = 1f;
    [SerializeField] float ySensitivity = 1f;
    [SerializeField] bool invertX = false;
    [SerializeField] bool invertY = false;
    [SerializeField][Range(0f,89f)] float maxYAngle = 70f;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled)
        {
            SetFocusPointPosition();
            GetMouseMovement();
            SetCameraPosition();
            SetCameraRotation();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
    }

    void GetMouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * ySensitivity * (invertX ? -1 : 1);
        float mouseY = Input.GetAxis("Mouse Y") * xSensitivity * (invertY ? -1 : 1);
        mouseY = Mathf.Clamp(mouseY, -maxYAngle, maxYAngle);
        float tempX = focusPoint.eulerAngles.x + -mouseY;
        float tempY = focusPoint.eulerAngles.y + mouseX;
        if(tempX <= 180f) tempX = Mathf.Clamp(tempX, -maxYAngle, maxYAngle);
        else tempX = Mathf.Clamp(tempX, 360 -maxYAngle, 360 + maxYAngle);//a negative x rotation in the editor is actually 180 > x > 360 internally

        focusPoint.eulerAngles = new Vector3(tempX, tempY, focusPoint.eulerAngles.z);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void SetFocusPointPosition()
    {
        focusPoint.position = player.position + focusPointOffset;
    }

    void SetCameraPosition()
    {
        float finalCameraDistance = cameraDistance;
        RaycastHit hit;
        int layermask1 = 1 << LayerMask.NameToLayer("Player");
        int layermask2 = 1 << LayerMask.NameToLayer("NonPlayerInteractable");
        int layermask3 = 1 << LayerMask.NameToLayer("Balls");
        int layermask4 = 1 << LayerMask.NameToLayer("Enemy");
        int layermask5 = 1 << LayerMask.NameToLayer("Decoration");
        int layermask6 = 1 << LayerMask.NameToLayer("Attack");
        int finalmask  = layermask1 | layermask2 | layermask3 | layermask4 | layermask5 | layermask6;
        finalmask = ~finalmask;
        //int layermask2 = LayerMask.NameToLayer("Player");
        if (Physics.Raycast(focusPoint.position, -focusPoint.forward, out hit, cameraDistance, finalmask))
        {
            Debug.DrawRay(focusPoint.position, -focusPoint.forward * cameraDistance, Color.yellow);
            finalCameraDistance = hit.distance;
        }
        else
        {
            Debug.DrawRay(focusPoint.position, -focusPoint.forward * cameraDistance, Color.red);
        }


        cam.transform.position = focusPoint.position + (focusPoint.forward * -finalCameraDistance);
    }

    void SetCameraRotation()
    {
        cam.transform.rotation = focusPoint.rotation;
    }
}
