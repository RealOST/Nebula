using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager> {
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;

    [SerializeField] private bool spawnEnemy = true;
    [SerializeField] private GameObject waveUI;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float timeBetweenSpawns = 1f;
    [SerializeField] private float timeBetweenWaves = 1f;
    [SerializeField] private int minEnemyAmount = 4;
    [SerializeField] private int maxEnemyAmount = 10;

    [Header("---- Boss Settings ----")]
    [SerializeField] private GameObject bossPrefab;

    [SerializeField] private int bossWaveNumber;

    private int waveNumber = 1;
    private int enemyAmount;
    private List<GameObject> enemyList = new();

    private WaitForSeconds waitTimeBetweenSpawns => new(timeBetweenSpawns);
    private WaitForSeconds waitTimeBetweenWaves => new(timeBetweenWaves);
    private WaitUntil waitUntilNoEnemy => new(() => enemyList.Count == 0);

    private IEnumerator Start() {
        while (spawnEnemy && GameManager.GameState != GameState.GameOver) {
            waveUI.SetActive(true);

            yield return waitTimeBetweenWaves;

            waveUI.SetActive(false);

            yield return StartCoroutine(RandomlySpawnCoroutine());
        }
    }

    private IEnumerator RandomlySpawnCoroutine() {
        if (waveNumber % bossWaveNumber == 0) {
            var boss = PoolManager.Release(bossPrefab);
            enemyList.Add(boss);
        }
        else {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / bossWaveNumber, maxEnemyAmount);
            for (var i = 0; i < enemyAmount; i++) {
                // var enemy = enemyPreFabs[Random.Range(0, enemyPreFabs.Length)];
                // PoolManager.Release(enemy);
                enemyList.Add(PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));

                yield return waitTimeBetweenSpawns;
            }
        }

        yield return waitUntilNoEnemy;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);
}