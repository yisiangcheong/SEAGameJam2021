using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinSpawnManager : MonoBehaviour
{
    [SerializeField] GameObject pinPrefab = null;
    [SerializeField] int maxTotalPins = 10;
    [SerializeField] Transform[] pinSpawnPoints = null;
    [SerializeField] Vector2[] spawnPattern = null;
    [SerializeField] float spawnInterval = 2.0f;
    [SerializeField] float minSpawningRadius = 50.0f;
    [SerializeField] float maxSpawningRadius = 100.0f;

    [SerializeField] Vector3 spawnPointRadius = new Vector3(5.0f, 0.0f, 5.0f);

    Coroutine spawnRoutine = null;

    [Header("Preview")]
    [SerializeField] Transform player = null;
    [SerializeField] List<GameObject> pinPool = new List<GameObject>();

    [SerializeField] List<Transform> viableSpawnPoints = new List<Transform>();

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    public void Initialize()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        pinPool.Clear();
    }

    public void RemovePin(GameObject pin)
    {
        if (pinPool.Contains(pin)) pinPool.Remove(pin);
    }

    void SpawnPins(int patternIndex)
    {
        if (player == null && GameObject.FindGameObjectWithTag("Player")) player = GameObject.FindGameObjectWithTag("Player").transform;

        viableSpawnPoints.Clear();

        if (player != null)
        {
            for (int i = 0; i < pinSpawnPoints.Length; i++)
            {
                if (Vector3.Distance(player.position, pinSpawnPoints[i].position) >= minSpawningRadius &&
                    Vector3.Distance(player.position, pinSpawnPoints[i].position) <= maxSpawningRadius)
                {
                    viableSpawnPoints.Add(pinSpawnPoints[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < pinSpawnPoints.Length; i++)
                viableSpawnPoints.Add(pinSpawnPoints[i]);
        }

        int spawnCounter = 0;
        int patternSpawnTotal = Mathf.RoundToInt(Random.Range(spawnPattern[patternIndex].x, spawnPattern[patternIndex].y));

        while (spawnCounter < patternSpawnTotal && pinPool.Count < maxTotalPins)
        {
            int spawnPointIndex = Random.Range(0, viableSpawnPoints.Count);

            GameObject newPin = Instantiate(pinPrefab, viableSpawnPoints[spawnPointIndex].position, viableSpawnPoints[spawnPointIndex].rotation);

            newPin.transform.position += new Vector3(Random.Range(-spawnPointRadius.x, spawnPointRadius.x),
                                                        Random.Range(-spawnPointRadius.y, spawnPointRadius.y),
                                                        Random.Range(-spawnPointRadius.z, spawnPointRadius.z));

            newPin.transform.parent = transform;
            newPin.GetComponent<PinController>().Initialize(this);

            pinPool.Add(newPin);

            spawnCounter += 1;
        }
    }

    IEnumerator SpawnRoutine()
    {
        float timer = 0.0f;
        int patternCounter = 0;

        while (true)
        {
            if (pinPool.Count >= maxTotalPins)
            {
                timer = 0.0f;
                yield return null;
                continue;
            }

            if (timer < spawnInterval) timer += Time.deltaTime;
            else
            {
                timer = 0.0f;

                SpawnPins(patternCounter);

                patternCounter += 1;
                if (patternCounter >= spawnPattern.Length - 1) patternCounter = 0;
            }
            
            yield return null;
        }
    }
}
