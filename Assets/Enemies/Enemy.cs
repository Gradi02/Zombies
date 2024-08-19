using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour, IDamage
{
    public float hp = 100;
    [SerializeField] private EnemyAI ai;

    [SerializeField] private GameObject helmet, chestProt, hair, glass, leggings;
    [SerializeField] private Material[] hairMaterials;
    [SerializeField] private Material[] leggingsMaterials;


    void Awake()
    {
        hp = Random.Range(100, 120);
        SetEnemyStyle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) SetEnemyStyle();
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

    private void SetEnemyStyle()
    {
        helmet.SetActive(false);
        chestProt.SetActive(false);
        hair.SetActive(false);
        glass.SetActive(false);

        if (Random.Range(0, 100) < 50)
        {
            hair.SetActive(true);
            hair.GetComponent<SkinnedMeshRenderer>().material = hairMaterials[Random.Range(0, hairMaterials.Length)];
            hp += 5;
        }
        else if (Random.Range(0, 100) < 30)
        {
            helmet.SetActive(true);
            hp += 30;
        }

        if (Random.Range(0, 100) < 20)
        {
            glass.SetActive(true);
        }
        if (Random.Range(0, 100) < 10)
        {
            chestProt.SetActive(true);
            hp += 100;
        }

        SkinnedMeshRenderer[] legs = leggings.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material mat = leggingsMaterials[Random.Range(0, leggingsMaterials.Length)];
        foreach (var le in legs)
            le.material = mat;
    }
}
