using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject pref;
    void Update()
    {
        if (!IsServer) return;
        
        if(Input.GetKeyDown(KeyCode.G))
        {
            NetworkObject en = Instantiate(pref, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
            en.Spawn();
            NetworkGameManager.instance.AddEnemyToList(en.GetComponent<EnemyAI>());
        }
    }
}
