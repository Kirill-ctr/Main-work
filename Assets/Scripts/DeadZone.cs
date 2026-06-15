using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private GameObject _deathScreen;

    private void Die()
    {
        enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Игрок умер");
        if (_deathScreen != null)
        {
            _deathScreen.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }
}
