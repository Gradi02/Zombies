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
            SpawnEnemyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc()
    {
        NetworkObject en = Instantiate(pref, new Vector3(-45, -5, -30), Quaternion.identity).GetComponent<NetworkObject>();
        en.Spawn();
        NetworkGameManager.instance.AddEnemyToList(en.GetComponent<EnemyAI>());
        Debug.Log("<color=#99ff99>Successfully created new Enemy!</color>");
    }
}
