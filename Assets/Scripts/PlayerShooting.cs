using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    public ParticleSystem PS_blood;
    public ParticleSystem PS_other;
    public GameObject gun;
    private float Cooldown = 0.2f;
    private float nextFireTime = 0f;


    void Update()
    {
		if (!IsOwner) return;

		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		Debug.DrawRay(ray.origin, ray.direction * 100);

		if (Input.GetMouseButtonDown(0))
		{
			if (Time.time >= nextFireTime)
			{
				nextFireTime = Time.time + Cooldown;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider.CompareTag("enemy"))
					{
						// hit
						Debug.Log("HIT");
						hit.collider.gameObject.GetComponent<IDamage>().TakeDamage(25);

						// hit particle
						ParticleSystem ps = Instantiate(PS_blood, hit.point, Quaternion.LookRotation(hit.normal));
						Destroy(ps.gameObject, 2);
					}
					else
					{
						ParticleSystem ps = Instantiate(PS_other, hit.point, Quaternion.LookRotation(hit.normal));
						ps.GetComponent<Renderer>().material = hit.collider.GetComponent<Renderer>().material;
						Destroy(ps.gameObject, 2);
					}
				}
			}
		}
	}
}
