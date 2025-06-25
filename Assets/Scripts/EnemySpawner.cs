using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs by Difficulty")]
    public GameObject[] lowEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] highEnemies;
    public GameObject[] bossEnemies;

    public Transform spawnPoint;
    public float spawnDelay = 5f;

    private int waveNumber = 1;

    void Start()
    {
        StartCoroutine(SpawnEnemyWaves());
    }

    IEnumerator SpawnEnemyWaves()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);

            waveNumber++;

            if (spawnDelay > 1f)
                spawnDelay -= 0.1f;
        }
    }

    void SpawnEnemy()
{
    int difficultyRoll = Random.Range(0, 100);
    GameObject selectedPrefab = null;

    if (difficultyRoll < 40)
    {
        selectedPrefab = lowEnemies[Random.Range(0, lowEnemies.Length)];
    }
    else if (difficultyRoll < 70)
    {
        selectedPrefab = mediumEnemies[Random.Range(0, mediumEnemies.Length)];
    }
    else if (difficultyRoll < 90)
    {
        selectedPrefab = highEnemies[Random.Range(0, highEnemies.Length)];
    }
    else
    {
        selectedPrefab = bossEnemies[Random.Range(0, bossEnemies.Length)];
    }

    if (selectedPrefab != null)
    {
        GameObject enemy = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            // Offset promptUI to prevent overlapping
            if (enemyScript.promptUI != null)
            {
                // Offset by a random horizontal amount
                enemyScript.promptUI.offset += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
            }

            // enemyScript.AssignDifficulty(waveNumber);
        }
    }
    else
    {
        Debug.LogWarning("No enemy prefab assigned for this difficulty level.");
    }
}
}


