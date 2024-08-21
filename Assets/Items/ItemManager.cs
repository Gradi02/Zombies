using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour, IInteractable
{
    public ulong parentID { get; private set; } = 100;

    public void MakeInteraction(ulong ID)
    {
        if(parentID == 100)
        {
            UpdateItemParentServerRpc();
            Transform parent = NetworkManager.Singleton.ConnectedClients[ID].PlayerObject.transform;
            parent.GetComponent<PlayerItemHolder>().CollectItem(gameObject);
        }
        else
        {
            Debug.Log("Ktoœ inny trzyma ten przedmiot!");
        }
    }

    public bool IsItem()
    {
        return true;
    }

    public void ResetItemParent()
    {
        parentID = 100;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateItemParentServerRpc()
    {
        UpdateItemParentClientRpc(parentID);
    }

    [ClientRpc]
    private void UpdateItemParentClientRpc(ulong newParent)
    {
        parentID = newParent;
    }
}
