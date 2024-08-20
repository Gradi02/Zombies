using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : MonoBehaviour, IInteractable
{
    //private NetworkVariable<ulong> itemID = new NetworkVariable<ulong>();

    public void MakeInteraction()
    {
        Debug.Log(gameObject.GetInstanceID());
    }

    public bool IsItem()
    {
        return true;
    }
}
