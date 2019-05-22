﻿using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CollectionData
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
        public int startRoom;
        public int bossRoom;
        public int monsterSetId;
        public string bossListId;
        public int levelMin;
        public int levelMax;
        public string itemDrop;
        public int moneyDrop;
        public string shopList;
    }

    [System.Serializable]
    public class MonsterDataSet
    {
        public int id;
        public string name;
        public string spriteSet;
        public string type;
        public string skillList;
        public string baseSTR;
        public string baseAGI;
        public string baseINT;
        public string attackPattern;
    }

    [System.Serializable]
    public class HeroDataSet
    {
        public int id;
        public string name;
        public string spriteSet;
        public string spriteName;
        public _Character type;
        public string skillList;
        public int passiveId;
        public int baseSTR;
        public int baseAGI;
        public int baseINT;
        public string attackPattern;
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
        public string buffList;
        public int crystal;
    }

    [System.Serializable]
    public class Skill
    {
        public int hate;
        public List<Buff> buff = new List<Buff>();
        public SkillDataSet skill;
    }

    [System.Serializable]
    public class SkillBlock
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
        public int hp;
        public int dungeonLayer;
        public int roomPosition;
        public int money;
        public string itemStore;
        public string heroStore;
        public int teamSelected;
        public string teamSetup;
        public string dungeonIsPass;
        public bool landScene;
        public string shopList;
    }

    [System.Serializable]
    public class HeroInTeam
    {
        public List<HeroStore> position = new List<HeroStore>();
    }

    [System.Serializable]
    public class HeroStore
    {
        public int id;
        public int heroId;
        public double exp;
        int Level;
        public int STR;
        public int AGI;
        public int INT;
        public int hpMax;
        private int HP;
        public int ATK;
        public int MATK;
        public int DEF;
        public int MDEF;
        public Skill[] attack = new Skill[2];
        public _passive passive;
        public HeroDataSet hero;
        public GameObject obj;
        MainCore _core;
        BattleController _battle;
        Calculate _cal;

        public int hp
        {
            get
            {
                return this.HP;
            }
            set
            {
                if(value<0)
                    this.HP = 0;
                else if (value > hpMax)
                    this.HP = hpMax;
                else
                    this.HP = value;
                
            }
        }

        public int level
        {
            get
            {
                return this.Level;
            }
            set
            {
                this.Level = value;
                if (_cal == null)
                    _cal = new Calculate();
                STR = _cal.CalculateSTR(hero.baseSTR,hero.baseAGI,hero.baseINT,Level);
                AGI = _cal.CalculateAGI(hero.baseSTR, hero.baseAGI, hero.baseINT, Level);
                INT = _cal.CalculateINT(hero.baseSTR, hero.baseAGI, hero.baseINT, Level);
                hpMax = _cal.CalculateHpMax(STR,AGI,INT);
                hp = hpMax;
            }
        }


    }

    [System.Serializable]
    public class ItemStore
    {
        public int id;
        public int itemId;
        public int amount;
        public ItemDataSet item;
        public GameObject obj;
    }
    
    [System.Serializable]
    public class Room
    {
        public int id;
        public int passCount;
        public int escapeCount;
    }

    [System.Serializable]
    public class Dungeon
    {
        public DungeonDataSet dungeon;
        public List<Room> roomIsPass = new List<Room>();
    }
    
    [System.Serializable]
    public class Buff
    {
        public int id;
        public int icon;
        public _Model whoUse;
        public bool forMe;
        public int startTime;
        public int timeCount;
        public Vector3 originalSize;
        public bool remove;
        public GameObject obj;
    }

    [System.Serializable]
    public class Defense
    {
        public int id;
        public int crystal;
        public GameObject obj;
    }

    [System.Serializable]
    public class ItemShop
    {
        public int id;
        public int buyCount;
        public ItemDataSet item;
    }
}

