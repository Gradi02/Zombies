using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DialogueInvoker : NetworkBehaviour
{
    [SerializeField] private string dialog;

    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.LocalClient.PlayerObject != null)
        {
            if (other.gameObject == NetworkManager.LocalClient.PlayerObject.gameObject)
            {
                other.GetComponent<DialogueController>().AddDialogueToQueue(dialog, 5f, true);
                gameObject.SetActive(false);
            }
        }
    }
}
