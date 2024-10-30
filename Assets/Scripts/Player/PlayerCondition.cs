using System;
using UnityEngine;
using UnityEngine.UI;

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

    public AdditionalAbility buffActivated = AdditionalAbility._NONE;
    private AdditionalAbility buffNew = AdditionalAbility._NONE;
    public event Action<AdditionalAbility> OnEndBuff;

    public event Action OnTakeDamage;

    private void Update()
    {
        health.Add(health.passiveValue * Time.deltaTime);
        mana.Add(mana.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }

        UpdateBuff();
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

    public void ActivateBuff(AdditionalAbility buff)
    {
        buffNew = buff;
    }
    private void UpdateBuff()
    {
        if (buffActivated == AdditionalAbility._NONE && buffNew != AdditionalAbility._NONE)
        {
            buffActivated = buffNew;
            buffNew = AdditionalAbility._NONE;

            buff.curValue = buff.maxValue;
        }
        else if (buffActivated != AdditionalAbility._NONE)
        {
            buff.Add(buff.passiveValue * Time.deltaTime);

            if (buff.curValue <= 0f)
            {
                OnEndBuff?.Invoke(buffActivated);
                buffActivated = AdditionalAbility._NONE;
            }
        }

        uiCondition.UpdateBuff(buffActivated);
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
