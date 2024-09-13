using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    private Transform Target;
    public Vector3 Offset;
    public Vector3 Rotation;
    public float SmoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        if (NetworkManager.Singleton != null &&
           NetworkManager.Singleton.SpawnManager != null &&
           NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null)
        {
            Target = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
        }

        if (Target == null) return;
        Vector3 targetPosition = Target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
        transform.rotation = Quaternion.Euler(Rotation);
    }   
}
