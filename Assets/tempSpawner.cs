using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class tempSpawner : MonoBehaviour
{
    public GameObject pref;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        NetworkObject en = Instantiate(pref,new Vector3(0, -5, 0), Quaternion.identity).GetComponent<NetworkObject>();
        en.Spawn();
        //Debug.Log("<color=#99ff99>Successfully</color> <color=#ffaa99>created</color> <color=#1199ff>new</color> <color=#eeee11>Enemy!</color>");
    }
}
