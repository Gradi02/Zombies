using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GetRecipeTask : TaskManager, IInteractable
{

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if (!NetworkGameManager.instance.gameStarted.Value) return;

        if(!taskStarted.Value)
        {
            StartTask();
        }
        else
        {
            if(playerItemHolder.itemInHand != null)
            {
                string id = playerItemHolder.itemInHand.GetComponent<ItemManager>().itemId;
                testedItem = playerItemHolder.itemInHand;
                RequestAddItemServerRpc(id);
            }
        }
    }

    public string GetInteractionText()
    {
        return "Press E To Interact!";
    }
}
