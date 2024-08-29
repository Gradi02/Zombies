using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCManager : MonoBehaviour, IInteractable
{
    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        NetworkGameManager.instance.StartGameServerRpc();
        gameObject.layer = 0;
    }
}
