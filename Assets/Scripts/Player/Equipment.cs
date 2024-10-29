using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Transform equipCamera;
    private Equip curEquip;

    private Equip curEquipAbility;

    private PlayerController controller;
    private PlayerCondition condition;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }

    public void EquipNew(ItemData data)
    {
        if (data.type != ItemType.Equipable) return;

        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipCamera).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (curEquip == null) return;

        Destroy(curEquip.gameObject);
        curEquip = null;
    }

    public void EquipAbilityNew(ItemData data)
    {
        if (data.type != ItemType.AddAbility) return;

        UnEquipAbility();
        curEquipAbility = Instantiate(data.equipPrefab, transform).GetComponent<Equip>();
    }

    public void UnEquipAbility()
    {
        if (curEquipAbility == null) return;

        Destroy(curEquipAbility.gameObject);
        curEquipAbility = null;
    }


    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook == true)
        {
            curEquip.OnAttackInput();
        }
    }

    public void OnAbilityInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquipAbility != null && controller.canLook == true)
        {
            curEquipAbility.OnAbilityInput();
        }
    }
}
