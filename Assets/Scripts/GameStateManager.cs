using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject winPanel; // Assign in Inspector
    private int activeEnemies = 0;

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

    void Win()
    {
        Debug.Log("You Win!");
        Time.timeScale = 0f;

        if (winPanel != null)
            winPanel.SetActive(true);

        var input = FindObjectOfType<PlayerInputManager>();
        if (input != null) input.enabled = false;

        //  Play win music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameWinMusic);
        }

        // Optional: Play a one-shot victory SFX
        // AudioManager.Instance.PlayRandomSFX(AudioManager.Instance.victorySFX, "victory");
    }
}


