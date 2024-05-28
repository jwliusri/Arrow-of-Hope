using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyDatabase enemyDatabase;
    [SerializeField] TextAsset enemyWaveCSV;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnerOffset = -2;
    [SerializeField] int spawnPointQty = 4;

    private Stack<EnemySpawnData> enemySpawnStack;
    private float elapsedTime = 0f;
    private GameObject[] spawnPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //List<EnemySpawnData> list = new();
        //for (int i = 0; i < spawnPointQty; i++)
        //{
        //    for (int j = 0; j < 1; j++)
        //    {

        //        list.Add(new EnemySpawnData()
        //        {
        //            time = 1 + j*2,
        //            enemyID = "dummy-enemy",
        //            spawnPointIndex = i,
        //        });
        //    }
        //}

        //enemySpawnStack = new(list.OrderByDescending(x => x.time));

        ParseSpawnData();
        CreateSpawnerPoints();
    }

    // Update is called once per frame
    void Update()
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
    }
}
