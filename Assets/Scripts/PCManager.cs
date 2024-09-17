using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PCManager : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject pcCanva;
    [SerializeField] private TextMeshProUGUI[] playerSlots;
    private float timeCounter = 0f;
    public float interval = 1f;

    void Update()
    {
        if(IsServer)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= interval)
            {
                RefreshPlayerListServerRpc();
                timeCounter = 0f;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RefreshPlayerListServerRpc()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > i)
            {
                string name = NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<SC_FPSController>().steamName.Value.ToString();
                SetNameClientRpc(i, name);
            }
            else
            {
                SetNameClientRpc(i);
            }
        }
    }

    [ClientRpc]
    private void SetNameClientRpc(int i, string n = "")
    {
        if (n == "")
        {
            playerSlots[i].gameObject.SetActive(false);
        }
        else
        {
            playerSlots[i].gameObject.SetActive(true);
            playerSlots[i].text = n;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableCanvaServerRpc()
    {
        DisableCanvaClientRpc();
    }

    [ClientRpc]
    private void DisableCanvaClientRpc()
    {
        pcCanva.SetActive(false);
    }


    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        NetworkGameManager.instance.StartGameServerRpc();
        DisableCanvaServerRpc();
        gameObject.layer = 0;
    }

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        return "Press E To Interact!";
    }
}
