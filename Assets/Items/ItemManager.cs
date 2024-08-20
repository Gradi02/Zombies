using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour, IInteractable
{
    public void MakeInteraction()
    {
        Debug.Log("Podnies item");
    }

    public bool IsItem()
    {
        return true;
    }
}
