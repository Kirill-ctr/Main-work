using UnityEngine;
using System.Collections.Generic;
public class SpawnerManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private float _respawnTime = 5f;
    [SerializeField] private int _maxItems = 5;

    private List<Transform>_spawnPoints = new List<Transform>();
    private List<GameObject> _activeItems = new List<GameObject>();
    private List<bool> _isPointEmpty = new List<bool>();

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
    }

    private void SpawnItem(int pointIndex)
    {
        if (pointIndex >= _spawnPoints.Count) return;
        
        GameObject newItem = Instantiate(_itemPrefab, _spawnPoints[pointIndex].position, _spawnPoints[pointIndex].rotation);
        _activeItems.Add(newItem);
        _isPointEmpty[pointIndex] = false;

        var itemScript = newItem.GetComponent<PickItUp>();
        if (itemScript != null) 
        {
            StartCoroutine(WaitForItemDelivery(newItem, pointIndex));
        }
    }

    private System.Collections.IEnumerator WaitForItemDelivery (GameObject item, int pointIndex)
    {
        yield return new WaitUntil(() => item == null);
        yield return new WaitForSeconds(_respawnTime);

        SpawnItem(pointIndex);
    } 
}
