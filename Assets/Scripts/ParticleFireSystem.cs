using Unity.Mathematics;
using UnityEngine;

public class ParticleFireSystem : MonoBehaviour
{
    [Header("Particle dynamics")]
    [SerializeField] private float _maxEmission = 50f;
    [SerializeField] private float _minEmission = 5f;
    [SerializeField] private float _maxSize = 1.0f;
    [SerializeField] private float _minSize = 0f;
    [SerializeField] private ParticleSystem _firearticle;
    [SerializeField] private ParticleSystem.MainModule _mainModule;

    private ParticleSystem.EmissionModule _emissionModule;

    private void Start()
    {
        _firearticle = GetComponentInChildren<ParticleSystem>();
        _emissionModule = _firearticle.emission;
        _mainModule = _firearticle.main;
    }

    public void SetIntensity (float normalizedHealth)
    {
        float emissionRate = math.lerp(_minEmission, _maxEmission, normalizedHealth);
        var rate = new ParticleSystem.MinMaxCurve(emissionRate);
        _emissionModule.rateOverTime = rate;
    }

    public void UpdateParticlesByHealth(float normalizedHealth)
    {
        float currentSize = math.lerp(_minSize, _maxSize, normalizedHealth);
        _mainModule.startSize = currentSize;
    }
}
