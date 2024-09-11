using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemSpawnerZone : MonoBehaviour
{
    [SerializeField] private int itemsToSpawn = 5;
    [SerializeField] private List<SpawnAnchors> spawnAnchors = new();
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
        List<SpawnAnchors> anchorsCopy = new List<SpawnAnchors>(spawnAnchors);
        List<SpawnAnchors> anchorsCopy2 = new();
        List<GameObject> objectsList = new();
        List<GameObject> staticObjectsList = new();

        //Create List of all possible Items
        for (int i=0; i < itemsInZone.Count; i++)
        {
            int num = itemsInZone[i].chance;
            GameObject obj = itemsInZone[i].itemObject;
            for(int j=0; j < num; j++)
            {
                if (itemsInZone[i].isStatic)
                    staticObjectsList.Add(obj);
                else
                    objectsList.Add(obj);
            }
        }

        //Create List of all possible Anchors
        for (int j = 0; j < anchorsCopy.Count; j++)
        {
            if (anchorsCopy[j].isStatic)
            {
                anchorsCopy2.Add(anchorsCopy[j]);
                anchorsCopy.RemoveAt(j);
            }
        }

        if (anchorsCopy2.Count < itemsToSpawn && anchorsCopy.Count > 0)
        {
            int itemsLeft = itemsToSpawn - anchorsCopy2.Count;
            for (int j = 0; j < itemsLeft; j++)
            {
                int rand = Random.Range(0, anchorsCopy.Count);
                anchorsCopy2.Add(anchorsCopy[rand]);
                anchorsCopy.RemoveAt(rand);
            }
        }

        //Spawn Items
        for (int i = 0; i < itemsToSpawn; i++)
        {
            int posIdx = Random.Range(0, anchorsCopy2.Count);

            if (staticObjectsList.Count > 0)
            {
                int objIdx = Random.Range(0, staticObjectsList.Count);

                GameObject spawnedItem = Instantiate(staticObjectsList[objIdx], anchorsCopy2[posIdx].spawnTransform.position, anchorsCopy2[posIdx].spawnTransform.rotation);
                spawnedItem.GetComponent<NetworkObject>().Spawn();

                staticObjectsList.RemoveAt(objIdx);
            }
            else
            {
                int objIdx = Random.Range(0, objectsList.Count);

                GameObject spawnedItem = Instantiate(objectsList[objIdx], anchorsCopy2[posIdx].spawnTransform.position, anchorsCopy2[posIdx].spawnTransform.rotation);
                spawnedItem.GetComponent<NetworkObject>().Spawn();

                objectsList.RemoveAt(objIdx);
            }

            anchorsCopy2.RemoveAt(posIdx);
        }

        yield return null;
    }
}

[System.Serializable]
public class ItemsInZone
{
    public GameObject itemObject;
    public int chance;
    public bool isStatic;
}

[System.Serializable]
public class SpawnAnchors
{
    public Transform spawnTransform;
    public bool isStatic;
}
