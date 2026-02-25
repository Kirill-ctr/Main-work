using UnityEngine;
using UnityEngine.UI;

public class Bon : MonoBehaviour
{
    [Header("Health settings")]
    [SerializeField] private float _bonFireHealth = 100f;
    [SerializeField] private float _healthLosePerSecond = 1f;
    [SerializeField] private float _itemHealAmount = 20f;

    [Header("References")]
    [SerializeField] private Slider _healsBar;
    [SerializeField] private ParticleSystem _fireParticle;
    [SerializeField] private Light _fireLight;
    [SerializeField] private GameObject _deathScreen;

    [Header("Visual feedback")]
    [SerializeField] private float _lowHealthThreshold = 30f;
    [SerializeField] private Color _lowHealthLightColor = Color.red;
    [SerializeField] private float _minParticleEmission = 10f;
    [SerializeField] private float _maxParticleEmission = 50f;

    private float _currentHealth;
    private bool _isBurning = true;
    private PlayerController _player;

}
