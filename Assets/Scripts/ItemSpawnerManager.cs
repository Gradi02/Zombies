using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSpawnerManager : NetworkBehaviour
{
    [SerializeField] private ItemSpawnerZone[] zones;

    [ContextMenu("spawn")]
    public void SpawnItemsInAllZones()
    {
        foreach(var zone in zones)
        {
            zone.SpawnItemsInZone();
        }
    }
}



