using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public ItemData item;

    public Image icon;
    public TextMeshProUGUI quantityText;
    private Image backGround;
    private Outline outline;

    public event Action<int> OnClick;

    [HideInInspector] public int index;
    [HideInInspector] public bool equiped;
    [HideInInspector] public bool highlighted;
    [HideInInspector] public int quantity;

    private void Awake()
    {
        backGround = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = true;
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = highlighted;
        }

        if (equiped)
        {
            if (item.type == ItemType.Equipable)
            {
                backGround.color = new Color(0.5f, 0.5f, 1.0f);
            }
            else
            {
                backGround.color = new Color(1.0f, 0.5f, 0.0f);
            }
        }
        else
        {
            backGround.color = new Color(99f / 255, 99f / 255, 99f / 255);
        }
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void OnClickButton()
    {
        OnClick?.Invoke(index);
    }
}
