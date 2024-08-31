using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Upgrader : NetworkBehaviour, IInteractable
{
    public List<GunUpgrade> upgrades = new List<GunUpgrade>();

    [SerializeField] private GameObject noUpgr, newUpgr;
    [SerializeField] private TextMeshProUGUI upgrText;

    private GunUpgrade currentUpgrade = null;
    private int currentUpgradeCost = 0;

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if (currentUpgrade != null && playerItemHolder.GetComponent<PlayerStats>().gold >= currentUpgradeCost)
        {
            playerItemHolder.GetComponent<PlayerStats>().AddRemoveGold(currentUpgradeCost, true);
            playerItemHolder.GetComponent<PlayerShooting>().OpenUpgradeCanva(currentUpgrade);
            ResetUpgradeServerRpc();
        }
    }



    // SET UPGRADE  

    [ServerRpc(RequireOwnership = false), ContextMenu("new upgr")]
    private void SetNewUpgradeServerRpc()
    {
        int idx = Random.Range(0, upgrades.Count);
        currentUpgradeCost = Random.Range(upgrades[idx].minCost, upgrades[idx].maxCost) + NetworkGameManager.instance.currentDay * 10;
        SetNewUpgradeClientRpc(idx, currentUpgradeCost);
    }

    [ClientRpc]
    private void SetNewUpgradeClientRpc(int idx, int cuc)
    {
        currentUpgradeCost = cuc;
        currentUpgrade = upgrades[idx];

        newUpgr.SetActive(true);
        noUpgr.SetActive(false);

        upgrText.text = "Upgrade:\n+" + currentUpgrade.value + " " + currentUpgrade.upgrade.ToString() + "\nCost: " + currentUpgradeCost;
    }

    // RESET UPGRADE

    [ServerRpc(RequireOwnership = false)]
    private void ResetUpgradeServerRpc()
    {
        ResetUpgradeClientRpc();
    }

    [ClientRpc]
    private void ResetUpgradeClientRpc()
    {
        currentUpgrade = null;
        newUpgr.SetActive(false);
        noUpgr.SetActive(true);
    }
}
