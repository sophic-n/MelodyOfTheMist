using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Difficulty { Low, Medium, High, VeryHigh }
    public Difficulty difficulty;

    public PromptUI promptUI;
    public Transform playerTarget;
    public float moveSpeed = 1.0f;

    private List<KeyCode> currentSequence;
    private float timer;
    private float timeLimit;
    private bool defeated = false;
    private int currentIndex = 0;

    private Animator animator;

    private KeyCode[] possibleKeys = new KeyCode[]
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
    };

    public List<KeyCode> GetCurrentSequence() => currentSequence;

    void Start()
    {
        animator = GetComponentInChildren<Animator>(); // or GetComponent<Animator>() depending on setup
        if (animator != null) animator.SetTrigger("walk");

        FindObjectOfType<GameStateManager>()?.RegisterEnemy();

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
            else Debug.LogError("[Enemy] Player target not found! Tag it as 'Player'");
        }

        if (promptUI == null) promptUI = GetComponentInChildren<PromptUI>();
        if (promptUI != null)
{
    Transform anchor = transform.Find("PromptAnchor");
    if (anchor != null)
    {
        promptUI.followTarget = anchor;
    }
    else
    {
        Debug.LogWarning("[Enemy] PromptAnchor not found — using root transform.");
        promptUI.followTarget = transform;
    }
}

        else Debug.LogWarning("[Enemy] PromptUI not assigned or found.");

        var inputManager = FindObjectOfType<PlayerInputManager>();
        inputManager?.RegisterEnemy(this);

        GenerateRandomSequence();
        timeLimit = GetTimeLimitByDifficulty();
        timer = timeLimit;

        AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.enemySpawns, "enemySpawn");
    }

    void Update()
    {
        if (defeated) return;

        if (playerTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, playerTarget.position) < 0.1f)
            {
                Debug.Log("[Enemy] Reached player!");

                // Play attack animation
                if (animator != null) animator.SetTrigger("attack");

                Fail();
                Destroy(gameObject);
                return;
            }
        }

        timer -= Time.deltaTime;
        if (timer <= 0f) Fail();
    }

    void GenerateRandomSequence()
    {
        int sequenceLength = GetSequenceLengthByDifficulty();
        currentSequence = new List<KeyCode>();
        currentIndex = 0;

        for (int i = 0; i < sequenceLength; i++)
        {
            KeyCode randomKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
            currentSequence.Add(randomKey);
        }

        promptUI.SetSequence(currentSequence);
        Debug.Log($"[Enemy] Generated sequence: {string.Join(", ", currentSequence)}");
    }

    int GetSequenceLengthByDifficulty() => difficulty switch
    {
        Difficulty.Low => 4,
        Difficulty.Medium => 6,
        Difficulty.High => 8,
        Difficulty.VeryHigh => 10,
        _ => 4,
    };

    float GetTimeLimitByDifficulty() => difficulty switch
    {
        Difficulty.Low => 10f,
        Difficulty.Medium => 7f,
        Difficulty.High => 5f,
        Difficulty.VeryHigh => 3f,
        _ => 10f,
    };

    public bool ReceiveInput(KeyCode key)
    {
        if (currentSequence == null || currentIndex >= currentSequence.Count)
        {
            Debug.Log("[Enemy] No sequence or sequence already completed.");
            return false;
        }

        Debug.Log($"[Enemy] Comparing {key} to expected {currentSequence[currentIndex]}");

        if (currentSequence[currentIndex] == key)
        {
            currentIndex++;
            promptUI.RemoveFirstNote();

            AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.fluteCorrectNotes, "fluteCorrect");

            if (currentIndex >= currentSequence.Count)
            {
                Debug.Log("[Enemy] Sequence complete!");
                AudioManager.Instance?.PlaySFXNonOverlapRandom(AudioManager.Instance.sequenceSuccess);
                StartCoroutine(DelayedDefeat());
            }

            return true;
        }
        else
        {
            Debug.Log("[Enemy] Input failed! Wrong key.");
            AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.fluteWrongNotes, "fluteWrong");
            Fail();
            return false;
        }
    }

    IEnumerator DelayedDefeat()
    {
        defeated = true;
        Debug.Log("[Enemy] Enemy defeated!");

        // Play defeat animation
        if (animator != null) animator.SetTrigger("defeat");

        AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.enemyDefeated, "enemyDead");

        // Let the animation play for a bit
        yield return new WaitForSeconds(0.5f);

        FindObjectOfType<PlayerInputManager>()?.UnregisterEnemy(this);
        FindObjectOfType<GameStateManager>()?.UnregisterEnemy();
        Destroy(gameObject);
    }

    void Fail()
    {
        if (defeated) return;

        Debug.Log("[Enemy] Player failed input!");

        timer = timeLimit;
        currentIndex = 0;

        var spirit = FindObjectOfType<SpiritualEnergy>();
        if (spirit != null)
        {
            spirit.LoseEnergy();
            AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.energyHit, "energyHit");
        }
    }
}
