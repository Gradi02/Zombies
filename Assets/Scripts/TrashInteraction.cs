using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TrashInteraction : NetworkBehaviour, IInteractable
{
    [SerializeField] private ItemManager itemToClean;
    [SerializeField] private GameObject objToDestroy;
    [SerializeField] private ParticleSystem particle;
    private bool clearable = false;
    private bool interacted = false, cleaning = false;
    private float timeToClean = 4f, cleanedTime = 0;
    private PlayerItemHolder reviver;
    public LayerMask interactionLayer;

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if (playerItemHolder != null && playerItemHolder.itemInHand != null &&
            playerItemHolder.itemInHand.GetComponent<ItemManager>().itemId == itemToClean.itemId)
        {
            clearable = true;

            if(!cleaning && !interacted)
                return "Hold E To Clean";

            return "Cleaning: " + cleanedTime.ToString("F2") + "s";
        }

        clearable = false;
        return $"Use {itemToClean.itemId} To Clean";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(clearable)
        {
            cleanedTime = timeToClean;
            reviver = playerItemHolder;
            interacted = true;
        }
    }

    void Update()
    {
        if (interacted)
        {
            RaycastHit hit;
            Camera cam = reviver.cam;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Physics.Raycast(ray, out hit, PlayerInteraction.interactDistance, interactionLayer);

            cleaning = Input.GetKey(PlayerInteraction.interactKey) && hit.collider != null && hit.collider.gameObject == this.gameObject;

            if (!cleaning || Input.GetKeyUp(PlayerInteraction.interactKey))
            {
                cleaning = false;
                interacted = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (cleaning)
        {
            cleanedTime -= Time.fixedDeltaTime;

            if (cleanedTime <= 0)
            {
                cleaning = false;
                interacted = false;
                RequestCleanTrashServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCleanTrashServerRpc()
    {
        HalfDestroyClientRpc();
        Invoke(nameof(RequestDestroyServerRpc), 2f);
    }

    [ClientRpc]
    private void HalfDestroyClientRpc()
    {
        objToDestroy.SetActive(false);
        particle.Play();
    }

    [ServerRpc]
    private void RequestDestroyServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(this.gameObject);
    }
}
