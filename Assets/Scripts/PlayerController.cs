using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _checkGroundRadius = 0.5f;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private LayerMask _layerMaskGround;
    [SerializeField] private float _jumpHeight = 0.1f;

    [Header("Sensitivity")]
    [Range(1, 100)]
    [SerializeField] private int _sensitivity = 30;

    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _velocity;
    private bool _isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        _isGrounded = IsGround();

        if (_isGrounded && _velocity < 0)
            _velocity = -2f;

        Movement(_moveDirection);
        Gravity();
    }

    private void Movement(Vector3 direction)
    {
        _characterController.Move(direction * _speed * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        _velocity = Mathf.Sqrt(_jumpHeight * -4 * _gravity);
    }

    private void ReadInput()
    {
        _moveDirection = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            _moveDirection += new Vector3(0f, 0f, 1f);

        if (Keyboard.current.aKey.isPressed)
            _moveDirection -= new Vector3(1f, 0f, 0f);

        if (Keyboard.current.sKey.isPressed)
            _moveDirection -= new Vector3(0f, 0f, 1f);

        if (Keyboard.current.dKey.isPressed)
            _moveDirection += new Vector3(1f, 0f, 0f);

        if(Keyboard.current.spaceKey.isPressed && _isGrounded)
            Jump();

        if (_moveDirection.magnitude > 0.1f)
        {
            _moveDirection = _moveDirection.normalized;
            _moveDirection = transform.TransformDirection(_moveDirection);
        }
    }

    private void Gravity()
    {
        _velocity += _gravity * Time.fixedDeltaTime;
        _characterController.Move(Vector3.up * _velocity * Time.fixedDeltaTime);
    }

    private bool IsGround()
    {
        bool result = Physics.CheckSphere(_groundChecker.position, _checkGroundRadius, _layerMaskGround);
        return result;
    }

}
