using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    private float Hmove, Vmove;
    private Rigidbody rb;
    private Vector3 dir;
    private float speed = 0.1f;
    private Camera cam;

    private void Start()
    {
        if (!IsOwner) return;
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!IsOwner) return;
        //rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * speed) + (transform.right * Input.GetAxis("Horizontal") * speed));
        Hmove = Input.GetAxis("Horizontal");
        Vmove = Input.GetAxis("Vertical");
        //sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vmove *= 1.5f;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector3 movement = transform.right * Hmove * speed + transform.forward * Vmove * speed;
        rb.MovePosition(rb.position + movement);
    }


}
