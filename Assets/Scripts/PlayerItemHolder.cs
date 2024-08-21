using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerItemHolder : NetworkBehaviour
{
    public GameObject itemInHand { get; private set; } = null;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float dropPower = 3f;
    [SerializeField] private Transform head;


    public void CollectItem(GameObject newItem)
    {
        if (newItem.transform.parent == null)
        {
            if (itemInHand != null)
            {
                DropItem();
            }

            itemInHand = newItem;
            itemInHand.GetComponent<Rigidbody>().freezeRotation = true;
            itemInHand.GetComponent<Rigidbody>().useGravity = false;
            itemInHand.GetComponent<BoxCollider>().enabled = false;
            itemInHand.transform.localPosition = Vector3.zero;
        }
    }

    public void DropItem()
    {
        itemInHand.GetComponent<ItemManager>().ResetItemParent();
        itemInHand.GetComponent<BoxCollider>().enabled = true;

        Rigidbody rb = itemInHand.GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.velocity = GetComponent<CharacterController>().velocity;
        rb.AddForce(head.forward * dropPower, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(0.0f, 0.01f), Random.Range(0.0f, 0.01f), Random.Range(0.0f, 0.01f)) * dropPower, ForceMode.Impulse);

        itemInHand = null;
    }

    private void LateUpdate()
    {
        if(itemInHand != null)
        {
            itemInHand.transform.position = handTransform.position;
            itemInHand.transform.rotation = handTransform.rotation;
        }
    }
}

