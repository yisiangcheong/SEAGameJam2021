﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectPooler : MonoBehaviour
{
    [SerializeField] HitEffect effectPrefab = null;
    [SerializeField] int initialPoolSize = 20;

    [Header("Preview")]
    [SerializeField] List<HitEffect> poolList = new List<HitEffect>();

    private void OnEnable()
    {
        PopulatePool();
    }

    void PopulatePool()
    {
        poolList.Clear();

        for (int i = 0; i < initialPoolSize; i++)
        {
            HitEffect newEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            newEffect.transform.parent = transform;
            newEffect.gameObject.SetActive(false);

            poolList.Add(newEffect);
        }
    }

    public void SpawnHitEffect(Vector3 pos)
    {
        bool populateNewSlot = true;

        for (int i = 0; i < poolList.Count; i++)
        {
            if (!poolList[i].gameObject.activeInHierarchy)
            {
                poolList[i].Initialize(pos);
                populateNewSlot = false;
                break;
            }
        }

        if (!populateNewSlot) return;

        HitEffect newEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        newEffect.transform.parent = transform;
        newEffect.gameObject.SetActive(false);

        poolList.Add(newEffect);

        newEffect.Initialize(pos);
    }
}
