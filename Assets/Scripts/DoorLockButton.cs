using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLockButton : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorLock doorLock;
    [SerializeField] private int num;
    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        return "";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        doorLock.EnterNumberServerRpc(num);
    }
}
