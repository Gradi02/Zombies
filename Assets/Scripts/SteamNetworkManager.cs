using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;

public class SteamNetworkManager : MonoBehaviour
{
    public static SteamNetworkManager instance { get; private set; } = null;

    public FacepunchTransport transport { get; set; } = null;

    public Lobby? currentLobby { get; private set; } = null;

    public ulong hostId;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;

    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        if (NetworkManager.Singleton == null)
        {
            return;
        }
        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;

    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting. Performing cleanup.");

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Disconnecting Clients: ");
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                Debug.Log(client.ClientId);
                NetworkManager.Singleton.DisconnectClient(client.ClientId);
            }
        }

        Disconnected();
    }

    //when you accept the invite or Join on a friend
    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamId)
    {
        RoomEnter joinedLobby = await _lobby.Join();
        if (joinedLobby != RoomEnter.Success)
        {
            Debug.Log("Failed to create lobby");
        }
        else
        {
            NetworkGameManager.instance.onClientJoin();
            currentLobby = _lobby;
            Debug.Log("Joined Lobby");
        }
    }

    private void SteamMatchmaking_OnLobbyGameCreated(Lobby _lobby, uint _ip, ushort _port, SteamId _steamId)
    {
        Debug.Log("Lobby was created");
        NetworkGameManager.instance.onHostCreated();
    }

    //friend send you an steam invite
    private void SteamMatchmaking_OnLobbyInvite(Friend _steamId, Lobby _lobby)
    {
        Debug.Log($"Invite from {_steamId.Name}");
    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _steamId)
    {
        Debug.Log("member leave");
        NetworkGameManager.instance.RemovePlayerFromDictionaryServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend _steamId)
    {
        Debug.Log("member join");
        NetworkGameManager.instance.AddPlayerToDictionaryServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }
        Debug.Log("join lobby start client");
        StartClient(currentLobby.Value.Owner.Id);
    }

    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            Debug.Log("lobby was not created");
            return;
        }
        _lobby.SetPublic();
        _lobby.SetJoinable(true);
        _lobby.SetGameServer(_lobby.Owner.Id);
        Debug.Log($"lobby created FakeSteamName");
    }

    public async void StartHost(int _maxMembers)
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();

        // Update Client ID
        //NetworkGameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
        //NetworkGameManager.instance.AddPlayerToDictionaryServerRpc(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);

        currentLobby = await SteamMatchmaking.CreateLobbyAsync(_maxMembers);
    }

    public void InviteFriendToLobby()
    {
        SteamFriends.OpenGameInviteOverlay(currentLobby.Value.Owner.Id);
        Debug.Log("Show Friends");
    }

    public void StartClient(SteamId _sId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        transport.targetSteamId = _sId;

        // Update Client ID
        //NetworkGameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client has started");
        }
        else
        {
            Debug.Log("Failed to start Client");
        }
    }

    [ContextMenu("Disconnect")]
    public void Disconnected()
    {
        currentLobby?.Leave();
        if (NetworkManager.Singleton == null)
        {
            return;
        }
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;

            Debug.Log("Disconnecting Clients: ");
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                Debug.Log(client.ClientId);
                NetworkManager.Singleton.DisconnectClient(client.ClientId);
            }
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        }
        NetworkManager.Singleton.Shutdown(true);
        //NetworkGameManager.instance.onDisconnected();
        Debug.Log("disconnected");
    }

    private void OnDisconnectedFromServer()
    {
        Disconnected();
    }

    private void OnFailedToConnect()
    {
        Disconnected();
    }

    private void Singleton_OnClientDisconnectCallback(ulong _cliendId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
        if (_cliendId == 0)
        {
            Disconnected();
            //NetworkGameManager.instance.RemovePlayerFromDictionaryServerRpc(_cliendId);
        }
    }

    private void Singleton_OnClientConnectedCallback(ulong _cliendId)
    {
        Debug.Log($"Client has connected : AnotherFakeSteamName");
    }

    private void Singleton_OnServerStarted()
    {
        Debug.Log("Host started");
    }

    public void GameStartHandler()
    {
        currentLobby.Value.SetJoinable(false);
    }
}