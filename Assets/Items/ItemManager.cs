using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour, IInteractable
{
    public PlayerItemHolder parent { get; private set; } = null;

    public void MakeInteraction(Transform player)
    {
        if(parent == null)
        { 
            parent = player.GetComponent<PlayerItemHolder>();
            parent.CollectItem(gameObject);
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
        parent = null;
    }
}
