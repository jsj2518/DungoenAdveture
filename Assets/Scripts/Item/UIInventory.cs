using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject equipAbilityButton;
    public GameObject unequipAbilityButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition condition;
    private Transform dropPosition;

    private ItemData selectedItem;
    private int selectedItemIndex = -1;

    private int curEquipIndex;
    private int curEquipAbilityIndex;

    private void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventoryToggle += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].OnClick += SelectItem;
        }

        ClearSelectedItemWindow();
    }

    private void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        equipAbilityButton.SetActive(false);
        unequipAbilityButton.SetActive(false);
        dropButton.SetActive(false);
    }


    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }


    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        //아이템이 중복 가능한지
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateSlotUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        //비어있는 슬롯 있는지
        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateSlotUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    private ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    private void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        if (selectedItemIndex >= 0)
        {
            slots[selectedItemIndex].highlighted = false;
        }

        selectedItemIndex = index;
        selectedItem = slots[selectedItemIndex].item;
        slots[selectedItemIndex].highlighted = true;

        selectedItemName.text = selectedItem.name;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equiped == false);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equiped == true);
        equipAbilityButton.SetActive(selectedItem.type == ItemType.AddAbility && slots[index].equiped == false);
        unequipAbilityButton.SetActive(selectedItem.type == ItemType.AddAbility && slots[index].equiped == true);
        dropButton.SetActive(true);

        UpdateSlotUI();
    }

    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateSlotUI();
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equiped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equiped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateSlotUI();

        SelectItem(selectedItemIndex);
    }
    private void UnEquip(int index)
    {
        slots[index].equiped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateSlotUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }
    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnEquipAbilityButton()
    {
        if (slots[curEquipAbilityIndex].equiped)
        {
            UnEquipAbility(curEquipAbilityIndex);
        }

        slots[selectedItemIndex].equiped = true;
        curEquipAbilityIndex = selectedItemIndex;
        CharacterManager.Instance.Player.ability = slots[selectedItemIndex].item.ability;
        UpdateSlotUI();

        SelectItem(selectedItemIndex);
    }
    private void UnEquipAbility(int index)
    {
        slots[index].equiped = false;
        CharacterManager.Instance.Player.ability = AdditionalAbility._NONE;
        UpdateSlotUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }
    public void OnUnEquipAbilityButton()
    {
        UnEquipAbility(selectedItemIndex);
    }
}
