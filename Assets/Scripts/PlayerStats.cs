using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    private NetworkVariable<float> health = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float Health {
        get { return health.Value; } 
        private set
        {
            if (health.Value != value)
            {
                health.Value = value;
                OnHealthChanged();
            }
        }
    }

    private float maxHealth = 100;
    [SerializeField] private Slider hpSlider;

    private void Start()
    {
        if (!IsOwner) return;

        hpSlider.maxValue = maxHealth;
        Health = maxHealth;
    }

    private void OnHealthChanged()
    {
        hpSlider.value = Health;
    }

    public void DamagePlayer(float dmg)
    {
        if (Health - dmg >= 0)
            Health -= dmg;
        else
            Health = 0;
    }
}
