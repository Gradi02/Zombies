using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class safeInteraction : NetworkBehaviour, IInteractable
{
    [SerializeField] private ItemManager cardItem;
    [SerializeField] private Animator animator;
    private NetworkVariable<bool> opened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if(!opened.Value)
        {
            if(playerItemHolder.itemInHand != null && playerItemHolder.itemInHand.GetComponent<ItemManager>().itemId == cardItem.itemId)
            {
                return "Press E To Open The Safe!";
            }

            return "Find Access Card To Open The Safe!";
        }

        return "The Safe Is Already Opened!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if (!opened.Value && playerItemHolder.itemInHand != null && playerItemHolder.itemInHand.GetComponent<ItemManager>().itemId == cardItem.itemId)
        {
            RequestOpenSafeServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOpenSafeServerRpc()
    {
        opened.Value = true;
        animator.SetTrigger("open");
    }
}
