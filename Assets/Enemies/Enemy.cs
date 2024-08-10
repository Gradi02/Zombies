using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    public int health = 100;
    public GameObject leftArm, rightArm;
    public GameObject leftLeg, rightLeg;
    public GameObject head;


    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        Destroy(gameObject);
    }
}
