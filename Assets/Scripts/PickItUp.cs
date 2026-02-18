using UnityEngine;
using UnityEngine.InputSystem;

public class PickItUp : MonoBehaviour
{
    [Header("Carry Settings")]
    [SerializeField] private float _pickupRange = 2f;
    [SerializeField] private float _carryDistance = 1.5f;
    [SerializeField] private float _carryHeight = 0.5f;
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("Visual")]
    [SerializeField] private GameObject _pickupPromptPrefab;

    private bool _isBeingCarried = false;
    private Transform _carrier;
    private GameObject _activePrompt;
    private Rigidbody _rb;
    private Collider _col;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.mass = 1f;
            _rb.linearDamping = 1f;
            _rb.angularDamping = 1f;
        }
    }

    private void Update()
    {
        if (!_isBeingCarried)
        {
            CheckPlayerNearby();
        }
    }

    private void FixedUpdate()
    {
        if (_isBeingCarried && _carrier != null)
        {
            Vector3 targetPosition = _carrier.position + _carrier.forward * _carryDistance + Vector3.up * _carryHeight;
            _rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.fixedDeltaTime));

            // ������� ������� ��������
            Quaternion targetRotation = Quaternion.LookRotation(_carrier.forward);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _smoothSpeed * Time.fixedDeltaTime));
        }
    }

    private void CheckPlayerNearby()
    {
        // ������� ������
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= _pickupRange)
        {
            if (_activePrompt == null && _pickupPromptPrefab != null)
            {
                _activePrompt = Instantiate(_pickupPromptPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                _activePrompt.transform.SetParent(transform); // ����������� � ��������
            }

            // ��������� ������� E
            if (Keyboard.current.eKey.isPressed)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TryPickupItem(this);
                }
            }
        }
        else
        {
            if (_activePrompt != null)
            {
                Destroy(_activePrompt);
                _activePrompt = null;
            }
        }
    }

    public void Pickup(Transform carrier)
    {
        _isBeingCarried = true;
        _carrier = carrier;


        _rb.useGravity = false;
        _rb.linearDamping = 5f; 

        if (_col != null)
            _col.enabled = false;

        if (_activePrompt != null)
        {
            Destroy(_activePrompt);
            _activePrompt = null;
        }

        Debug.Log($"Предмет {name} поднят");
    }

    public void Drop()
    {
        _isBeingCarried = false;
        _carrier = null;

        _rb.useGravity = true;
        _rb.linearDamping = 1f;

        if (_col != null)
            _col.enabled = true;

        Debug.Log($"Предмет {name} Брошен");
    }

    public void DeliverToBonfire()
    {
        Debug.Log($"Предмет {name} доставлен к костру!");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _pickupRange);

        if (_carrier != null)
        {
            Gizmos.color = Color.green;
            Vector3 carryPos = _carrier.position + _carrier.forward * _carryDistance + Vector3.up * _carryHeight;
            Gizmos.DrawWireSphere(carryPos, 0.2f);
        }
    }
}