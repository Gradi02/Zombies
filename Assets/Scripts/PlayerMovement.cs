using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    private float Hmove, Vmove;
    private Rigidbody rb;
    private Vector3 dir;
    public float speed;

    private void Start()
    {
        if (!IsOwner) return;

        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!IsOwner) return;
        rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * speed) + (transform.right * Input.GetAxis("Horizontal") * speed));
    }
}
