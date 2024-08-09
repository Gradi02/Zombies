using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraMovement : NetworkBehaviour
{
	public float SensX;
	public float SensY;

	public GameObject cam;

	float xRotation;
	float yRotation;

	private void Start()
	{
		if (!IsOwner)
		{
			cam.SetActive(false);
			return;
		}

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	private void Update()
	{
		if (!IsOwner) return;
		float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SensX;
		float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SensY;

		yRotation += mouseX;
		xRotation -= mouseY;

		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, transform.rotation.z);
		transform.rotation = Quaternion.Euler(0, yRotation, 0);
	}
}
