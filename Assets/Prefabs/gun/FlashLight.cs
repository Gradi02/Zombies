using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private float disableTime = 0.1f;

    private void OnEnable()
    {
        Invoke(nameof(DisableLight), disableTime);
    }

    private void DisableLight()
    {
        gameObject.SetActive(false);
    }
}
