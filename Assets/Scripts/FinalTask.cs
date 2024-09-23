using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FinalTask : NetworkBehaviour, IInteractable
{
    [SerializeField] private MainTasksManager mtm;
    [SerializeField] private EnemySpawner spawner;
    private bool finalEvent = false;
    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if(mtm.finalTask)
        {
            if(!finalEvent)
                return "Hole E To Call Your Boss!";

            return "Event In Progress!";
        }

        return "Tasks Are Not Finished Yet!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(mtm.finalTask && !finalEvent)
        {
            spawner.StartFinalEventSpawnerServerRpc();
            FinalEventClientRpc();
        }
    }

    [ClientRpc]
    private void FinalEventClientRpc()
    {
        finalEvent = true;
        NetworkGameManager.instance.finalEvent = true;
    }
}
