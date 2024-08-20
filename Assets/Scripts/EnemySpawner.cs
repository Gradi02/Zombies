using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : MonoBehaviour
{
    public GameObject pref;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            NetworkObject en = Instantiate(pref, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
            en.Spawn();
        }
    }
}
