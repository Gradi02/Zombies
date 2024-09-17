using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DialogueController : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    public List<Dialogue> dialoguesQueue = new();
    private float nextShow = 0;
    private bool force = false;

    void Update()
    {
        if (!IsOwner) return;

        if (Time.time > nextShow)
        {
            if (dialoguesQueue.Count > 0)
            {
                nextShow = Time.time + dialoguesQueue[0].showTime;
                dialogueText.text = dialoguesQueue[0].text;
                dialoguesQueue.RemoveAt(0);
            }
            else
            {
                dialogueText.text = "";
            }
        }
        else if(force && dialoguesQueue.Count > 0)
        {
            force = false;
            int idx = dialoguesQueue.Count - 1;
            Dialogue dl = dialoguesQueue[idx];
            nextShow = Time.time + dl.showTime;
            dialogueText.text = dl.text;
            dialoguesQueue.Remove(dl);
        }
    }

    public void AddDialogueToQueue(string dText, float dTime, bool fShow = false)
    {
        dialoguesQueue.Add(new Dialogue(dText, dTime, fShow));
        force = fShow;
    }
}

[System.Serializable]
public class Dialogue
{
    public string text;
    public float showTime;
    public bool forceShow = false;

    public Dialogue(string dText, float dTime, bool fShow)
    {
        text = dText;
        showTime = dTime;
        forceShow = fShow;
    }
}
