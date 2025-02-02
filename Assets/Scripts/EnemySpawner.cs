using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject mutant;
    private bool isEnabled = false;
    public GameObject pref;
    [SerializeField] private float maxRadius;
    [SerializeField] private LayerMask mask;

    //Spawner
    private int day => NetworkGameManager.instance.currentDay;
    private int[] maxZombiesOnMap =
    {
        20,
        30,
        50,
        80,
        100
    };

    void Update()
    {
        if (!IsServer || !isEnabled) return;

        int idx = day > maxZombiesOnMap.Length ? maxZombiesOnMap[maxZombiesOnMap.Length - 1] : maxZombiesOnMap[day];
        float multiplayerMultiplier = (NetworkManager.Singleton.ConnectedClients.Count-1) * 0.2f + 1;
        if (NetworkGameManager.instance.enemiesServerList.Count < idx * multiplayerMultiplier)
        {
            SpawnEnemy();
        }
    }


    private void SpawnEnemy(Vector3? pos = null)
    {
        if (!IsHost) return;
        NetworkObject en = Instantiate(pref,GetSpawnPos(pos ?? Vector3.zero), Quaternion.identity).GetComponent<NetworkObject>();
        en.Spawn();
        NetworkGameManager.instance.AddEnemyToList(en.GetComponent<EnemyAI>());
        //Debug.Log("<color=#99ff99>Successfully</color> <color=#ffaa99>created</color> <color=#1199ff>new</color> <color=#eeee11>Enemy!</color>");
    }

    public void RequestBossMinion(Vector3 targetpos)
    {
        if (!IsHost) return;
        SpawnEnemy(targetpos);
    }

    private Vector3 GetSpawnPos(Vector3 searchPos)
    {
        Vector3 pos = Vector3.zero;
        bool pathCorrect = true;
        int i = 0;
        while (pathCorrect && (i++) < 100)
        {
            Vector3 newDestRay = searchPos;
            if (searchPos == Vector3.zero)
            {
                float r = Random.Range(0, maxRadius);
                float fi = Random.Range(0, 360);
                newDestRay = new Vector3(r * Mathf.Cos(fi), 1000, r * Mathf.Sin(fi)) + new Vector3(-70, 0, -30);
            }
            else
            {
                float r = Random.Range(5, 10);
                float fi = Random.Range(0, 360);
                newDestRay = new Vector3(r * Mathf.Cos(fi), searchPos.y + 3f, r * Mathf.Sin(fi)) + new Vector3(searchPos.x, 0, searchPos.z);
            }


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

                        if((hit2.position - playerPosition).sqrMagnitude < 400)
                        {
                            visibleToAnyPlayer = true;
                            break; // No need to check further players
                        }

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

    [ServerRpc(RequireOwnership = false)]
    public void StartFinalEventSpawnerServerRpc()
    {
        StartCoroutine(IEfinal());
    }

    private IEnumerator IEfinal()
    {
        yield return new WaitForSeconds(1f);
    }
}
