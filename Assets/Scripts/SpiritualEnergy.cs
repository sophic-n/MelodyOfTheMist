using UnityEngine;
using UnityEngine.UI;

public class SpiritualEnergy : MonoBehaviour
{
    public int maxEnergy = 5;
    public int currentEnergy;

    public Image[] energyIcons;
    public Sprite fullIcon;
    public Sprite emptyIcon;

    public GameObject gameOverPanel;
    public PlayerFluteController playerFluteController;

    [Header("Debug")]
    public bool godMode = false; // ✅ Toggle this in Inspector

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateUI();
    }

    public void LoseEnergy(int amount = 1)
    {
        if (godMode)
        {
            Debug.Log("[SpiritualEnergy] God mode ON – no energy lost.");
            return;
        }

        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateUI();

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
            energyIcons[i].sprite = i < currentEnergy ? fullIcon : emptyIcon;
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

        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameOverMusic);

        playerFluteController?.PlayDefeatAnimation();
    }

    public void GainEnergy(int amount = 1)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateUI();
    }
}
