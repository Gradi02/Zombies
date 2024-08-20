using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour, IDamage
{
    public EnemyStyleData styleData;

    private float hp = 100;
    [SerializeField] private EnemyAI ai;

    [SerializeField] private GameObject helmet, chestProt, hair, glass, leggings;
    [SerializeField] private Material[] hairMaterials;
    [SerializeField] private Material[] leggingsMaterials;



    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            hp = Random.Range(100, 120);
            SetEnemyStyleServerRpc();
        }
    }

    void Start()
    {
        RequestApplyEnemyStyleServerRpc();
    }

    public void TakeDamage(float amount)
    {
        RequestDamageEntityServerRpc(amount);
        ai.ReactionState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDamageEntityServerRpc(float amount)
    {
        hp -= amount;
        if(hp <= 0)
        {
            RequestKillEntityServerRpc();
        }
    }

    [ServerRpc]
    private void RequestKillEntityServerRpc()
    {
        GetComponent<EnemyAI>().DeathStateClientRpc();
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
            hp += 5;
        }
        else if (Random.Range(0, 100) < 30)
        {
            data.hasHelmet = true;
            hp += 30;
        }

        if (Random.Range(0, 100) < 10)
        {
            data.hasChestProt = true;
            hp += 100;
        }

        styleData = data;
    }

    [ClientRpc]
    private void ApplyEnemyStyleClientRpc(EnemyStyleData data_in)
    {
        EnemyStyleData data = data_in;

        helmet.SetActive(data.hasHelmet);
        chestProt.SetActive(data.hasChestProt);
        hair.SetActive(data.hasHair);
        glass.SetActive(data.hasGlass);

        if (data.hasHair) hair.GetComponent<SkinnedMeshRenderer>().material = hairMaterials[data.hairMaterialIndex];

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
