using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject pref;
    void Update()
    { 
        if(Input.GetKeyDown(KeyCode.G))
        {
            SpawnEnemyServerRpc(new Vector3(-45, -5, -30));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnEnemyServerRpc(new Vector3(-45, -5, -10));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(Vector3 pos)
    {
        NetworkObject en = Instantiate(pref, pos, Quaternion.identity).GetComponent<NetworkObject>();
        en.Spawn();
        NetworkGameManager.instance.AddEnemyToList(en.GetComponent<EnemyAI>());
        Debug.Log("<color=#99ff99>Successfully</color> <color=#ffaa99>created</color> <color=#1199ff>new</color> <color=#eeee11>Enemy!</color>");
    }
}
