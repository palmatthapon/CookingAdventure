using Core;
using Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CollectionData
{
    public class JsonReadWrite
    {
        public Setting[] ReadSetting(Setting[] dataSetting)
        {
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/W3AFile/";
            string filePath = folderPath + "Setting.json";
            if (File.Exists(filePath))
            {
                string loadData = File.ReadAllText(filePath);
                dataSetting = JsonHelper.FromJson<Setting>(loadData);
                print("load " + dataSetting.Length + " settings.");
            }
            else
            {
                TextAsset loadedLog = Resources.Load<TextAsset>("JsonDataNew/Setting_default");
                dataSetting = JsonHelper.FromJson<Setting>(loadedLog.text);
                print("load " + dataSetting.Length + " settings.");
            }
            return dataSetting;
        }

        public PlayerLog[] ReadDataPlayerLog(PlayerLog[] dataPlayerLog)
        {
            /*---android can't load
            string reader = File.ReadAllText(Application.dataPath + "/Resources/PlayerLog.json");
            dataPlayerLog = JsonHelper.FromJson<PlayerLog>(reader);*/
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/W3AFile/";
            string filePath = folderPath + "PlayerLog.json";
            if (File.Exists(filePath))
            {
                string loadData = File.ReadAllText(filePath);
                dataPlayerLog = JsonHelper.FromJson<PlayerLog>(loadData);
                print("load " + dataPlayerLog.Length + " playerLogs.");
            }
            else
            {
                Camera.main.GetComponent<MainCore>()._loadNewGame = true;
                TextAsset loadedLog = Resources.Load<TextAsset>("JsonDataNew/PlayerLog_default");
                dataPlayerLog = JsonHelper.FromJson<PlayerLog>(loadedLog.text);
                print("load " + dataPlayerLog.Length + " playerLogs.");
            }
            return dataPlayerLog;
        }

        public void WriteDataPlayerLog(PlayerLog[] dataPlayerLog)
        {
            MainCore _core = Camera.main.GetComponent<MainCore>();
            int logNumber = 0;
            dataPlayerLog[logNumber].hp = _core._playerHP;
            dataPlayerLog[logNumber].dungeonLayer = _core._currentDungeonLayer;
            dataPlayerLog[logNumber].roomPosition = _core._currentRoomPosition;
            dataPlayerLog[logNumber].money = _core._currentMoney;
            string itemStore = "";
            for (int i = 0; i < _core._itemStore.Count; i++)
            {
                itemStore += _core._itemStore[i].id + ":" + _core._itemStore[i].itemId + ":" + _core._itemStore[i].amount;
                if (i < _core._itemStore.Count - 1)
                    itemStore += ",";
            }
            dataPlayerLog[logNumber].itemStore = itemStore;
            string heroStore = "";
            for (int i = 0; i < _core._heroStore.Count; i++)
            {
                heroStore += _core._heroStore[i].id + ":" + _core._heroStore[i].heroId + ":" + _core._heroStore[i].exp + ":" + _core._heroStore[i].hp;
                if (i < _core._heroStore.Count - 1)
                    heroStore += ",";
            }
            dataPlayerLog[logNumber].heroStore = heroStore;
            dataPlayerLog[logNumber].teamSelected = _core._currentTeamIsSelect;
            string teamSetup = "";
            for (int i = 0; i < _core._teamSetup.Count; i++)
            {
                for (int j = 0; j < _core._teamSetup[i].position.Count; j++)
                {
                    teamSetup += _core._teamSetup[i].position[j].id;
                    if (j != 4)
                        teamSetup += ":";
                }
                if (i < _core._teamSetup.Count - 1)
                    teamSetup += ",";
            }
            dataPlayerLog[logNumber].teamSetup = teamSetup;
            string dungeonIsPass = "";
            for (int i = 0; i < _core._dungeon.Length; i++)
            {
                if (_core._dungeon[i].roomIsPass.Count > 0)
                {
                    if (i != 0)
                        dungeonIsPass += ",";
                    dungeonIsPass += _core._dungeon[i].dungeon.id + "_";
                    for (int a = 0; a < _core._dungeon[i].roomIsPass.Count; a++)
                    {
                        dungeonIsPass += _core._dungeon[i].roomIsPass[a].id + "-" + _core._dungeon[i].roomIsPass[a].passCount+ "-" + _core._dungeon[i].roomIsPass[a].escapeCount;
                        if (a != _core._dungeon[i].roomIsPass.Count - 1)
                            dungeonIsPass += ":";
                    }
                }
            }
            dataPlayerLog[logNumber].dungeonIsPass = dungeonIsPass;

            string shop = "";
            for (int i = 0; i < _core._landShopList.Count; i++)
            {
                if (i != 0)
                    shop += ",";
                shop += _core._landShopList[i].id + ":" + _core._landShopList[i].buyCount;
            }
            dataPlayerLog[logNumber].shopList = shop;

            string playerLogToJson = JsonHelper.ToJson<PlayerLog>(dataPlayerLog);
            //Debug.Log(playerLogToJson);
            WriteJson(playerLogToJson);
            print("Game Saved.");
        }

        public void WriteSetting(Setting[] dataSetting)
        {
            string setting = JsonHelper.ToJson<Setting>(dataSetting);
            WriteJson(setting, "Setting.json");
        }

        void WriteJson(string text, string fileName = "PlayerLog.json")
        {
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/W3AFile/";
            string filePath = folderPath + fileName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            File.WriteAllText(filePath, text);
        }
        
        void print(string txt)
        {
            Debug.Log(txt);
        }
    }
}

