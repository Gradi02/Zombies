using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TaskManager : NetworkBehaviour
{
    protected NetworkVariable<bool> taskStarted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public List<GameObject> possibleTaskItems = new List<GameObject>();
    protected List<string> taskItems = new List<string>();
    protected int itemsToCollect = 5;
    protected GameObject testedItem;

    [SerializeField] protected GameObject canva;
    [SerializeField] protected TextMeshProUGUI[] itemsCanva;


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
        StartTaskClientRpc();
        ShowListServerRpc();
    }

    [ClientRpc]
    private void StartTaskClientRpc()
    {
        canva.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowListServerRpc()
    {
        for (int i = 0; i < itemsToCollect; i++)
        {
            ShowItemClientRpc(i, taskItems[i]);
        }
    }

    [ClientRpc]
    private void ShowItemClientRpc(int i, string n)
    {
        canva.SetActive(true);
        itemsCanva[i].gameObject.SetActive(true);
        itemsCanva[i].text = n;
    }

    [ServerRpc(RequireOwnership = false)]
    protected void RequestAddItemServerRpc(string idx)
    {
        if(idx != "" && taskItems.Contains(idx))
        {
            taskItems.Remove(idx);
            testedItem.GetComponent<ItemManager>().ConsumeItemServerRpc();
            testedItem = null;
            RefreshCanvaItemsServerRpc();
            Debug.Log("Dodano Przedmiot!");
        }

        if(taskItems.Count == 0)
        {
            gameObject.layer = 0;
            Debug.Log("Task Ukoñczony!!!!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RefreshCanvaItemsServerRpc()
    {
        for (int i = 0; i < itemsCanva.Length; i++)
        {
            if (taskItems.Count > i && taskItems[i] != null)
                ResetCanvaClientRpc(i, taskItems[i]);
            else
                ResetCanvaClientRpc(i);
        }
    }

    [ClientRpc]
    private void ResetCanvaClientRpc(int i, string n = "")
    {
        itemsCanva[i].text = "";

        if(n == "")
            itemsCanva[i].gameObject.SetActive(false);
        else
            itemsCanva[i].text = n;
    }
}
