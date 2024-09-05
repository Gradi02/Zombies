using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TaskManager : NetworkBehaviour
{
    protected NetworkVariable<bool> taskStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public List<GameObject> possibleTaskItems = new List<GameObject>();
    protected List<int> taskItems = new List<int>();
    protected int itemsToCollect = 5;
    protected GameObject testedItem;

    protected void StartTask()
    {
        Debug.Log("Test Started!");
        StartTaskServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void StartTaskServerRpc()
    {
        taskStarted.Value = true;
        for(int i=0; i<itemsToCollect; i++)
        {
            if (possibleTaskItems.Count > 0)
            {
                int idx = Random.Range(0, possibleTaskItems.Count);
                taskItems.Add(possibleTaskItems[idx].GetComponent<ItemManager>().itemId);
                possibleTaskItems.RemoveAt(idx);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected void RequestAddItemServerRpc(int idx)
    {
        if(idx > 0 && taskItems.Contains(idx))
        {
            taskItems.Remove(idx);
            testedItem.GetComponent<ItemManager>().ConsumeItemServerRpc();
            testedItem = null;
            Debug.Log("Dodano Przedmiot!");
        }

        if(taskItems.Count == 0)
        {
            gameObject.layer = 0;
            Debug.Log("Task Ukoñczony!!!!");
        }
    }

}
