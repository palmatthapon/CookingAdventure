using CollectionData;
using Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateJsonFile : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CreateDungeon(1, 0, 1, 5);

    }

    void CreateDungeon(int id,int startRoom,int lvlMin,int max)
    {
        //-------test create room in dungeon---------
        DungeonDataSet[] instance = new DungeonDataSet[100];
        for (int i = 0; i < max; i++)
        {
            int bossRoom = Random.Range(0, 100);
            instance[i] = new DungeonDataSet();
            instance[i].id = id;
            instance[i].name = "ชั้นที่ " + id;
            instance[i].startRoom = startRoom;
            instance[i].bossRoom = bossRoom;
            instance[i].monsterSetId = 1;
            instance[i].bossListId = "";
            instance[i].levelMin = lvlMin;
            instance[i].levelMax = lvlMin = lvlMin + 9;
            //instance[i].bossSecret = Random.Range(0, 100)+":";
            startRoom = bossRoom;
            id++;
            lvlMin++;
        }


        //Convert to Jason
        string playerToJason = JsonHelper.ToJson(instance, true);
        //Debug.Log(playerToJason);
        WriteJson(playerToJason);
    }

    void WriteJson(string text, string fileName = "CreateDungeon.json")
    {
        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/W3AFile/";
        string filePath = folderPath + fileName;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText(filePath, text);
    }
}
