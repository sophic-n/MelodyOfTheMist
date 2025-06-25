using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject instructionsPanel;
    public GameObject gameplayUI; 

    public GameObject spawner; // Reference to your enemy spawner script object

    void Start()
    {
        // Pause game at start until player presses Start
        Time.timeScale = 0f;
        mainMenuCanvas.SetActive(true);
        if (gameplayUI != null)
            gameplayUI.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        instructionsPanel.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        if (spawner != null)
            spawner.SetActive(true); // Start spawning enemies

        Time.timeScale = 1f;
    }

    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        instructionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game (only works in build)");
    }
}

