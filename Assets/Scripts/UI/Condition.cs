using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    [HideInInspector] public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;

    private void Start()
    {
        curValue = startValue;
    }

    private void Update()
    {
        uiBar.fillAmount = GetPercentage();
    }

    private float GetPercentage()
    {
        return curValue / maxValue;
    }


    public void Add(float value)
    {
        curValue = Mathf.Clamp(curValue + value, 0, maxValue);
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Clamp(curValue - value, 0, maxValue);
    }
}
