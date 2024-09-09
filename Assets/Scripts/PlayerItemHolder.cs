using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;

public class PlayerItemHolder : NetworkBehaviour
{
    public GameObject itemInHand { get; private set; } = null;
    public Transform handTransform;
    [SerializeField] private float dropPower = 3f;
    [SerializeField] private Transform head;
    [SerializeField] private TwoBoneIKConstraint itemHandConstraint;
    private KeyCode useItemButton = KeyCode.Mouse1;

    private void Start()
    {
        if (!IsOwner) return;

        SetHandConstraintWeightServerRpc(0);
    }
    public void CollectItem(GameObject newItem)
    {
        if (itemInHand != null)
        {
            DropItem();
        }

        itemInHand = newItem;
        CollectItemServerRpc(itemInHand.GetComponent<NetworkObject>().NetworkObjectId);

        SetHandConstraintWeightServerRpc(1);
    }

    public void DropItem()
    {
        DropItemServerRpc(itemInHand.GetComponent<NetworkObject>().NetworkObjectId, GetComponent<CharacterController>().velocity, head.forward);
        itemInHand.GetComponent<ItemManager>().ResetItemParentServerRpc();

        itemInHand = null;
        SetHandConstraintWeightServerRpc(0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CollectItemServerRpc(ulong id)
    {
        Transform item = NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].transform;

        item.GetComponent<Rigidbody>().freezeRotation = true;
        item.GetComponent<Rigidbody>().useGravity = false;

        if (item.GetComponent<BoxCollider>() != null)
            item.GetComponent<BoxCollider>().enabled = false;
        else if (item.GetComponent<SphereCollider>() != null)
            item.GetComponent<SphereCollider>().enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(ulong id, Vector3 vel, Vector3 headf)
    {
        Transform item = NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].transform;


        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.velocity = vel;
        rb.AddForce(headf * dropPower, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(0.0f, 0.01f), Random.Range(0.0f, 0.01f), Random.Range(0.0f, 0.01f)) * dropPower, ForceMode.Impulse);

        if (item.GetComponent<BoxCollider>() != null)
            item.GetComponent<BoxCollider>().enabled = true;
        else if (item.GetComponent<SphereCollider>() != null)
            item.GetComponent<SphereCollider>().enabled = true;
    }

    public void ConsumeItem()
    {
        itemInHand.GetComponent<ItemManager>().ResetItemParentServerRpc();
        DestroyItemServerRpc(itemInHand.GetComponent<NetworkObject>().NetworkObjectId); 

        itemInHand = null;
        SetHandConstraintWeightServerRpc(0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyItemServerRpc(ulong id)
    {
        Transform item = NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].transform;
        item.GetComponent<NetworkObject>().Despawn();
        Destroy(item);
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

    private void Update()
    {
        if (!IsOwner) return;

        if(itemInHand != null && Input.GetKeyDown(useItemButton))
        {
            ItemManager mng = itemInHand.GetComponent<ItemManager>();

            if (mng.usable)
            {
                if (mng.dmgToUse && GetComponent<PlayerStats>().Health == GetComponent<PlayerStats>().maxHealth)
                {
                    return;
                }

                mng.ConsumeEffect(this);
                ConsumeItem();

            }
        }
    }
}

