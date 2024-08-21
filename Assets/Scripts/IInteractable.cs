using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void MakeInteraction(Transform player);
    public bool IsItem();
}
