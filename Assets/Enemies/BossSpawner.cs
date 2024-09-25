using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BossSpawner : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject mutant;
    private float timeToActive = 3f;
    private float currentTime;
    private bool spawning = false, interacted = false;
    private PlayerItemHolder reviver;
    public LayerMask interactionLayer;

    public BossItem[] bossItems;
    [SerializeField] private Transform spawnPoint;
    private GameObject itemToDestroy, itemPref = null;

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if (!spawning && !interacted)
        {
            return "Hold E To Summon Mutant!";
        }

        return "Summoning Mutant: " + currentTime.ToString("F2") + "s";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if (!interacted)
        {
            currentTime = timeToActive;
            interacted = true;
            reviver = playerItemHolder;
        }
    }

    void Update()
    {
        if (interacted)
        {
            RaycastHit hit;
            Camera cam = reviver.cam;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Physics.Raycast(ray, out hit, PlayerInteraction.interactDistance, interactionLayer);

            spawning = Input.GetKey(PlayerInteraction.interactKey) && hit.collider != null && hit.collider.gameObject == this.gameObject;

            if (!spawning || Input.GetKeyUp(PlayerInteraction.interactKey))
            {
                spawning = false;
                interacted = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (spawning)
        {
            currentTime -= Time.fixedDeltaTime;

            if (currentTime <= 0)
            {
                spawning = false;
                interacted = false;
                RequestSummonBossServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSummonBossServerRpc()
    {
        GameObject g = Instantiate(mutant, transform.position, Quaternion.identity);
        g.GetComponent<NetworkObject>().Spawn();
        NetworkGameManager.instance.AddEnemyToList(g.GetComponent<BossEnemyAI>());

        g.GetComponent<BossEnemyAI>().rewardItem = itemPref;
        if (itemToDestroy != null)
        {
            itemToDestroy.GetComponent<NetworkObject>().Despawn();
            Destroy(itemToDestroy);
        }

        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetStatueItemServerRpc(int num)
    {
        itemToDestroy = Instantiate(bossItems[num].itemPrefab, spawnPoint.position + bossItems[num].spawnOffset, Quaternion.identity);
        itemToDestroy.GetComponent<NetworkObject>().Spawn();
        itemToDestroy.GetComponent<ItemManager>().ChangeItemStateClientRpc(false);
        itemPref = bossItems[num].itemPrefab;
    }
}
