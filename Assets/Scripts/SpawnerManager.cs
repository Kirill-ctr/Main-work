using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using JetBrains.Annotations;
public class SpawnerManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private int _maxItems = 5;

    private List<Transform>_spawnPoints = new List<Transform>();
    private List<GameObject> _activeItems = new List<GameObject>();
    private List<bool> _isPointEmpty = new List<bool>();

    private bool _waveIsActive;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("SpawnPoint"))
            {
                _spawnPoints.Add(child);
                _isPointEmpty.Add(false);
            }
        }

        for (int i = 0; i < _maxItems && i < _spawnPoints.Count; i++)
        {
            SpawnItem(i);
        }

        StartCoroutine(RespawnWave());
    }

    public int GetRemainingItemsCount()
    {
        _activeItems.RemoveAll(item => item == null);
        return _activeItems.Count;
    }

    private void SpawnItem(int pointIndex)
    {
        if (pointIndex >= _spawnPoints.Count) return;
        
        GameObject newItem = Instantiate(_itemPrefab, _spawnPoints[pointIndex].position, _spawnPoints[pointIndex].rotation);
        _activeItems.Add(newItem);
        _isPointEmpty[pointIndex] = false;

        var itemScript = newItem.GetComponent<PickItUp>();
        
    }

    private void RespawnItems()
    {
        foreach (GameObject item in _activeItems)
        {
            if (item != null)
                Destroy(item);
        }

        _activeItems.Clear();

        for (int i = 0; i < _isPointEmpty.Count; i++)
        {
            _isPointEmpty[i] = false;
        }

        for (int i = 0; i < _maxItems && i < _spawnPoints.Count; i++)
        {
            SpawnItem(i);
        }
    } 

    private IEnumerator RespawnWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(40f);
            _waveIsActive = false;
            RespawnItems();
            _waveIsActive = true;
        }
    }

    public bool IsWaveActive() => _waveIsActive;
}
