using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class LockDoorHint : NetworkBehaviour, IInteractable
{
    [HideInInspector] public NetworkVariable<FixedString512Bytes> hint = new NetworkVariable<FixedString512Bytes>("nope", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        return "Press E to Interact!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        playerItemHolder.GetComponent<DialogueController>().AddDialogueToQueue(hint.Value.ToString(), 5f, true);
    }
}
