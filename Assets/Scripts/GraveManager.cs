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
    public string GetInteractionText()
    {
        return "Hold E To Revive Your Friend!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(playerItemHolder.GetComponent<PlayerStats>().isAlive.Value)
        {
            timeLeft = timeToRevive;
            interacted = true;
        }
    }

    void Update()
    {
        if(interacted)
        {
            reviving = Input.GetKey(PlayerInteraction.interactKey);

            if(Input.GetKeyUp(PlayerInteraction.interactKey))
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
            Debug.Log(timeLeft);

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
