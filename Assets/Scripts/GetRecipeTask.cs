using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GetRecipeTask : TaskManager, IInteractable
{

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(!taskStarted.Value)
        {
            StartTask();
        }
        else
        {
            if(playerItemHolder.itemInHand != null)
            {
                int id = playerItemHolder.itemInHand.GetComponent<ItemManager>().itemId;
                testedItem = playerItemHolder.itemInHand;
                RequestAddItemServerRpc(id);
            }
        }
    }
}
