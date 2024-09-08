using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSpawnerZone : MonoBehaviour
{
    [SerializeField] private int itemsToSpawn = 5;
    [SerializeField] private List<Transform> spawnAnchors = new();
    [SerializeField] private List<ItemsInZone> itemsInZone = new();

    public void SpawnItemsInZone()
    {
        if(spawnAnchors == null || spawnAnchors.Count == 0)
        {
            Debug.LogWarning("Zone '" + gameObject.name + "' don't have declared any spawn anchors! Items won't be spawned in this zone!");
            return;
        }

        StartCoroutine(IESpawnItemsInZone());
    }

    private IEnumerator IESpawnItemsInZone()
    {
        List<Transform> anchorsCopy = new List<Transform>(spawnAnchors);
        List<GameObject> objectsList = new();

        for(int i=0; i < itemsInZone.Count; i++)
        {
            int num = itemsInZone[i].chance;
            GameObject obj = itemsInZone[i].itemObject;
            for(int j=0; j < num; j++)
            {
                objectsList.Add(obj);
            }
        }

        for (int i = 0; i < itemsToSpawn; i++)
        {
            int posIdx = Random.Range(0, anchorsCopy.Count);
            int objIdx = Random.Range(0, objectsList.Count);

            GameObject spawnedItem = Instantiate(objectsList[objIdx], anchorsCopy[posIdx].position, anchorsCopy[posIdx].rotation);
            spawnedItem.GetComponent<NetworkObject>().Spawn();

            objectsList.RemoveAt(objIdx);
            anchorsCopy.RemoveAt(posIdx);
        }

        yield return null;
    }
}

[System.Serializable]
public class ItemsInZone
{
    public GameObject itemObject;
    public int chance;
}
