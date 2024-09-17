using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class AIDGenerator : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject aidPref;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI canvaText;

    private bool repaired = false;
    private bool finished = false;
    private float regenTime = 60f, nextFinished = 0;

    public string GetInteractionText(PlayerItemHolder playerItemHolder = null)
    {
        if(repaired)
        {
            if(!finished)
            {
                return "Generator Is Working!";
            }

            return $"Press E To Collect {aidPref.GetComponent<ItemManager>().itemId}!";
        }

        return "AID Generator Is Broken!";
    }

    public void MakeInteraction(ulong clientId, PlayerItemHolder playerItemHolder = null)
    {
        if(repaired && finished)
        {
            ResetGeneratorServerRpc();
            GameObject aid = Instantiate(aidPref, spawnPoint.position, spawnPoint.rotation);
            aid.GetComponent<NetworkObject>().Spawn();
            playerItemHolder.CollectItem(aid);
            aid.GetComponent<ItemManager>().UpdateItemParentServerRpc(clientId);
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        if(repaired && !finished)
        {
            nextFinished -= Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if (repaired && !finished)
        {
            canvaText.color = Color.red;
            canvaText.text = $"Next AID In {nextFinished}s!";
        }

        if (!IsServer) 
            return;

        if (nextFinished < 0)
        {
            SetGenClientRpc(true);
            nextFinished = regenTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartAIDGeneratorServerRpc()
    {
        StartGenClientRpc();
        nextFinished = regenTime;
    }

    [ClientRpc]
    private void StartGenClientRpc()
    {
        repaired = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetGeneratorServerRpc()
    {
        SetGenClientRpc(false);
    }

    [ClientRpc]
    private void SetGenClientRpc(bool f)
    {
        finished = f;

        if(finished)
        {
            canvaText.color = Color.green;
            canvaText.text = "Collect AID To Start Producing Next One!";
        }
    }
}
