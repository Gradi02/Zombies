using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager instance { get; private set; } = null;

    public Dictionary<ulong, GameObject> connectedPlayers = new Dictionary<ulong, GameObject>();

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

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerToDictionaryServerRpc(ulong id)
    {
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
        connectedPlayers.Add(id, playerObject);

        AddPlayerToDictionaryClientRpc(id);
    }

    [ClientRpc]
    public void AddPlayerToDictionaryClientRpc(ulong id)
    {
        if (connectedPlayers.ContainsKey(id)) return;

        GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).gameObject;
        connectedPlayers.Add(id, playerObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayerFromDictionaryServerRpc(ulong id)
    {
        if (connectedPlayers.ContainsKey(id))
        {
            connectedPlayers.Remove(id);
            RemovePlayerFromDictionaryClientRpc(id);
        }
    }

    [ClientRpc]
    public void RemovePlayerFromDictionaryClientRpc(ulong id)
    {
        if (connectedPlayers.ContainsKey(id))
        {
            connectedPlayers.Remove(id);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void RequestDictionarySyncServerRpc(ServerRpcParams rpcParams = default)
    {
        foreach (var entry in connectedPlayers)
        {
            AddPlayerToDictionaryClientRpc(entry.Key, rpcParams.Receive.SenderClientId);
        }
    }

    [ClientRpc]
    private void AddPlayerToDictionaryClientRpc(ulong id, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        if (!connectedPlayers.ContainsKey(id))
        {
            GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).gameObject;
            connectedPlayers.Add(id, playerObject);
        }
    }
}
