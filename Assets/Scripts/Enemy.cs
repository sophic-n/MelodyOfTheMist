using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Difficulty { Low, Medium, High, VeryHigh }
    public Difficulty difficulty;

    public PromptUI promptUI;

    private List<KeyCode> currentSequence;
    private float timer;
    private float timeLimit;
    private bool defeated = false;

    private KeyCode[] possibleKeys = new KeyCode[]
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
    };

    void Start()
    {
        if (promptUI == null)
            promptUI = GetComponentInChildren<PromptUI>();

        if (promptUI != null)
        {
            promptUI.followTarget = this.transform;
        }
        else
        {
            Debug.LogWarning("[Enemy] PromptUI not assigned or found.");
        }

        void Start()
    {
    GenerateRandomSequence();    
    Debug.Log($"[Enemy] Sequence initialized: {string.Join(", ", currentSequence)}");
    }
        timeLimit = GetTimeLimitByDifficulty();
        timer = timeLimit;
    }

    void Update()
    {
        if (defeated) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Fail();
        }
    }

 void GenerateRandomSequence()
{
    int sequenceLength = GetSequenceLengthByDifficulty();
    currentSequence = new List<KeyCode>();

    for (int i = 0; i < sequenceLength; i++)
    {
        KeyCode randomKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
        currentSequence.Add(randomKey);
    }

    Debug.Log("[Enemy] Generated Sequence: " + string.Join(", ", currentSequence));

    if (promptUI != null)
    {
        promptUI.SetSequence(currentSequence);
    }
}


    int GetSequenceLengthByDifficulty()
    {
        return difficulty switch
        {
            Difficulty.Low => 4,
            Difficulty.Medium => 6,
            Difficulty.High => 8,
            Difficulty.VeryHigh => 10,
            _ => 4,
        };
    }

    float GetTimeLimitByDifficulty()
    {
        return difficulty switch
        {
            Difficulty.Low => 10f,
            Difficulty.Medium => 7f,
            Difficulty.High => 5f,
            Difficulty.VeryHigh => 3f,
            _ => 10f,
        };
    }

    private int currentInputIndex = 0;

public bool ReceiveInput(KeyCode input)
{
    Debug.Log($"[Enemy] Input received: {input}");

    if (timer <= 0f || defeated)
    {
        Debug.LogWarning("[Enemy] Rejected input: Time expired or already defeated.");
        return false;
    }

    if (currentSequence == null || currentSequence.Count == 0)
    {
        Debug.LogWarning("[Enemy] Rejected input: No sequence available.");
        return false;
    }

    if (currentInputIndex >= currentSequence.Count)
    {
        Debug.LogWarning("[Enemy] Rejected input: Already completed sequence.");
        return false;
    }

    KeyCode expected = currentSequence[currentInputIndex];
    Debug.Log($"[Enemy] Comparing: Expected {expected}, Got {input}");

    if (input == expected)
    {
        currentInputIndex++;
        Debug.Log($"[Enemy] Correct input! Progress: {currentInputIndex}/{currentSequence.Count}");

        if (currentInputIndex >= currentSequence.Count)
        {
            Debug.Log("[Enemy] Sequence complete — triggering defeat.");
            StartCoroutine(DelayedDefeat());
        }

        return true;
    }
    else
    {
        Debug.Log("[Enemy] Incorrect input. Triggering fail.");
        Fail();
        return false;
    }
}


    IEnumerator DelayedDefeat()
    {
        defeated = true;
        Debug.Log("[Enemy] Enemy defeated!");

        yield return null; // wait one frame to avoid accessing destroyed UI

        FindObjectOfType<PlayerInputManager>()?.UnregisterEnemy(this);
        Destroy(gameObject);
    }

    void Fail()
    {
        Debug.Log("[Enemy] Input failed!");

        timer = timeLimit; // reset timer
        currentSequence = new List<KeyCode>(currentSequence); // keep same sequence for retry

        var spirit = FindObjectOfType<SpiritualEnergy>();
        if (spirit != null)
        {
            spirit.LoseEnergy(); // your energy logic
        }
    }
}
