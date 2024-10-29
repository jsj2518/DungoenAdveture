using System;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(float damage);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [HideInInspector] public UICondition uiCondition;

    private Condition health { get { return uiCondition.health; } }
    private Condition mana { get { return uiCondition.mana; } }
    private Condition buff { get { return uiCondition.buff; } }

    private bool buffActivated;

    public event Action OnTakeDamage;

    private void Update()
    {
        health.Add(health.passiveValue * Time.deltaTime);
        mana.Add(mana.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("You Died");
    }


    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        mana.Add(amount);
    }

    public void TakePhysicalDamage(float damage)
    {
        health.Subtract(damage);
        OnTakeDamage?.Invoke();
    }

    public bool UseMana(float amount)
    {
        if (mana.curValue - amount < 0f)
        {
            return false;
        }

        mana.Subtract(amount);
        return true;
    }
}
