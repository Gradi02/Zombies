using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    private bool isEnabled = false;
    public GameObject pref;
    [SerializeField] private float maxRadius;
    [SerializeField] private LayerMask mask;

    //Spawner
    private int day => NetworkGameManager.instance.currentDay;
    private int[] maxZombiesOnMap =
    {
        15,
        18,
        20,
        25,
        30,
        35,
        40
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnEnemyServerRpc();
        }

        if (!IsServer || !isEnabled) return;

        if(NetworkGameManager.instance.enemiesServerList.Count < maxZombiesOnMap[day])
        {
            SpawnEnemyServerRpc();
        }




        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnEnemyServerRpc(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(bool p = false)
    {
        NetworkObject en = Instantiate(pref, p ? new Vector3(-30, -5, -30) : GetSpawnPos(), Quaternion.identity).GetComponent<NetworkObject>();
        en.Spawn();
        NetworkGameManager.instance.AddEnemyToList(en.GetComponent<EnemyAI>());
        //Debug.Log("<color=#99ff99>Successfully</color> <color=#ffaa99>created</color> <color=#1199ff>new</color> <color=#eeee11>Enemy!</color>");
    }

    private Vector3 GetSpawnPos()
    {
        Vector3 pos = Vector3.zero;
        bool pathCorrect = true;
        while (pathCorrect)
        {
            float r = Random.Range(0, maxRadius);
            float fi = Random.Range(0, 360);
            Vector3 newDestRay = new Vector3(r*Mathf.Cos(fi), 1000, r * Mathf.Sin(fi)) + new Vector3(-70, 0, -30);

            if (Physics.Raycast(newDestRay, Vector3.down, out RaycastHit hit, 1100, mask))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit hit2, 3f, NavMesh.AllAreas))
                {
                    bool visibleToAnyPlayer = false;

                    foreach(var player in NetworkGameManager.instance.playersServerList)
                    {
                        // Assume players have a position field and an appropriate transform
                        Vector3 playerPosition = player.Value.transform.position;
                        Vector3 directionToSpawn = (hit2.position - playerPosition).normalized;

                        if (Physics.Raycast(playerPosition, directionToSpawn, out RaycastHit playerHit, Mathf.Infinity))
                        {
                            // If the hit point is close to the spawn position, it means the player can see it
                            if (Vector3.Distance(playerHit.point, hit2.position) < 1.0f)
                            {
                                visibleToAnyPlayer = true;
                                break; // No need to check further players
                            }
                        }
                    }

                    if (!visibleToAnyPlayer)
                    {
                        pathCorrect = false;
                        pos = hit2.position;
                    }
                }
            }
        }

        return pos;
    }

    public void StartSpawner()
    {
        if (IsServer)
        {
            isEnabled = true;
        }
    }
}
