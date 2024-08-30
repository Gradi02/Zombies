using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Upgrader : NetworkBehaviour, IInteractable
{
    //[SerializeField] private GameObject canvaUpgr;
    private GunUpgrade currentUpgrade = null;

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        Debug.Log("i" + currentUpgrade);
        playerItemHolder.GetComponent<PlayerShooting>().OpenUpgradeCanva(currentUpgrade);
    }

    [ServerRpc(RequireOwnership = false), ContextMenu("new upgr")]
    private void SetNewUpgradeServerRpc()
    {
        SetNewUpgradeClientRpc();
    }

    [ClientRpc]
    private void SetNewUpgradeClientRpc()
    {
        currentUpgrade = new GunUpgrade(Random.Range(1, 10), Random.Range(0, 1));
    }
}
