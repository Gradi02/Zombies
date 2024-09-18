using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MainTasksManager : NetworkBehaviour, IInteractable
{
    private bool tasksStarted = false;
    private bool tasksFinished = false;
    private bool finalTask = false;

    [SerializeField] private GameObject tasksCanva;
    [SerializeField] private MainTasks[] maintasks;
    [SerializeField] private GameObject secondCanva;
    [SerializeField] private TextMeshProUGUI secondCanvaText;

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if(!NetworkGameManager.instance.gameStarted.Value)
        {
            return "Game Has Not Been Started Yet!";
        }
        else if((tasksStarted && !tasksFinished) || finalTask)
        {
            return "Task In Progress!";
        }

        return "Press E To Interact!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if (!NetworkGameManager.instance.gameStarted.Value) return;

        if (!tasksStarted)
        {
            tasksStarted = true;
            StartTasksServerRpc();
        }
        else if(tasksFinished && !finalTask)
        {
            finalTask = true;
            StartFinalServerRpc();
        }
    }

    public MainTasks GetTaskByName(string nm)
    {
        for(int i = 0; i < maintasks.Length; i++)
        {
            if (maintasks[i].taskName == nm)
            {
                return maintasks[i];
            }
        }

        return null;
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
                    ShowTasksInfoClientRpc(i, maintasks[i].taskProgress);
                    if (maintasks[i].taskProgress >= maintasks[i].taskSteps)
                    {
                        maintasks[i].finished = true;
                        maintasks[i].taskProgressText.color = Color.green;
                        maintasks[i].taskParentText.color = Color.green;
                        CheckAllTasksServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    public void RequestFinishTaskServerRpc(string task)
    {
        MainTasks mt = GetTaskByName(task);
        mt.finished = true;
        mt.taskProgressText.color = Color.green;
        mt.taskParentText.color = Color.green;
        CheckAllTasksServerRpc();
    }

    [ClientRpc]
    private void ShowTasksInfoClientRpc(int idx, int progress)
    {
        maintasks[idx].taskProgress = progress;
        maintasks[idx].taskProgressText.text = $"{maintasks[idx].taskProgress} / {maintasks[idx].taskSteps}";
    }


    [ServerRpc(RequireOwnership = false)]
    private void CheckAllTasksServerRpc()
    {
        bool notAll = false;
        for (int i = 0; i < maintasks.Length; i++)
        {
            if (!maintasks[i].finished)
            {
                notAll = true;
                break;
            }
        }

        if(!notAll)
        {
            FinishAllTasksClientRpc();
        }
    }

    [ClientRpc]
    private void FinishAllTasksClientRpc()
    {
        tasksFinished = true;
        tasksCanva.SetActive(false);
        secondCanva.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartFinalServerRpc()
    {
        StartFinalClientRpc();
    }

    [ClientRpc]
    private void StartFinalClientRpc()
    {
        secondCanvaText.text = "Wait For The Rescue In {place}";
    }
}

[System.Serializable]
public class MainTasks
{
    public string taskName;
    public int taskSteps;
    public int taskProgress = -1;
    public bool finished;

    public TextMeshProUGUI taskProgressText;
    public TextMeshProUGUI taskParentText;
}