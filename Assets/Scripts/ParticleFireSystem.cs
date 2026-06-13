using Unity.Mathematics;
using UnityEngine;

public class ParticleFireSystem : MonoBehaviour
{
    [SerializeField] private float _maxEmission = 50f;
    [SerializeField] private float _minEmission = 5f;
    [SerializeField] private ParticleSystem _firearticle;
    private ParticleSystem.EmissionModule _emissionModule;

    private void Start()
    {
        _firearticle = GetComponentInChildren<ParticleSystem>();
        _emissionModule = _firearticle.emission;
    }

    public void SetIntensity (float normalizedHealth)
    {
        float emissionRate = math.lerp(_minEmission, _maxEmission, normalizedHealth);
        var rate = new ParticleSystem.MinMaxCurve(emissionRate);
        _emissionModule.rateOverTime = rate;
    }
}
