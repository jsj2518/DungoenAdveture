using UnityEngine;
using UnityEngine.UI;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition mana;
    public Condition buff;

    public Image buffIcon;
    public Image buffDuration;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
        buffIcon = buff.transform.GetChild(0).GetComponent<Image>();
        buffDuration = buff.transform.GetChild(1).GetComponent<Image>();
    }

    public void UpdateBuff(AdditionalAbility ability)
    {
        if (ability != AdditionalAbility._NONE)
        {
            buffIcon.enabled = true;
            buffDuration.enabled = true;
        }
        else
        {
            buffIcon.enabled = false;
            buffDuration.enabled = false;
        }
    }
}
