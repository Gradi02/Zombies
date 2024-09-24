using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using TMPro;

public class NetworkGameManager : NetworkBehaviour
{
    private int startGameTime = 5;
    public int daysToEmergency { get; private set; }
    public bool finalEvent { get; set; }
    public int timeToWin1 = 10;
    public float timeToWin2 = 0;
    public int hoursToEmergency { get; set; }
    public static NetworkGameManager instance { get; private set; } = null;

    public Dictionary<ulong, GameObject> playersServerList = new Dictionary<ulong, GameObject>();
    public List<StateMachine> enemiesServerList = new List<StateMachine>();
    public int deadPlayers { get; private set; } = 0;

    [HideInInspector] public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [HideInInspector] public NetworkVariable<bool> tasksStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private GameObject barrier;
    [SerializeField] private LightingManager lighting;
    [SerializeField] private GameObject pc;
    [SerializeField] private Animator phoneAnim;
    private bool ring = false;
    public int currentDay { get; private set; } = 0;
    public EnemySpawner spawner;
    public Volume globalVolume;
    [SerializeField] private GameObject gravePrefab;
    [SerializeField] private DoorLock militaryDoorLock;
    public MainTasksManager mainTasksManager;

    private string[] finalDial =
    {
        "Boss: You did it my amigos! Your reward is coming for you!",
        "Boss: PERO before that we need your help once more...",
        "Boss: We are coming for you and Conrad will give you más detalles!",
        "Conrad von Cookenberg: Yes We Need To Talk..."
    };

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

        if(finalEvent)
        {
            timeToWin2 -= Time.deltaTime;
            if(timeToWin2 <= 0)
            {
                timeToWin1 -= 1;
                timeToWin2 = 60;
            }

            if(timeToWin1 <= 0)
            {
                Debug.Log("WIN!");
                StartDialogueServerRpc();
            }
        }
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

    public void AddEnemyToList(StateMachine enemy)
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
        foreach (StateMachine e in enemiesServerList)
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

    public void onClientJoin(ulong _cliendId)
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
        gameStarted.Value = true;
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
        CallAnswerClientRpc();
    }

    [ClientRpc]
    private void CallAnswerClientRpc()
    {
        phoneAnim.enabled = false;
        FindObjectOfType<AudioManager>().Stop("ringing");
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndPhoneServerRpc()
    {
        spawner.StartSpawner();
        OpenBarrierClientRpc();
    }

    [ClientRpc]
    private void OpenBarrierClientRpc()
    {
        Destroy(barrier);
        militaryDoorLock.GenerateCodeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnlockAllTasksServerRpc()
    {
        tasksStarted.Value = true;
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


    public void UpdateDayValue(int nday)
    {
        currentDay = nday;
        daysToEmergency = startGameTime - currentDay;
    }


    [ServerRpc(RequireOwnership = false)]
    public void HandleDeadPlayerServerRpc(ulong playerId)
    {
        Transform deadPlayer = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.transform;

        GameObject grave = Instantiate(gravePrefab, deadPlayer.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        grave.GetComponent<NetworkObject>().Spawn();
        grave.GetComponent<GraveManager>().deadPlayerId = playerId;

        deadPlayers++;
        ControllGameState();
    }

    private void ControllGameState()
    {
        if(deadPlayers == NetworkManager.Singleton.ConnectedClients.Count)
        {
            Debug.LogWarning("GAMEOVER!!!!!!!!!!!!!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandlePlayerReviveServerRpc(ulong playerId)
    {
        RevivePlayerClientRpc(playerId);
        deadPlayers--;
    }

    [ClientRpc]
    private void RevivePlayerClientRpc(ulong id)
    {
        if(NetworkManager.Singleton.LocalClientId == id)
        {
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerStats>().RevivePlayer();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void StartDialogueServerRpc()
    {
        ShowDialClientRpc();
    }

    [ClientRpc]
    private void ShowDialClientRpc()
    {
        DialogueController contr = NetworkManager.LocalClient.PlayerObject.GetComponent<DialogueController>();

        contr.AddDialogueToQueue(finalDial[0], 5f);
        contr.AddDialogueToQueue(finalDial[1], 5f);
        contr.AddDialogueToQueue(finalDial[2], 5.5f);
        contr.AddDialogueToQueue(finalDial[3], 4.5f);
    }
}
