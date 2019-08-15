
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using model;
using controller;

namespace menu
{
    public class RewardPanel : MonoBehaviour
    {
        GameCore _core;
        Calculate _cal;
        BattleController _battleCon;
        public Transform _gridViewHero;
        List<ItemStore> _itemDropList;
        public GameObject _itemDropSlot;
        public GameObject _heroRewardSlot;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = new Calculate();
            _battleCon = _core._battleObj.GetComponent<BattleController>();
        }

        Monster[] _monsterList;
        Dungeon _dungeon;

        private void OnEnable()
        {
            LoadData();
            LoadHeroIcon();
            DropItem();
            ExpDrop();
            AddRoomIsPass();
        }

        void LoadData()
        {
            _monsterList = _core._currentMonsterBattle;
            _dungeon = _core._dungeon[_core._currentDungeonLayer - 1];

        }
        
        void AddRoomIsPass()
        {
            foreach (Room room in _dungeon.roomIsPass)
            {
                if (room.id == _core._currentRoomPosition)
                {
                    room.passCount++;
                    break;
                }
            }
        }
        int _heroCount=1;
        GameObject _heroRewardIcon;

        void LoadHeroIcon()
        {
            Sprite[] loadSprite = null;
            string getSpriteSet = "";
            
            GameObject slot = Instantiate(_heroRewardSlot);
            slot.transform.SetParent(_gridViewHero);
            slot.transform.localScale = new Vector3(1, 1, 1);

            if (getSpriteSet != _core._heroIsPlaying.GetData().spriteSet)
            {
                getSpriteSet = _core._heroIsPlaying.GetData().spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            slot.transform.Find("Hero").Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _core._heroIsPlaying.GetData().spriteName);
            slot.transform.Find("Hero").Find("Level").GetComponent<Text>().text = "Lv. " + _core._heroIsPlaying.GetStatus().level;
            slot.transform.Find("Name").GetComponent<Text>().text = _core._heroIsPlaying.GetStatus().name;
            _heroRewardIcon = slot;
        }
        int moneyDropTotal;

        public void DropItem()
        {
            _itemDropList = new List<ItemStore>();
            
            for (int p = 0; p < _monsterList.Length; p++)
            {
                string[] itemList = _dungeon.dungeon.itemDrop.Split(',');
                int count = 0;
                for (int a = 0; a < itemList.Length; a++)
                {
                    string[] item = itemList[a].Split(':');
                    float droprate = _cal.IntParseFast(item[1])/100f;
                    //Debug.Log("drop rate " + droprate);
                    if (Random.Range(0f, 1f) <= droprate)
                    {
                        int row=0;
                        int amount = Random.Range(1, _heroCount+1);
                        do
                        {
                            if (count >= _core.dataItemList.Length)
                                count = 0;
                            if (_core.dataItemList[count].id == _cal.IntParseFast(item[0]))
                            {
                                bool haveItem = false;
                                foreach (ItemStore dropList in _itemDropList)
                                {
                                    if(dropList.itemId == _core.dataItemList[count].id)
                                    {
                                        dropList.amount += amount;
                                        haveItem = true;
                                        break;
                                    }
                                }
                                if (!haveItem)
                                {
                                    ItemStore newItem = new ItemStore();
                                    int id = 1;
                                    if (_core._itemStore.Count > 0)
                                    {
                                        _core._itemStore = _core._itemStore.OrderByDescending(o => o.id).ToList();
                                        id = _core._itemStore[_core._itemStore.Count - 1].id + 1;
                                    }
                                    newItem.id = id;
                                    newItem.itemId = _core.dataItemList[count].id;
                                    newItem.item = _core.dataItemList[count];
                                    newItem.amount = amount;
                                    _itemDropList.Add(newItem);
                                }
                                break;
                            }
                            row++;
                            count++;
                            
                        } while (row < _core.dataItemList.Length);
                        //Debug.Log("item drop row " + row);
                    }
                    else
                    {
                        moneyDropTotal += Random.Range(_dungeon.dungeon.moneyDrop / 8, _dungeon.dungeon.moneyDrop + 1);
                    }
                }
            }
            ViewItemDrop();
        }

        public void ViewItemDrop()
        {
            Transform trans = _core._rewardPanel.transform.Find("GridView").Find("ItemDrop").Find("GridView");
            Sprite[] loadSprite = null;
            string nameSpriteSet = "";
            
            foreach (ItemStore item in _itemDropList)
            {
                bool haveItem = false;
                foreach(ItemStore data in _core._itemStore)
                {
                    if(item.itemId == data.itemId)
                    {
                        data.amount += item.amount;
                        haveItem = true;
                        break;
                    }
                }
                if (!haveItem)
                {
                    _core._itemStore.Add(item);
                }
                GameObject itemSlot = Instantiate(_itemDropSlot);
                itemSlot.transform.SetParent(trans);
                itemSlot.transform.localScale = new Vector3(1, 1, 1);
                itemSlot.transform.Find("Count").GetComponent<Text>().text = "X "+item.amount;
                if (nameSpriteSet != item.item.spriteSet)
                {
                    nameSpriteSet = item.item.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.item.spriteName);
            }

            ViewMoneyDrop();

            _itemDropList.Clear();
        }

