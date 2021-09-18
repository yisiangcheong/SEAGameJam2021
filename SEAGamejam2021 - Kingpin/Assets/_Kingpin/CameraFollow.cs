﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] Camera cam = null;
    [SerializeField] Transform focusPoint = null;
    [SerializeField] Vector3 focusPointOffset = Vector3.zero;
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
        SetFocusPointPosition();
        GetMouseMovement();
        SetCameraPosition();
        SetCameraRotation();
    }

    void GetMouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * ySensitivity * (invertX ? -1 : 1);
        float mouseY = Input.GetAxis("Mouse Y") * xSensitivity * (invertY ? -1 : 1);
        float tempX = focusPoint.eulerAngles.x + -mouseY;
        float tempY = focusPoint.eulerAngles.y + -mouseX;
        if(tempX <= 90f) tempX = Mathf.Clamp(tempX, -maxYAngle, maxYAngle);
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
        cam.transform.position = focusPoint.position + (focusPoint.forward * -10);
    }

    void SetCameraRotation()
    {
        cam.transform.rotation = focusPoint.rotation;
    }
}
