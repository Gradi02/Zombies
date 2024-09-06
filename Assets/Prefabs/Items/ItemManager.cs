using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour, IInteractable
{
    //public ulong parentID { get; private set; } = 100;
    public NetworkVariable<ulong> parentID = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private PlayerItemHolder pih;
    public string itemId = "";
    public bool usable = false;

    public void MakeInteraction(ulong ID, PlayerItemHolder ph)
    {
        if(parentID.Value == 100)
        {
            ph.CollectItem(gameObject);
            UpdateItemParentServerRpc(ID);
        }
        else
        {
            Debug.Log("Ktoœ inny trzyma ten przedmiot!");
        }
    }

    public string GetInteractionText()
    {
        return "Press E To Collect " + itemId + "!";
    }

    public void ConsumeEffect(PlayerItemHolder player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.HealPlayer(20);
        player.GetComponent<PostProcessingController>().StartVodkaEffect(30f);
        stats.Slow(30f, 5f);
    }




    [ServerRpc(RequireOwnership = false)]
    public void ConsumeItemServerRpc()
    {
        pih.ConsumeItem();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetItemParentServerRpc()
    {
        parentID.Value = 100;
        pih = null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateItemParentServerRpc(ulong id)
    {
        parentID.Value = id;
    }

    private void LateUpdate()
    {
        if (!IsServer) return;

        if (parentID.Value != 100)
        {
            if (pih != null)
            {
                transform.SetPositionAndRotation(pih.handTransform.position, pih.handTransform.rotation);
            }
            else
            {
                Transform parent = NetworkManager.Singleton.ConnectedClients[parentID.Value].PlayerObject.transform;
                pih = parent.GetComponent<PlayerItemHolder>();
            }
        }
    }
}
