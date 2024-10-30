using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayerMask;
    private Vector2 curMovementInput;

    [SerializeField] private Transform startPoint;
    [SerializeField] private LayerMask layerTrampoline;

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private float lookSensitivity;
    private float camCurXRot;
    private Vector2 mouseDelta;
    [HideInInspector] public bool canLook = true;

    public Action inventoryToggle;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CharacterManager.Instance.Player.condition.OnEndBuff += BuffEndEvent;
        GotoStartPoint();
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f) GotoStartPoint();

        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void GotoStartPoint()
    {
        transform.position = startPoint.position;
    }


    private void Move()
    {
        Vector3 dir = (transform.forward * curMovementInput.y + transform.right * curMovementInput.x).normalized;

        if (IsGrounded())
        {
            dir *= moveSpeed;
            dir.y = _rigidbody.velocity.y;
        }
        else
        {
            dir *= moveSpeed * Time.deltaTime * 5f;
            Vector3 temp = dir + _rigidbody.velocity;
            temp = new Vector3(temp.x, 0, temp.z);
            if (temp.magnitude > moveSpeed)
            {
                temp = temp.normalized * moveSpeed;
            }
            dir = new Vector3(temp.x, _rigidbody.velocity.y, temp.z);
        }

        _rigidbody.velocity = dir;
    }

    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }



    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsLayerMatched(layerTrampoline, collision.gameObject.layer))
        {
            _rigidbody.AddForce(Vector2.up * 500f, ForceMode.Impulse);
        }
    }

    public void OnUseAbility(InputAction.CallbackContext context)
    {
        Debug.Log($"{CharacterManager.Instance.Player.ability}    {CharacterManager.Instance.Player.condition.buffActivated}");
        if (context.phase == InputActionPhase.Started &&
            CharacterManager.Instance.Player.ability != AdditionalAbility._NONE &&
            CharacterManager.Instance.Player.condition.buffActivated == AdditionalAbility._NONE)
        {
            if (CharacterManager.Instance.Player.condition.UseMana(20f)) // 고정값
            {
                CharacterManager.Instance.Player.condition.ActivateBuff(CharacterManager.Instance.Player.ability);
                moveSpeed *= 2f; // 효과 적용
            }
        }
    }
    private void BuffEndEvent(AdditionalAbility ability)
    {
        moveSpeed /= 2f; // 효과지속 끝
    }

    public void OnInventoryToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventoryToggle?.Invoke();
            ToggleCursor();
        }
    }

    private void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    private bool IsLayerMatched(int layerMask, int objectLayer)
    {
        return layerMask == (layerMask | (1 << objectLayer));
    }
}
