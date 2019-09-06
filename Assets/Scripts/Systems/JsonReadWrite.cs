
using Json;
using System.IO;
using UnityEngine;

namespace system
{
    public class JsonReadWrite
    {
        public Setting[] ReadSetting(Setting[] dataSetting)
        {
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/FileSave/";
            string filePath = folderPath + "Setting.json";
            if (File.Exists(filePath))
            {
                string loadData = File.ReadAllText(filePath);
                dataSetting = JsonHelper.FromJson<Setting>(loadData);
                print("load " + dataSetting.Length + " settings.");
            }
            else
            {
                TextAsset loadedLog = Resources.Load<TextAsset>("JsonDatabase/Setting_default");
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
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/FileSave/";
            string filePath = folderPath + "PlayerLog.json";
            if (File.Exists(filePath))
            {
                string loadData = File.ReadAllText(filePath);
                dataPlayerLog = JsonHelper.FromJson<PlayerLog>(loadData);
                print("load " + dataPlayerLog.Length + " playerLogs.");
            }
            else
            {
                Camera.main.GetComponent<GameCore>()._loadNewGame = true;
                TextAsset loadedLog = Resources.Load<TextAsset>("JsonDatabase/PlayerLog_default");
                dataPlayerLog = JsonHelper.FromJson<PlayerLog>(loadedLog.text);
                print("load " + dataPlayerLog.Length + " playerLogs.");
            }
            return dataPlayerLog;
        }

        public void WriteDataPlayerLog(PlayerLog[] dataPlayerLog)
        {
            GameCore _core = Camera.main.GetComponent<GameCore>();
            int logNumber = 0;
            dataPlayerLog[logNumber].soul = _core._player.currentSoul;
            dataPlayerLog[logNumber].dungeonFloor = _core._player.currentDungeonFloor;
            dataPlayerLog[logNumber].stayDungeonBlock = _core._player.currentStayDunBlock;
            dataPlayerLog[logNumber].money = _core._player.currentMoney;
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
                heroStore += _core._heroStore[i].getStoreId() + ":" + _core._heroStore[i].getId() + ":" + _core._heroStore[i].getStatus().getExp() + ":" + _core._heroStore[i].getStatus().currentHP;
                if (i < _core._heroStore.Count - 1)
                    heroStore += ",";
            }
            dataPlayerLog[logNumber].heroStore = heroStore;
            dataPlayerLog[logNumber].heroIsPlaying = _core._player._heroIsPlaying.getStoreId();
            string floor = "";
            for (int i = 0; i < _core._dungeon.Length; i++)
            {
                if (_core._dungeon[i].blockIsPlayed.Count > 0)
                {
                    if (i != 0)
                        floor += ",";
                    floor += _core._dungeon[i].data.id + "_";
                    for (int a = 0; a < _core._dungeon[i].blockIsPlayed.Count; a++)
                    {
                        floor += _core._dungeon[i].blockIsPlayed[a].getNumber() + "-" + _core._dungeon[i].blockIsPlayed[a].getPlayed()+ "-" + _core._dungeon[i].blockIsPlayed[a].getEscaped();
                        if (a != _core._dungeon[i].blockIsPlayed.Count - 1)
                            floor += ":";
                    }
                }
            }
            dataPlayerLog[logNumber].floorIsPlayed = floor;

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
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/FileSave/";
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