        void ViewMoneyDrop()
        {
            if (moneyDropTotal <= 0) return;
            Transform trans = _core._rewardPanel.transform.Find("GridView").Find("ItemDrop").Find("GridView");
            GameObject money = Instantiate(_itemDropSlot);
            money.transform.SetParent(trans);
            money.transform.localScale = new Vector3(1, 1, 1);
            money.transform.Find("Count").GetComponent<Text>().text = "X " + moneyDropTotal;
            Sprite[] loadMoneySprite = Resources.LoadAll<Sprite>("Sprites/UI/ui");
            money.transform.Find("Icon").GetComponent<Image>().sprite = loadMoneySprite.Single(s => s.name == "ui_27");
            _core._currentMoney += moneyDropTotal;
            moneyDropTotal = 0;
        }
        
        public void ExpDrop()
        {
            int roomPassCount=0;
            foreach (Room room in _dungeon.roomIsPass)
            {
                if (room.id == _core._currentRoomPosition)
                {
                    roomPassCount = room.passCount;
                    break;
                }
            }
            double[] expForHero = new double[_battleCon._damage_of_each_hero.GetLength(1)];
            for(int monCount=0; monCount < _battleCon._damage_of_each_hero.GetLength(0); monCount++)
            {
                int total_damage = 0;
                for(int h = 0;h< _battleCon._damage_of_each_hero.GetLength(1); h++)
                {
                    total_damage += _battleCon._damage_of_each_hero[monCount, h];
                }
                
                for (int heroCount=0; heroCount < _battleCon._damage_of_each_hero.GetLength(1); heroCount++)
                {
                    int betweenLvl = Mathf.Abs(_core._heroIsPlaying.GetStatus().level - _monsterList[monCount].GetStatus().level);
                    //Debug.Log("betweenLvl" + betweenLvl);
                    if (betweenLvl < 6)
                    {
                        expForHero[heroCount] += ((_battleCon._damage_of_each_hero[monCount, heroCount] / 100.00) * (_monsterList[monCount].GetExpDrop() + roomPassCount) / (total_damage / 100.00))* ((6-betweenLvl)/6.00);
                        Debug.Log("hero "+ heroCount + " exp A "+ expForHero[heroCount]);
                    }
                    else
                    {
                        expForHero[heroCount] += 1;
                        Debug.Log("hero " + heroCount + " exp B " + expForHero[heroCount]);
                    }
                    
                }
            }

            _heroRewardIcon.transform.Find("ExpAdd").GetComponent<Text>().text = "+" + expForHero[0];
            double expAdd = _core._heroIsPlaying.GetExp() + expForHero[0];
            int levelAdd = _cal.CalculateLevel(expAdd);
            double expMin = _cal.CalculateExp(levelAdd);
            double expMax = _cal.CalculateExp(levelAdd + 1);
            int newLevel = _cal.CalculateLevel(expAdd);

            double expBetween = _core._heroIsPlaying.GetExp() - expMin;
            double expExcess = expAdd - expMin;

            float fillExp = levelAdd > _core._heroIsPlaying.GetStatus().level ? 0 : (float)(expBetween / (expMax - expMin));
            float fillExpAdd = (float)(expExcess / (expMax - expMin));
            _heroRewardIcon.transform.Find("Slider").GetComponent<ExpSlider>().controlFillRectExp(fillExp);
            _heroRewardIcon.transform.Find("Slider").GetComponent<ExpSlider>().controlFillRectExpAdd(fillExpAdd);

            _core._heroIsPlaying.SetExp(expAdd);
            if (_core._heroIsPlaying.GetStatus().level != newLevel)
            {
                _core._heroIsPlaying.GetStatus().level = newLevel;
                _heroRewardIcon.transform.Find("Hero").Find("Level").GetComponent<Text>().text = "Lv. " + newLevel;
            }
        }

        public void ConfirmBtn()
        {
            this.gameObject.SetActive(false);
            Transform trans = _core._rewardPanel.transform.Find("GridView").Find("ItemDrop").Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
            if (_core._currentRoomPosition == _core._dungeon[_core._currentDungeonLayer-1].dungeon.bossRoom)
            {
                _core._currentDungeonLayer = _core._currentDungeonLayer + 1;
                if(_core._currentDungeonLayer > _core._dungeon.Length)
                {
                    _core._currentDungeonLayer = 1;
                    _core._currentRoomPosition = _core._dungeon[_core._currentDungeonLayer - 1].dungeon.startRoom;
                    _core.LoadScene(_GameStatus.LAND);
                }
                else
                {
                    Room newRoom = new Room();
                    newRoom.id = _core._dungeon[_core._currentDungeonLayer - 1].dungeon.startRoom;
                    newRoom.passCount = 1;
                    _core._dungeon[_core._currentDungeonLayer - 1].roomIsPass.Add(newRoom);
                    _core._currentRoomPosition = _core._dungeon[_core._currentDungeonLayer - 1].dungeon.startRoom;
                    _core.LoadScene(_GameStatus.MAP);
                }
            }
            else
            {
                _core.LoadScene(_GameStatus.MAP);
            }
        }


        public void OnDisable()
        {
            foreach(Transform child in _gridViewHero)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
