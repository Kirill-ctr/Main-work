using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _checkGroundDistans = 0.3f;
    [SerializeField] private float _jumpHeight = 0.3f;
    [SerializeField] private LayerMask _layerMaskGround;
    [SerializeField] private Transform _carryPoint;
    [SerializeField] private Transform _groundChecker;

    private float _velocity;
    private bool _isGrounded = false;
    private Vector3 _moveDirection;
    private PickItUp _currentCarriedItem = null;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_carryPoint == null)
        {
            GameObject carryPoint = new GameObject("CarryPoint");
            carryPoint.transform.SetParent(transform);
            carryPoint.transform.localPosition = new Vector3(0, 0.5f, 1f);
            _carryPoint = carryPoint.transform;
        }
    }

    private void Update()
    {
        ReadInput();
        RotateToMovement();
    }

    private void FixedUpdate()
    {
        _isGrounded = IsGround();

        if (_isGrounded && _velocity < 0)
            _velocity = -2f;

        Movement(_moveDirection);
        Gravity();
    }

    public void Die()
    {
        Debug.Log("Игрок умер");

        enabled = false;
    }

    public bool HasItem()
    {
        return _currentCarriedItem != null;
    }

    public void DeliverItemToBonfire()
    {
        if (_currentCarriedItem != null)
        {
            _currentCarriedItem.DeliverToBonfire();
            _currentCarriedItem = null;
        }
    }

    public void TryPickupItem(PickItUp item)
    {
        if (_currentCarriedItem == null)
        {
            _currentCarriedItem = item;
            item.Pickup(_carryPoint);
        }
    }

    private void DropItem()
    {
        if (_currentCarriedItem != null)
        {
            _currentCarriedItem.Drop();
            _currentCarriedItem = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bonfire") && _currentCarriedItem != null)
        {
            _currentCarriedItem.DeliverToBonfire();
            _currentCarriedItem = null;
        }
    }

    private void Movement(Vector3 direction)
    {
        _characterController.Move(direction * _speed * Time.fixedDeltaTime);
    }

    private void RotateToMovement()
    {
        if(_moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed* Time.deltaTime);
        }
    }

    private void Jump()
    {
        _velocity = Mathf.Sqrt(_jumpHeight * -4 * _gravity);
    }

    private void ReadInput()
    {
        Transform cameraTransform = Camera.main.transform;

        _moveDirection = Vector3.zero;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        float vertical = 0;
        float horizontal = 0;

        if (Keyboard.current.wKey.isPressed)
            vertical +=1;
        if (Keyboard.current.sKey.isPressed)
            vertical -= 1;
        if (Keyboard.current.dKey.isPressed)
            horizontal += 1;
        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1;
        if(Keyboard.current.spaceKey.wasPressedThisFrame && _isGrounded)
            Jump();

        _moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (Keyboard.current.qKey.isPressed && _currentCarriedItem != null)
        {
            DropItem();
        }
    }

    private void Gravity()
    {
        _velocity += _gravity * Time.fixedDeltaTime;
        _characterController.Move(Vector3.up * _velocity * Time.fixedDeltaTime);
    }
    
    private bool IsGround()
    {
        RaycastHit hit;
        bool result = Physics.Raycast(transform.position, -transform.up, out hit, _checkGroundDistans);
        Debug.DrawRay(transform.position, -transform.up * _checkGroundDistans, Color.red);
        return result;
    }

}