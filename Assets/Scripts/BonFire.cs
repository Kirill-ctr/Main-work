using UnityEngine;
using UnityEngine.UI;

public class Bon : MonoBehaviour
{
    [Header("Health settings")]
    [SerializeField] private float _bonFireMAXHealth = 100f;
    [SerializeField] private float _healthLosePerSecond = 1f;
    [SerializeField] private float _itemHealAmount = 20f;
    [SerializeField] private float _currentHealth;

    [Header("References")]
    [SerializeField] private Slider _healsBar;
    [SerializeField] private ParticleSystem _fireParticle;
    [SerializeField] private Light _fireLight;
    [SerializeField] private GameObject _deathScreen;

    
    private bool _isBurning = true;
    private PlayerController _player;

    private void Start()
    {
        _currentHealth = _bonFireMAXHealth;
        _player = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        PermanentDamage();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(_healsBar != null)
        {
            _healsBar.value = _currentHealth;
        }
    }

    private void PermanentDamage()
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= _healthLosePerSecond * Time.deltaTime;
        }
        else
        {
            _currentHealth = 0;
            Debug.Log("Костер потух");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _player != null && _player.HasItem())
        {
            HealingByItem();
            _player.DeliverItemToBonfire();
        }
    }

    private void HealingByItem()
    {
        _currentHealth += _itemHealAmount;
        _currentHealth = Mathf.Min(_currentHealth, _bonFireMAXHealth);

        Debug.Log($"Костёр полечен! Текущее HP: {_currentHealth}");
    }
}