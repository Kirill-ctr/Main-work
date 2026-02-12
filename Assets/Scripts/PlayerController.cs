using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _gravity = -9.8f;

    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _velocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        Movement(_moveDirection);
        Gravity();
    }

    private void Movement(Vector3 direction)
    {
        _characterController.Move(direction * _speed * Time.fixedDeltaTime);
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

}
