using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager instance { get; private set; } = null;

    public Dictionary<ulong, GameObject> playersServerList = new Dictionary<ulong, GameObject>();
    public List<EnemyAI> enemiesServerList = new List<EnemyAI>();

    public bool gameStarted { get; private set; } = false;
    [SerializeField] private GameObject barrier;
    [SerializeField] private LightingManager lighting;
    [SerializeField] private GameObject pc;
    [SerializeField] private Animator phoneAnim;
    private bool ring = false;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        RemoveDeadEnemyFromList();
        UpdateEnemiesTargetsList();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerToDictionaryServerRpc(ulong id)
    {
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
        playersServerList.Add(id, playerObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayerFromDictionaryServerRpc(ulong id)
    {
        if (playersServerList.ContainsKey(id))
        {
            playersServerList.Remove(id);
        }
    }

    public void AddEnemyToList(EnemyAI enemy)
    {
        enemiesServerList.Add(enemy);
    }

    private void RemoveDeadEnemyFromList()
    {
        for (int i = enemiesServerList.Count - 1; i >= 0; i--)
        {
            if (enemiesServerList[i] != null && enemiesServerList[i].isDead)
            {
                enemiesServerList.RemoveAt(i);
            }
        }
    }

    private void UpdateEnemiesTargetsList()
    {
        foreach (EnemyAI e in enemiesServerList)
        {
            if (e != null && !e.isDead)
            {
                int index = 0;
                foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
                {
                    if (!e.players.Contains(clientPair.Value.PlayerObject.gameObject))
                    {
                        e.players.Add(clientPair.Value.PlayerObject.gameObject);
                        index++;
                    }
                }
            }
        }
    }

    public void onClientJoin()
    {
        lighting.SetupSunServerRpc();
    }

    public void onHostCreated()
    {
        lighting.SetupSunServerRpc();
    }





    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        gameStarted = true;
        SteamNetworkManager.instance.GameStartHandler();
        ring = true;
        StartCoroutine(PhoneCall());
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        phoneAnim.gameObject.layer = 10;
    }

    [ClientRpc]
    private void CallSoundClientRpc()
    {
        FindObjectOfType<AudioManager>().Play("ringing");
    }

    [ServerRpc(RequireOwnership = false)]
    public void CallAnswerServerRpc()
    {
        ring = false;
        OpenBarrierClientRpc();
    }

    [ClientRpc]
    private void OpenBarrierClientRpc()
    {
        Destroy(barrier);
        phoneAnim.enabled = false;
        FindObjectOfType<AudioManager>().Stop("ringing");
        Debug.Log("Ciocia Dzwoni AAAAAAAAAAAAAA!");
    }



    private IEnumerator PhoneCall()
    {
        while(ring)
        {
            phoneAnim.SetTrigger("ringing");
            CallSoundClientRpc();
            yield return new WaitForSeconds(3f);
        }
    }
}
