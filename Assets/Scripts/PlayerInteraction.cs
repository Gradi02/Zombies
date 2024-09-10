using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using TMPro;

public class PlayerInteraction : NetworkBehaviour
{
    public Camera cam;
    public static KeyCode interactKey = KeyCode.E;
    public static KeyCode dropItemKey = KeyCode.Q;
    public static float interactDistance = 5;
    public LayerMask interactionLayer, grabLootMask;

    public PlayerItemHolder playerItemHolder;
    public PlayerStats stats;
    public Image crosshair;
    public TextMeshProUGUI interactText;

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
            interactText.gameObject.SetActive(true);
            IInteractable inter;
            
            if(hit.collider.transform.root.CompareTag("enemy"))
            {
                Transform e = hit.collider.transform.root;
                inter = e.GetComponent<IInteractable>();
            }
            else
            {
                inter = hit.collider.GetComponent<IInteractable>();
            }

            if(inter != null)
                interactText.text = inter.GetInteractionText();

            if (Input.GetKeyDown(interactKey))
            {
                try
                {
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
        else
        {
            crosshair.color = Color.white;
            interactText.gameObject.SetActive(false);
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
