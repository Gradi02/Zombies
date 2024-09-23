using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ThrowCollision : NetworkBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private float damageRadius = 10;
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsHost) return;

        if(collision.collider.gameObject.layer == 6 || collision.collider.gameObject.layer == 8)
        {
            ImpactParticleClientRpc();                      
            foreach(var s in NetworkManager.Singleton.ConnectedClients)
            {
                GameObject obj = s.Value.PlayerObject.gameObject;

                Debug.Log(Vector3.Distance(obj.transform.position, transform.position));
                if(obj != null && Vector3.Distance(obj.transform.position, transform.position) <= damageRadius)
                    obj.GetComponent<PlayerStats>().DamagePlayerServerRpc(80);
            }
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void ImpactParticleClientRpc()
    {
        ParticleSystem psHolder = Instantiate(ps, transform.position, Quaternion.identity);
        Destroy(psHolder, 5);
    }
}
