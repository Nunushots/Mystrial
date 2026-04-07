using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private GameObject boss; // Assign in Inspector

    private Coroutine spawnRoutine;

    private void Start()
    {
        if (boss != null)
        {
            spawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (boss != null)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        StopSpawning();
    }

    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    private void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        Debug.Log("Boss defeated. Spawner stopped.");
    }
}
