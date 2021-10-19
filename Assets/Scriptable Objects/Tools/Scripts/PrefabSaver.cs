using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabSaver
{
    public static void SaveEnemyPrefabBalanceData(string filename)
    {
        var objects = Resources.LoadAll("", typeof(GameObject));
        List<EnemyData> enemies = new List<EnemyData>();

        foreach(GameObject obj in objects)
        {
            if(obj.TryGetComponent(out EnemyGeneric e))
            {
                EnemyData data = EnemyData.GetEnemyData(obj);
                enemies.Add(data);
            }
        }

        var json = JsonConvert.SerializeObject(enemies);
        Debug.Log(json);
        File.WriteAllText(Path.Combine(Application.dataPath, filename), json);
    }
}
