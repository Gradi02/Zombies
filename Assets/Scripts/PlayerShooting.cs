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
	public Camera cam;

	[Header("Gun Stats")]
	private Animator animator;
	[SerializeField] private AnimationClip shoot, reload;
	private float damage = 50;
	private int Ammo = 8;
	private float ReloadTime = -1;

    private void Awake()
    {
		animator = gun.GetComponent<Animator>();
	}

    void Update()
    {
		if (!IsOwner && cam != null) return;

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		Debug.DrawRay(ray.origin, ray.direction * 100);

        if (Ammo <= 0)
        {
			animator.SetTrigger("reload");
            Ammo = 8;
        }

        if (Input.GetMouseButtonDown(0))
		{
			animator.SetTrigger("shoot");
			Ammo --;

			if (Time.time >= nextFireTime)
			{
				nextFireTime = Time.time + Cooldown;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider.CompareTag("head"))
					{
						// hit
						Debug.Log("HIT - head");
						hit.collider.transform.root.GetComponent<IDamage>().TakeDamage(damage * 2);

						// hit particle
						ParticleSystem ps = Instantiate(PS_blood, hit.point, Quaternion.LookRotation(hit.normal));
						Destroy(ps.gameObject, 2);
					}
					else if(hit.collider.CompareTag("body"))
                    {
						// hit
						Debug.Log("HIT - body");
						hit.collider.transform.root.GetComponent<IDamage>().TakeDamage(damage);

						// hit particle
						ParticleSystem ps = Instantiate(PS_blood, hit.point, Quaternion.LookRotation(hit.normal));
						Destroy(ps.gameObject, 2);
					}
					else if (hit.collider.CompareTag("legs"))
					{
						// hit
						Debug.Log("HIT - legs");
						hit.collider.transform.root.GetComponent<IDamage>().TakeDamage(damage * 0.5f);

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
