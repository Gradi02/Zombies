using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    private float health;
    public float Health {
        get { return health; } 
        private set
        {
            if (health != value)
            {
                health = value;
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
    void Update()
    {
        if (!IsOwner) return;
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
