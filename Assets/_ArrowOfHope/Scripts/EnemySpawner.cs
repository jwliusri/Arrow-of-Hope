using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyDatabase enemyDatabase;
    [SerializeField] TextAsset enemyWaveCSV;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnerOffset = -2;
    [SerializeField] int spawnPointQty = 4;
    [SerializeField] float gameEndSpawnSpamCooldown = 3f;
    [SerializeField] Enemy[] gameEndEnemies;

    public bool isInitialized { get; private set; } = false;
    public float elapsedTime { get; private set; } = 0f;
    private Stack<EnemySpawnData> enemySpawnStack;
    private GameObject[] spawnPoints;
    private bool spawnDataParsed, spawnPointCreated = false;
    private GameController gameController;
    private float gameEndSpawnSpamTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameController.Instance != null);
        gameController = GameController.Instance;

        ParseSpawnData();
        CreateSpawnerPoints();
        yield return new WaitUntil(() => spawnDataParsed && spawnPointCreated );
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController == null) return;
        if (gameController.gameState == GameController.GameState.Started)
        {
            elapsedTime += Time.deltaTime;
            while (enemySpawnStack.Count > 0 && enemySpawnStack.Peek().time < elapsedTime)
            {
                var enemySpawnData = enemySpawnStack.Pop();
                //Debug.Log($"spawn {enemySpawnData.enemyID}");
                var enemy = Instantiate(enemyPrefab, spawnPoints[enemySpawnData.spawnPointIndex].transform.position, Quaternion.identity);
                var enemyData = enemyDatabase.Find(enemySpawnData.enemyID);
                enemy.GetComponent<EnemyController>().SetEnemyData(enemyData);
            }
            if (enemySpawnStack.Count == 0 && gameEndEnemies.Length > 0)
            {
                if (gameEndSpawnSpamTimer > 0) gameEndSpawnSpamTimer -= Time.deltaTime;
                else
                {
                    for (int i = 0; i < spawnPointQty; i++)
                    {
                        var enemyData = gameEndEnemies[UnityEngine.Random.Range(0, gameEndEnemies.Length)];
                        var enemy = Instantiate(enemyPrefab, spawnPoints[i].transform.position, Quaternion.identity);
                        enemy.GetComponent<EnemyController>().SetEnemyData(enemyData);
                    }
                    gameEndSpawnSpamTimer = gameEndSpawnSpamCooldown;
                }
            }
        }
    }

    private void CreateSpawnerPoints()
    {
        var cam = Camera.main;
        var screenLeftWorldPoint = cam.ScreenToWorldPoint(new Vector3(0, 0));
        var screenRighttWorldPoint = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0));
        var screenWorldPointLength = screenRighttWorldPoint.x - screenLeftWorldPoint.x;
        var spawnerDistance = screenWorldPointLength / (spawnPointQty + 1);
        spawnPoints = new GameObject[spawnPointQty];
        for (int i = 0; i < spawnPointQty; i++)
        {
            var spawnPoint = new GameObject($"Spawn Point {i}");
            spawnPoint.transform.position = new Vector3(spawnerDistance * (i + 1) + screenLeftWorldPoint.x, screenLeftWorldPoint.y + spawnerOffset);
            spawnPoints[i] = spawnPoint;
        }
        spawnPointCreated = true;
    }

    private void ParseSpawnData()
    {
        bool skipFirstLine = true;
        string[] splitLineStrings = new String[] { "\r\n", "\r", "\n" };
        List<EnemySpawnData> list = new();
        foreach (var line in enemyWaveCSV.text.Split(splitLineStrings, StringSplitOptions.None))
        {
            if (skipFirstLine)
            {
                skipFirstLine = false;
                continue;
            }
            var stringArr = line.Split(',');
            var time = stringArr[0];
            for (int i = 1; i < stringArr.Length; i++)
            {
                //Debug.Log($"{time} {i} {stringArr[i]}");
                if (!string.IsNullOrEmpty(stringArr[i]))
                {
                    //Debug.Log($"Add to list: {i} {stringArr[i]}");
                    list.Add(new EnemySpawnData()
                    {
                        time = int.Parse(time),
                        enemyID = stringArr[i],
                        spawnPointIndex = i - 1,
                    });
                }
            }
        }
        enemySpawnStack = new(list.OrderByDescending(x => x.time));
        spawnDataParsed = true;
    }
}
