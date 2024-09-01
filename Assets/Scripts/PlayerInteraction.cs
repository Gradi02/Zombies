using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class PlayerInteraction : NetworkBehaviour
{
    public Camera cam;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode dropItemKey = KeyCode.Q;
    public float interactDistance = 5;
    public LayerMask interactionLayer, grabLootMask;

    public PlayerItemHolder playerItemHolder;
    public PlayerStats stats;
    public Image crosshair;

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        if(!IsOwner)
        {
            crosshair.transform.parent.gameObject.SetActive(false);
            return;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        InteractionCheck();
        HandleDropItem();
    }

    private void InteractionCheck()
    {
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, interactDistance, interactionLayer))
        {
            crosshair.color = Color.red;

            if (Input.GetKeyDown(interactKey))
            {
                try
                {
                    IInteractable inter = hit.collider.GetComponent<IInteractable>();
                    ulong id = NetworkManager.Singleton.LocalClientId;
                    inter.MakeInteraction(id, playerItemHolder);
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                    Debug.LogWarning("You try to interact with object that dont have IInteractable interface! Make sure that this object should be in interaction Layer!");
                }
            }
        }
        else if (Physics.Raycast(ray, out hit, interactDistance, grabLootMask))
        {
            Transform e = hit.collider.transform.root;

            if (e.GetComponent<EnemyAI>().isDead && !e.GetComponent<DeadEnemyManager>().searched)
            {
                crosshair.color = Color.blue;

                if (Input.GetKeyDown(interactKey))
                {
                    hit.collider.transform.root.GetComponent<DeadEnemyManager>().SearchUp(stats);
                }
            }
        }
        else
        {
            crosshair.color = Color.white;
        }
    }

    private void HandleDropItem()
    {
        if(Input.GetKeyDown(dropItemKey) && playerItemHolder.itemInHand != null)
        {
            playerItemHolder.DropItem();
        }
    }
}
