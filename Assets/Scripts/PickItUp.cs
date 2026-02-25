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
    private Rigidbody _rigidBody;
    private Collider _collider;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        if (_rigidBody == null)
        {
            _rigidBody = gameObject.AddComponent<Rigidbody>();
            _rigidBody.mass = 1f;
            _rigidBody.linearDamping = 1f;
            _rigidBody.angularDamping = 1f;
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
            _rigidBody.MovePosition(Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.fixedDeltaTime));

            Quaternion targetRotation = Quaternion.LookRotation(_carrier.forward);
            _rigidBody.MoveRotation(Quaternion.Slerp(_rigidBody.rotation, targetRotation, _smoothSpeed * Time.fixedDeltaTime));
        }
    }

    private void CheckPlayerNearby()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= _pickupRange)
        {
            if (_activePrompt == null && _pickupPromptPrefab != null)
            {
                _activePrompt = Instantiate(_pickupPromptPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                _activePrompt.transform.SetParent(transform); 
            }

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


        _rigidBody.useGravity = false;
        _rigidBody.linearDamping = 5f; 

        if (_collider != null)
            _collider.enabled = false;

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

        _rigidBody.useGravity = true;
        _rigidBody.linearDamping = 1f;

        if (_collider != null)
            _collider.enabled = true;

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