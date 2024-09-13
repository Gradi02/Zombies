using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MainTasksManager : NetworkBehaviour, IInteractable
{
    private bool tasksStarted = false;

    [SerializeField] private GameObject tasksCanva;
    [SerializeField] private MainTasks[] maintasks;

    public string GetInteractionText()
    {
        return "Press E To Interact!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if (!tasksStarted)
        {
            tasksStarted = true;
            StartTasksServerRpc();
        }
    }

    void FixedUpdate()
    {
        for(int i = 0; i < maintasks.Length; i++)
        {
            maintasks[i].taskProgressText.text = $"{maintasks[i].taskProgress} / {maintasks[i].taskSteps}";

            if (maintasks[i].finished)
            {
                maintasks[i].taskProgressText.color = Color.green;
                maintasks[i].taskParentText.color = Color.green;
            }
            else
            {
                maintasks[i].taskProgressText.color = Color.white;
                maintasks[i].taskParentText.color = Color.white;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartTasksServerRpc()
    {
        StartTasksClientRpc();

        NetworkGameManager.instance.UnlockAllTasksServerRpc();
    }

    [ClientRpc]
    private void StartTasksClientRpc()
    {
        tasksStarted = true;
        tasksCanva.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestProgressTaskServerRpc(string task)
    {
        for(int i=0; i<maintasks.Length; i++)
        {
            if(task == maintasks[i].taskName)
            {
                if (!maintasks[i].finished)
                {
                    maintasks[i].taskProgress++;
                    if (maintasks[i].taskProgress >= maintasks[i].taskSteps)
                    {
                        ShowTasksInfoClientRpc(i, maintasks[i].taskProgress);
                    }
                }
                else
                {
                    Debug.LogWarning("You Are Trying To Progress Task That Is Already Finished!");
                }

                return;
            }
        }

        Debug.LogWarning($"Task {task} does not exist!");
    }

    [ClientRpc]
    private void ShowTasksInfoClientRpc(int idx, int progress)
    {
        maintasks[idx].taskProgress = progress;
    }
}

[System.Serializable]
public class MainTasks
{
    public string taskName;
    public int taskSteps;
    public int taskProgress;
    public bool finished;

    public TextMeshProUGUI taskProgressText;
    public TextMeshProUGUI taskParentText;
}