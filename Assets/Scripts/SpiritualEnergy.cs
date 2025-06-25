using UnityEngine;
using UnityEngine.UI;

public class SpiritualEnergy : MonoBehaviour
{
    public int maxEnergy = 5;
    public int currentEnergy;

    public Image[] energyIcons; // Optional: assign sprite icons in the Inspector
    public Sprite fullIcon;
    public Sprite emptyIcon;

    public GameObject gameOverPanel; // Assign in Inspector
    public PlayerFluteController playerFluteController; // Drag your player here in Inspector


    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateUI();
    }

    public void LoseEnergy(int amount = 1)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateUI();

        // 🔊 Play energy lost sound
        AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.energyHit, "energyHit");

        if (currentEnergy <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        if (energyIcons == null || energyIcons.Length == 0) return;

        for (int i = 0; i < energyIcons.Length; i++)
        {
            if (i < currentEnergy)
                energyIcons[i].sprite = fullIcon;
            else
                energyIcons[i].sprite = emptyIcon;
        }
    }

    void GameOver()
{
    Debug.Log("Game Over!");
    Time.timeScale = 0f;

    if (gameOverPanel != null)
        gameOverPanel.SetActive(true);

    var input = FindObjectOfType<PlayerInputManager>();
    if (input != null) input.enabled = false;

    // 🔊 Optional: stop background music and play game over music
    AudioManager.Instance?.StopMusic();
    AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameOverMusic);

    // 🎭 Trigger player defeat animation
    playerFluteController?.PlayDefeatAnimation();
}


    public void GainEnergy(int amount = 1)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateUI();
    }
}
