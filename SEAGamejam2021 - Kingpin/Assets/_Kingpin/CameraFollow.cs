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
    [SerializeField] bool startCamShake = false;
    [SerializeField] float camShakeDuration = 1f;
    [SerializeField] [Range(0, 100)] float amplitude = 1;
    [SerializeField] [Range(0.00001f, 0.99999f)] float frequency = 0.98f;
    [SerializeField] [Range(1, 4)] int octaves = 2;
    [SerializeField] [Range(0.00001f, 5)] float persistance = 0.2f;
    [SerializeField] [Range(0.00001f, 100)] float lacunarity = 20;
    [SerializeField] [Range(0.00001f, 0.99999f)] float burstFrequency = 0.5f;
    [SerializeField] [Range(0, 5)] int burstContrast = 2;
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
        if(startCamShake)
        {
            startCamShake = false;
            StartCamShake(camShakeDuration);
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

    public void StartCamShake(float duration)
    {
        StartCoroutine(CamShakeRoutine(duration));
    }

    IEnumerator CamShakeRoutine(float duration)
    {
        float timer = 0;

        

        while (timer < duration)
        {
            Vector3 tempPos = cam.transform.position;
            tempPos += (Vector3)NoiseGen.Shake2D(amplitude, frequency, octaves, persistance, lacunarity, burstFrequency, burstContrast, Time.time);
            cam.transform.position = tempPos;
            timer += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}

public static class NoiseGen
{
    public static Vector2 Shake2D(float amplitude, float frequency, int octaves, float persistance, float lacunarity, float burstFrequency, int burstContrast, float time)
    {
        float valX = 0;
        float valY = 0;

        float iAmplitude = 1;
        float iFrequency = frequency;
        float maxAmplitude = 0;

        // Burst frequency
        float burstCoord = time / (1 - burstFrequency);

        // Sample diagonally trough perlin noise
        float burstMultiplier = Mathf.PerlinNoise(burstCoord, burstCoord);

        //Apply contrast to the burst multiplier using power, it will make values stay close to zero and less often peak closer to 1
        burstMultiplier = Mathf.Pow(burstMultiplier, burstContrast);

        for (int i = 0; i < octaves; i++) // Iterate trough octaves
        {
            float noiseFrequency = time / (1 - iFrequency) / 10;

            float perlinValueX = Mathf.PerlinNoise(noiseFrequency, 0.5f);
            float perlinValueY = Mathf.PerlinNoise(0.5f, noiseFrequency);

            // Adding small value To keep the average at 0 and   *2 - 1 to keep values between -1 and 1.
            perlinValueX = (perlinValueX + 0.0352f) * 2 - 1;
            perlinValueY = (perlinValueY + 0.0345f) * 2 - 1;

            valX += perlinValueX * iAmplitude;
            valY += perlinValueY * iAmplitude;

            // Keeping track of maximum amplitude for normalizing later
            maxAmplitude += iAmplitude;

            iAmplitude *= persistance;
            iFrequency *= lacunarity;
        }

        valX *= burstMultiplier;
        valY *= burstMultiplier;

        // normalize
        valX /= maxAmplitude;
        valY /= maxAmplitude;

        valX *= amplitude;
        valY *= amplitude;

        return new Vector2(valX, valY);
    }
}