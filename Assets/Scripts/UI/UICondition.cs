using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition mana;
    public Condition buff;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}
