using System;
using UnityEngine;

public enum ItemType
{
    Equipable,
    AddAbility,
    Consumable,
    Resource
}

public enum ConsumableType
{
    Health,
    Mana
}

public enum AdditionalAbility
{
    _NONE,
    SpeedUp
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Ability")]
    public AdditionalAbility ability;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}