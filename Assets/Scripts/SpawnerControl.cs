using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerControl : NetworkSingleton<SpawnerControl>
{
    [SerializeField]
    private GameObject objectPrefab;

    [SerializeField] private int maxObjectInstanceCount = 3;

    private void Awake()
    {
        //inital pool
    }

    public void SpawnObjects()
    {
        if(!IsServer) return;

        for (int i = 0; i < maxObjectInstanceCount; i++)
        {
            // GameObject go = Instantiate(objectPrefab, 
            //     new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10)), Quaternion.identity);
            GameObject go = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab).gameObject;
            go.transform.position = new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10));
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}
