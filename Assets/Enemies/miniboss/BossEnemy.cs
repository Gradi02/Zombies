using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;

public class BossEnemy : NetworkBehaviour, IDamage
{
    private float hp = 100;
    [SerializeField] private BossEnemyAI ai;

    private bool damaga = false, dead = false;

    private int day => NetworkGameManager.instance.currentDay;
    private float[] daysBonus =
    {
        0,
        40,
        100,
        150,
        200,
        300
    };

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            hp = Random.Range(600, 700) + daysBonus[day];
        }
    }

    void Start()
    {
        var ach = new Steamworks.Data.Achievement("ACH_WIN_ONE_GAME");
        ach.Clear();
    }

    public void TakeDamage(float amount)
    {
        damaga = true;
        RequestDamageEntityServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDamageEntityServerRpc(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            IsDeadClientRpc();
            RequestKillEntityServerRpc();
        }
    }

    [ServerRpc]
    private void RequestKillEntityServerRpc()
    {
        ai.DeathStateClientRpc();
    }

    [ClientRpc]
    private void IsDeadClientRpc()
    {
        dead = true;

        if (dead && damaga)
        {
            var ach = new Steamworks.Data.Achievement("ACH_WIN_ONE_GAME");
            if (!ach.State)
            {
                ach.Trigger();
            }
        }

        dead = false;
        damaga = false;
    }
}
