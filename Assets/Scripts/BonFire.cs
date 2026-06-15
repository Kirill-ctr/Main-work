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
    [SerializeField] private GameObject _deathScreen;
    [SerializeField] private GameObject _winScreen;

    [Header("Dynamic Fire")]
    [SerializeField] private ParticleFireSystem _dynamicFire;

    private bool _isBurning = true;
    private PlayerController _player;
    private SpawnerManager _spawnerManager;
    private int _deliveredItemCounts;
    private int _deliveredInWave;
    private bool _hasResetInThisWave = false;

    private void Start()
    {

        if (_deathScreen != null)
        {
            _deathScreen.SetActive(false);
        }

        _currentHealth = _bonFireMAXHealth;
        _player = FindAnyObjectByType<PlayerController>();
        _spawnerManager = FindAnyObjectByType<SpawnerManager>();

        _hasResetInThisWave = false;
    }

    private void Update()
    {
        bool isWaveActive = _spawnerManager != null && _spawnerManager.IsWaveActive();

        if (!isWaveActive && !_hasResetInThisWave)
        {
            _deliveredInWave = 0;
            _deliveredItemCounts = 0;
            _hasResetInThisWave = true;

            Debug.Log("Волна закончилась");
        }
        else if (isWaveActive && _hasResetInThisWave)
        {
            _hasResetInThisWave = false;
            Debug.Log("Новая волна началась");
        }
        
        PermanentDamage();
        UpdateUI();
        UpdateParticls();

        if (!_isBurning) return; 
        if(_currentHealth <= 0 && _isBurning)
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        if(!_isBurning) return;

        _isBurning=false;
        Debug.Log("Костер потух, конец игры");

        if(_deathScreen != null)
        {
            _deathScreen.SetActive(true);
        }

        if (_player != null)
            _player.Die();

        Time.timeScale = 0f;
    }

    private void UpdateParticls()
    {
        float normalizedHealth = _currentHealth / _bonFireMAXHealth;
        if (_dynamicFire != null)
        {
            _dynamicFire.SetIntensity(normalizedHealth);
        }

        float normalizedSize = _currentHealth / _bonFireMAXHealth;
        if (_dynamicFire != null)
        {
            _dynamicFire.UpdateParticlesByHealth(normalizedSize);
        }
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
            _deliveredItemCounts++;
            _deliveredInWave++;
            _player.PlayerUpgrade(_deliveredItemCounts);

            Debug.Log($"Доставлено в этой волне { _deliveredInWave}");

            if (_deliveredInWave >= 5)
            {
                Win();
            }
        }
    }

    private void HealingByItem()
    {
        _currentHealth += _itemHealAmount;
        _currentHealth = Mathf.Min(_currentHealth, _bonFireMAXHealth);

        Debug.Log($"Костёр полечен! Текущее HP: {_currentHealth}");
    }

    private void Win()
    {
        _isBurning = false;
        Time.timeScale = 0f;
        enabled = false;

        if (_winScreen != null)
        {
            _winScreen.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}