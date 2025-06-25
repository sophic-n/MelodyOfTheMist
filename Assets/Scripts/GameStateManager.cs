using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject winPanel;
    private int activeEnemies = 0;

    [Header("Debug")]
    public bool debugMode = false;

    public WaveSpawner enemySpawner; // <-- Assign your spawner in Inspector

    public void RegisterEnemy()
    {
        activeEnemies++;
    }

    public void UnregisterEnemy()
    {
        activeEnemies--;
        if (activeEnemies <= 0)
        {
            Win();
        }
    }
    void Update()
    {
        if (!debugMode) return;

        // Skip to next wave with 'N'
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("[Debug] Skipping to next wave via 'N' key.");

            if (enemySpawner != null)
            {
                enemySpawner.ForceNextWave();
                activeEnemies = 0; // Optional: reset counter
            }
            else
            {
                Debug.LogWarning("[GameStateManager] No EnemySpawner assigned for next wave!");
            }
        }

        // Force win screen with 'W'
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("[Debug] Forcing Win Screen via 'W' key.");
            Win();
        }
    }


    void Win()
    {
        Debug.Log("You Win!");
        Time.timeScale = 0f;

        if (winPanel != null)
            winPanel.SetActive(true);

        var input = FindObjectOfType<PlayerInputManager>();
        if (input != null) input.enabled = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameWinMusic);
        }
    }
}
