using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerInputManager : MonoBehaviour
{
    public List<Enemy> activeEnemies = new List<Enemy>();
    private PlayerControls controls;

    private Enemy currentTarget;

    [Header("Player Animation")]
    public PlayerFluteController playerFluteController; // <-- Drag reference in Inspector

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.NoteInput.performed += OnNoteInput;
    }

    private void OnDisable()
    {
        controls.Gameplay.NoteInput.performed -= OnNoteInput;
        controls.Disable();
    }

    private void OnNoteInput(InputAction.CallbackContext context)
    {
        if (context.control is KeyControl keyControl)
        {
            try
            {
                string keyName = keyControl.name.ToUpper();
                KeyCode pressedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);

                Debug.Log($"Raw key: {keyName}, Converted KeyCode: {pressedKey}");

                if (activeEnemies.Count > 0)
                {
                    SetCurrentTarget(activeEnemies[0]);
                    bool success = activeEnemies[0].ReceiveInput(pressedKey);

                    if (success)
                    {
                        playerFluteController?.PlayAttackAnimation();
                    }
                }
                else
                {
                    Debug.LogWarning("No active enemies to receive input.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to parse key: " + e.Message);
            }
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
        Debug.Log($"[PlayerInputManager] Registered new enemy: {enemy.name}");
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);

        if (enemy == currentTarget)
        {
            currentTarget?.promptUI.Highlight(false);
            currentTarget = null;
        }
    }

    public void SetCurrentTarget(Enemy enemy)
    {
        if (enemy == currentTarget) return;

        currentTarget?.promptUI.Highlight(false);
        currentTarget = enemy;
        currentTarget?.promptUI.Highlight(true);
    }
}
