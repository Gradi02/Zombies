using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;

public class PlayerItemHolder : NetworkBehaviour
{
    public GameObject itemInHand { get; private set; } = null;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float dropPower = 3f;
    [SerializeField] private Transform head;
    [SerializeField] private TwoBoneIKConstraint itemHandConstraint;

    private void Start()
    {
        if (!IsOwner) return;

        SetHandConstraintWeightServerRpc(0);
    }
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
            SetHandConstraintWeightServerRpc(1);
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
        SetHandConstraintWeightServerRpc(0);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetHandConstraintWeightServerRpc(int w)
    {
        SetHandConstraintWeightClientRpc(w);
    }

    [ClientRpc]
    private void SetHandConstraintWeightClientRpc(int w)
    {
        itemHandConstraint.weight = w;
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        if(itemInHand != null)
        {
            ChangePosRotItemServerRpc(handTransform.position, handTransform.rotation);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePosRotItemServerRpc(Vector3 pos, Quaternion rot)
    {
        itemInHand.transform.SetPositionAndRotation(pos, rot);
    }
}

