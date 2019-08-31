using System.Collections.Generic;
using UnityEngine;

namespace system
{
    [System.Serializable]
    public class Setting
    {
        public float soundValue;
        public bool question;
        public string questionLink;
    }

    [System.Serializable]
    public class DungeonDataSet
    {
        public int id;
        public string name;
        public string size;
        public int warpBlock;
        public int bossBlock;
        public string monsterIdList;
        public string bossIdList;
        public int levelMin;
        public int levelMax;
        public string itemDrop;
        public int moneyDrop;
        public string shopList;
    }

    [System.Serializable]
    public class MonsterDataList
    {
        public int id;
        public string name;
        public string spriteSet;
        public string type;
        public string skillList;
        public string baseSTR;
        public string baseAGI;
        public string baseINT;
        public string patternAttack;
    }
    
    [System.Serializable]
    public class ModelDataSet
    {
        public int id;
        public string name;
        public string spriteSet;
        public string spriteName;
        public string skillList;
        public int passiveId;
        public int baseSTR;
        public int baseAGI;
        public int baseINT;
        public string patternAttack;
    }

    [System.Serializable]
    public class SkillDataSet
    {
        public int id;
        public string name;
        public string detail;
        public float bonusDmg;
        public _Attack type;
        public string effect;
        public float delay;
        public int crystal;
    }
    
    [System.Serializable]
    public class ItemDataSet
    {
        public int id;
        public string name;
        public string detail;
        public string spriteSet;
        public string spriteName;
        public int price;
    }

    [System.Serializable]
    public class PlayerLog
    {
        public string playerName;
        public int soul;
        public int dungeonFloor;
        public int stayDungeonBlock;
        public int money;
        public string itemStore;
        public string heroStore;
        public int heroIsPlaying;
        public string floorIsPlayed;
        public bool landScene;
        public string shopList;
    }

    public class Skill
    {
        public int hate;
        public SkillDataSet data;
    }

    public class AttackBlock
    {
        public int slotId;
        public int heroStoreId;
        public int color;
        public int defCrystal;
        public int crystal;
        public bool isAttack;
        public bool isUltimate;
        public int blockStack;
        public GameObject obj;
    }

    public class ItemStore
    {
        public int id;
        public int itemId;
        public int amount;
        public ItemDataSet data;
        public GameObject obj;
    }
    
    public class DungeonBlock
    {
        int number;
        int played;
        int escaped;
        public GameObject obj;

        public DungeonBlock(int number,int played,int escaped)
        {
            this.number = number;
            this.played = played;
            this.escaped = escaped;
        }

        public int getNumber()
        {
            return number;
        }

        public int getPlayed()
        {
            return played;
        }
        public int getEscaped()
        {
            return escaped;
        }

        public void AddPlayed(int count)
        {
            played = played + count;
        }
        public void AddEscaped(int count)
        {
            escaped = escaped + count;
        }
    }
    
    public class Dungeon
    {
        public DungeonDataSet data;
        public List<DungeonBlock> blockIsPlayed = new List<DungeonBlock>();
    }
    
    public class Defense
    {
        public int id;
        public int crystal;
        public GameObject obj;
    }
    
    public class ItemShop
    {
        public int id;
        public int buyCount;
        public ItemDataSet item;
    }
}

