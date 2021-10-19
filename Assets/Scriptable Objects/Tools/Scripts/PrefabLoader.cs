using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabLoader
{
    public static void LoadEnemyPrefabBalanceData(string filename)
    {
        if(!File.Exists(Path.Combine(Application.dataPath, filename)))
        {
            Debug.Log("No enemy prefab data found");
            return;
        }

        string data = File.ReadAllText(Path.Combine(Application.dataPath, filename));
        EnemyData[] dataList = JsonConvert.DeserializeObject<EnemyData[]>(data);

        foreach (EnemyData enemy in dataList)
        {
            GameObject prefab = (GameObject)Resources.Load(enemy.name);
            Debug.Log("Loading Data for " + prefab.name);
            EnemyData.LoadEnemyData(prefab, enemy);
        }
    }
}
