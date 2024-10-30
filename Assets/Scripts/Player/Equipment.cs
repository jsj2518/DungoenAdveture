using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Transform equipCamera;
    private Equip curEquip;

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


    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook == true)
        {
            curEquip.OnAttackInput();
        }
    }
}
