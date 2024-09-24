using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PhoneCall : NetworkBehaviour, IInteractable
{
    private string[] dialText =
    {
        "Boss: Hola grandes amigos, years of yours hard work gave us a hope for better future!",
        "Boss: Your Cure es perfecto, your mission is going to end soon.",
        "Boss: Locate The Townhall and prepare the rest for our arrival or you'll be...",
        "Conrad von Cookenberg: Dinner Time dear Lord!",
        "Boss: Oh, I need to go my amigos, Good Luck!"
    };


    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        StartDialogueServerRpc();
        gameObject.layer = 0;
    }

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        return "Press E To Interact!";
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartDialogueServerRpc()
    {
        ShowDialClientRpc();
        NetworkGameManager.instance.CallAnswerServerRpc();
        Invoke(nameof(FinishDialogue), 25f);
    }

    [ClientRpc]
    private void ShowDialClientRpc()
    {
        DialogueController contr = NetworkManager.LocalClient.PlayerObject.GetComponent<DialogueController>();
        
        contr.AddDialogueToQueue(dialText[0], 6f);
        contr.AddDialogueToQueue(dialText[1], 5.5f);
        contr.AddDialogueToQueue(dialText[2], 6f);
        contr.AddDialogueToQueue(dialText[3], 5f);
        contr.AddDialogueToQueue(dialText[4], 5f);
    }

    private void FinishDialogue()
    {
        NetworkGameManager.instance.EndPhoneServerRpc();
    }
}
