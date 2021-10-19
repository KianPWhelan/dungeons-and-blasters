using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONTools : ScriptableObject
{
    [SerializeField]
    private string fileName;

    public void SaveGameObjectsToJSONTextFile()
    {
        PrefabSaver.SaveEnemyPrefabBalanceData(fileName);
    }

    public void LoadEnemyPrefabBalanceDataFromTextFile()
    {
        PrefabLoader.LoadEnemyPrefabBalanceData(fileName);
    }
}
