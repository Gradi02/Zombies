using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCall : MonoBehaviour, IInteractable
{
    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        NetworkGameManager.instance.CallAnswerServerRpc();
        gameObject.layer = 0;
    }
}
