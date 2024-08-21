using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.VFX;

public class PlayerShooting : NetworkBehaviour
{
	public ParticleSystem PS_blood;
	public ParticleSystem PS_other;
	public GameObject gun;
	private float Cooldown = 0.3f;
	private float nextFireTime = 0f;
	private float reloadingTime = 3f;
	public Camera cam;
	[SerializeField] private TextMeshProUGUI ammoText;
	[SerializeField] private VisualEffect shootParticle;
	[SerializeField] private GameObject flashlight;

	[Header("Gun Stats")]
	private Animator animator;
	[SerializeField] private AnimationClip shoot, reload;
	private float damage = 50;
	private int Ammo = 8;
	public LayerMask obstacleMask;


	private void Awake()
	{
		animator = gun.GetComponent<Animator>();
	}

	void Update()
	{
		if (!IsOwner && cam != null) return;

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		if (Ammo <= 0 && Time.time > nextFireTime - 0.1f)
		{
			ammoText.text = "reloading";
			nextFireTime = Time.time + reloadingTime;
			CancelInvoke();
			animator.CrossFade(reload.name, 0.2f);
			Ammo = 8;
		}

		if (Time.time >= nextFireTime)
		{
			ammoText.text = "ammo: " + Ammo;
			if (Input.GetMouseButtonDown(0))
			{
				nextFireTime = Time.time + Cooldown;

				Ammo--;
				ammoText.text = "ammo: " + Ammo;
				RequestShootParticleServerRpc();
				Invoke(nameof(PlayShootAnimDelayed), 0.15f);
				AlarmNearEnemies();

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, obstacleMask))
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
					else if (hit.collider.CompareTag("body"))
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
					else if (hit.collider.CompareTag("protection"))
					{
						// hit
						Debug.Log("HIT - PROT");

						if (hit.collider.GetComponent<Renderer>() != null)
						{
							ParticleSystem ps = Instantiate(PS_blood, hit.point, Quaternion.LookRotation(hit.normal));
							ps.GetComponent<Renderer>().material = hit.collider.GetComponent<Renderer>().material;
							Destroy(ps.gameObject, 2);
						}
					}
					else
					{
						if (hit.collider.GetComponent<Renderer>() != null)
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

	private void PlayShootAnimDelayed()
	{
		animator.CrossFade(shoot.name, 0.2f);
	}

	[ServerRpc]
	private void RequestShootParticleServerRpc()
	{
		DisplayShootParticleClientRpc();
	}

	[ClientRpc]
	private void DisplayShootParticleClientRpc()
    {
		shootParticle.Play();
		flashlight.SetActive(true);
	}

	public LayerMask enemyLayer;
	private void AlarmNearEnemies()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 400, enemyLayer);

		foreach (Collider hitCollider in hitColliders)
		{
			EnemyAI enemyScript = hitCollider.GetComponent<EnemyAI>();

			if (enemyScript != null)
			{
				enemyScript.AlarmEnemy(transform.position);
			}
		}
	}
}
