using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;

public class Enemy : NetworkBehaviour, IDamage
{
    public EnemyStyleData styleData;

    private float hp = 100;
    [SerializeField] private EnemyAI ai;

    [SerializeField] private GameObject helmet, chestProt, hair, glass, leggings;
    [SerializeField] private Material[] hairMaterials;
    [SerializeField] private Material[] leggingsMaterials;

    private bool damaga = false, dead = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            hp = Random.Range(150, 200);
            SetEnemyStyleServerRpc();
        }
    }

    void Start()
    {
        if(IsHost)
            RequestApplyEnemyStyleServerRpc();

        var ach = new Steamworks.Data.Achievement("ACH_WIN_ONE_GAME");
        ach.Clear();
    }

    public void TakeDamage(float amount)
    {
        damaga = true;
        RequestDamageEntityServerRpc(amount);
        ai.ReactionStateServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDamageEntityServerRpc(float amount)
    {
        hp -= amount;
        if(hp <= 0)
        {
            IsDeadClientRpc();
            RequestKillEntityServerRpc();
        }
    }

    [ServerRpc]
    private void RequestKillEntityServerRpc()
    {
        GetComponent<EnemyAI>().DeathStateClientRpc();
    }

    [ClientRpc]
    private void IsDeadClientRpc()
    {
        dead = true;

        if(dead && damaga)
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

    [ServerRpc]
    private void SetEnemyStyleServerRpc()
    {
        EnemyStyleData data = new EnemyStyleData();

        data.leggingsMaterialIndex = Random.Range(0, leggingsMaterials.Length);
        data.hasGlass = Random.Range(0, 100) < 20;

        if (Random.Range(0, 100) < 50)
        {
            data.hasHair = true;
            data.hairMaterialIndex = Random.Range(0, hairMaterials.Length);
            hp += 15;
        }
        else if (Random.Range(0, 100) < 30)
        {
            data.hasHelmet = true;
            hp += 80;
        }

        if (Random.Range(0, 100) < 10)
        {
            data.hasChestProt = true;
            hp += 200;
        }

        styleData = data;
    }

    [ClientRpc]
    private void ApplyEnemyStyleClientRpc(EnemyStyleData data_in)
    {
        EnemyStyleData data = data_in;

        if (data.hasHelmet) helmet.SetActive(true);
        else Destroy(helmet);

        if (data.hasChestProt) chestProt.SetActive(true);
        else Destroy(chestProt);

        if (data.hasHair)
        {
            hair.SetActive(true);
            hair.GetComponent<SkinnedMeshRenderer>().material = hairMaterials[data.hairMaterialIndex];
        }
        else Destroy(hair);

        if(data.hasGlass) glass.SetActive(true);
        else Destroy (glass);

        SkinnedMeshRenderer[] legs = leggings.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material mat = leggingsMaterials[data.leggingsMaterialIndex];
        foreach (var le in legs)
            le.material = mat;


    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestApplyEnemyStyleServerRpc()
    {
        ApplyEnemyStyleClientRpc(styleData);
    }
}


[System.Serializable]
public class EnemyStyleData : INetworkSerializable
{
    public bool hasHelmet = false;
    public bool hasChestProt = false;
    public bool hasHair = false;
    public bool hasGlass = false;
    public int hairMaterialIndex;
    public int leggingsMaterialIndex;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref hasHelmet);
        serializer.SerializeValue(ref hasChestProt);
        serializer.SerializeValue(ref hasHair);
        serializer.SerializeValue(ref hasGlass);
        serializer.SerializeValue(ref hairMaterialIndex);
        serializer.SerializeValue(ref leggingsMaterialIndex);
    }
}
