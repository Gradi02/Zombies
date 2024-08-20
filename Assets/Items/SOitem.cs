using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Item", menuName = "ScriptableObjects/new Item", order = 1)]
public class SOitem : ScriptableObject
{
    public string itemName;
    public GameObject itemModel;
}
