using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour, IInteractable
{
    private NetworkVariable<ulong> parentID = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private PlayerItemHolder pih;
    public string itemId = "";
    public bool usable = false;
    public bool dmgToUse = false;



    public void MakeInteraction(ulong ID, PlayerItemHolder ph)
    {
        if(parentID.Value == 0)
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

        if (itemId == "Alcohol")
        {
            stats.HealPlayer(5);
            player.GetComponent<PostProcessingController>().StartVodkaEffect(30f);
            stats.Slow(30f, 5f);
        }
        else if(itemId == "AID")
        {
            stats.HealPlayer(1000);
        }
        else if (itemId == "Omega Serum")
        {
            stats.HealPlayer(30);
        }
        else if (itemId == "Alpha Serum")
        {
            stats.DamagePlayer(30);
        }
        else if (itemId == "Beta Serum")
        {
            stats.Slow(10f, 15f, true);
        }
        else if (itemId == "Muschrom")
        {
            stats.HealPlayer(10);
        }
    }




    [ServerRpc(RequireOwnership = false)]
    public void ConsumeItemServerRpc()
    {
        pih.ConsumeItem();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetItemParentServerRpc()
    {
        parentID.Value = 0;
        pih = null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateItemParentServerRpc(ulong id)
    {
        parentID.Value = id;
    }

    private void LateUpdate()
    {
        //if (!IsServer) return;
        if (parentID.Value != 0)
        {
            if (pih != null)
            {
                transform.SetPositionAndRotation(pih.handTransform.position, pih.handTransform.rotation);
            }
            else
            {
                try
                {
                    Transform parent = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(parentID.Value).transform;
                    pih = parent.GetComponent<PlayerItemHolder>();
                }
                catch
                {
                    pih = null;
                }
            }
        }
        else
        {
            if(IsHost && GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
            {
                SyncItemTransformClientRpc(transform.position, transform.rotation);
            }
        }
    }

    [ClientRpc]
    private void SyncItemTransformClientRpc(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
}
