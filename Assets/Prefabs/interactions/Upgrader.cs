using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Upgrader : NetworkBehaviour, IInteractable
{
    public List<GunUpgrade> upgrades = new List<GunUpgrade>();

    [SerializeField] private GameObject noUpgr, newUpgr;
    [SerializeField] private TextMeshProUGUI upgrTitle, upgrCost;
    [SerializeField] private Image upgrImage;

    private GunUpgrade currentUpgrade = null;
    private int currentUpgradeCost = 0;

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        PlayerStats stats = playerItemHolder.GetComponent<PlayerStats>();
        if (currentUpgrade != null && stats.gold >= currentUpgradeCost && stats.mUpgrades.Count < stats.maxUpgrades)
        {
            stats.BuyUpgrade(currentUpgrade, currentUpgradeCost);
            ResetUpgradeServerRpc();
        }
    }
    public string GetInteractionText()
    {
        return "Press E To Buy Upgrade!";
    }

    // SET UPGRADE  

    [ServerRpc(RequireOwnership = false), ContextMenu("new upgr")]
    public void SetNewUpgradeServerRpc()
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

        upgrTitle.text = currentUpgrade.upgradeTitle;
        upgrCost.text = currentUpgradeCost + " coins";
        upgrImage.sprite = currentUpgrade.image;
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
