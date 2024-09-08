using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeadEnemyManager : NetworkBehaviour, IInteractable
{
    public bool searched { get; private set; } = false;
    private float timeToRemove = 0;

    [SerializeField] private int minGold = 1, maxGold = 5;
    [SerializeField] private float removingTime = 50;
    [SerializeField] private NetworkObject nobj;
    [SerializeField] private EnemyAI ai;

    void Update()
    {
        if (!IsServer || !ai.isDead) return;

        timeToRemove += Time.deltaTime;
        if(timeToRemove > removingTime)
        {
            nobj.Despawn();
            Destroy(nobj.gameObject);
        }
    }

    public void SearchUp(PlayerStats pst)
    {
        if(!searched)
        {
            searched = true;
            pst.AddRemoveGold(Random.Range(minGold, maxGold));
            ai.ResetDeathLayer();
        }
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(ai.isDead && !searched)
        {
            SearchUp(playerItemHolder.GetComponent<PlayerStats>());
        }
    }

    public string GetInteractionText()
    {
        return "Press E To Search Body!";
    }
}
