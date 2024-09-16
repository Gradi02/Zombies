using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GraveManager : NetworkBehaviour, IInteractable
{
    public ulong deadPlayerId { get; set; }
    private float timeToRevive = 10;
    private float timeLeft = 10;
    private bool interacted = false, reviving = false;
    private PlayerItemHolder reviver;
    public LayerMask interactionLayer;
    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if(!reviving && !interacted)
        {
            return "Hold E To Revive Your Friend!";
        }

        return "Reviving Friend: " + timeLeft.ToString("F2") + "s";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(playerItemHolder.GetComponent<PlayerStats>().isAlive.Value)
        {
            timeLeft = timeToRevive;
            interacted = true;
            reviver = playerItemHolder;
        }
    }

    void Update()
    {
        if(interacted)
        {
            RaycastHit hit;
            Camera cam = reviver.cam;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Physics.Raycast(ray, out hit, PlayerInteraction.interactDistance, interactionLayer);

            reviving = Input.GetKey(PlayerInteraction.interactKey) && hit.collider != null && hit.collider.gameObject == this.gameObject;

            if (!reviving || Input.GetKeyUp(PlayerInteraction.interactKey))
            {
                reviving = false;
                interacted = false;
            }
        }
    }

    void FixedUpdate()
    {
        if(reviving)
        {
            timeLeft -= Time.fixedDeltaTime;

            if(timeLeft <= 0)
            {
                reviving = false;
                interacted = false;
                RequestRevivePlayerServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRevivePlayerServerRpc()
    {
        NetworkGameManager.instance.HandlePlayerReviveServerRpc(deadPlayerId);
        GetComponent<NetworkObject>().Despawn();
        Destroy(this.gameObject);
    }
}
