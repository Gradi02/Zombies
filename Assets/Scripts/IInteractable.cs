using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null);

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null);
}
