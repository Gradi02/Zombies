using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "new Upgrade", menuName = "ScriptableObjects/new Upgrade")]
public class GunUpgrade : ScriptableObject
{
    public Sprite image;
    public int minCost, maxCost;
    public Upgrades upgrade;
    public int value;
}

public enum Upgrades
{
    maxHealth,
    maxAmmo,
    baseDamage
}
