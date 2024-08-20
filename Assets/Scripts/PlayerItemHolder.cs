using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerItemHolder : MonoBehaviour
{
    private GameObject itemInHand;
    [SerializeField] private Transform handTransform;


    public void CollectItem(GameObject newItem)
    {
        if (newItem.transform.parent == null)
        {
            if (itemInHand != null)
            {
                DropItem();
            }

            itemInHand = newItem;
            itemInHand.GetComponent<NetworkObject>().TrySetParent(handTransform);
            itemInHand.GetComponent<Rigidbody>().freezeRotation = true;
            itemInHand.GetComponent<Rigidbody>().useGravity = false;
            itemInHand.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void DropItem()
    {
        itemInHand.GetComponent<NetworkObject>().TryRemoveParent();
        itemInHand.GetComponent<Rigidbody>().freezeRotation = false;
        itemInHand.GetComponent<Rigidbody>().useGravity = true;
        itemInHand.GetComponent<BoxCollider>().enabled = true;
        itemInHand = null;
    }
}
