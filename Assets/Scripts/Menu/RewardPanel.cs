using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using model;
using controller;
using system;
using System;
using Random = UnityEngine.Random;

namespace menu
{
    public class RewardPanel : MonoBehaviour
    {
        GameCore _core;
        BattleController _battleCon;
        List<ItemStore> _itemDropList;
        public GameObject _heroRewardSlot;
        Transform trans;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _battleCon = _core._battleSpace.GetComponent<BattleController>();
            trans = transform.Find("ItemDrop").Find("GridView");
        }

        Monster[] _monsterList;
        Dungeon _dungeon;

        private void OnEnable()
        {
            LoadData();
            DropItem();
            ExpDrop();
            _core.getMapCon()._dunBlock[_core._player.currentStayDunBlock].AddPlayed(1);
        }

        void LoadData()
        {
            _monsterList = _core.getBattCon()._currentMonsterBattle;
            _dungeon = _core._dungeon[_core._player.currentDungeonFloor - 1];

        }
        
        int _heroCount=1;
        int moneyDropTotal;

        public void DropItem()
        {
            _itemDropList = new List<ItemStore>();
            
            for (int p = 0; p < _monsterList.Length; p++)
            {
                string[] itemList = _dungeon.data.itemDrop.Split(',');
                int count = 0;
                for (int a = 0; a < itemList.Length; a++)
                {
                    string[] item = itemList[a].Split(':');
                    float droprate = Calculator.IntParseFast(item[1])/100f;
                    //Debug.Log("drop rate " + droprate);
                    if (Random.Range(0f, 1f) <= droprate)
                    {
                        int row=0;
                        int amount = Random.Range(1, _heroCount+1);
                        do
                        {
                            if (count >= _core.dataItemList.Length)
                                count = 0;
                            if (_core.dataItemList[count].id == Calculator.IntParseFast(item[0]))
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
                                    newItem.data = _core.dataItemList[count];
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
                        moneyDropTotal += Random.Range(_dungeon.data.moneyDrop / 8, _dungeon.data.moneyDrop + 1);
                    }
                }
            }
            ViewItemDrop();
        }

        public void ViewItemDrop()
        {
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
                GameObject slot = Instantiate(trans.GetChild(0).gameObject);
                slot.transform.SetParent(trans);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.transform.Find("Count").GetComponent<Text>().text = "X "+item.amount;
                if (nameSpriteSet != item.data.spriteSet)
                {
                    nameSpriteSet = item.data.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                slot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.data.spriteName);
                slot.SetActive(true);
                foreach (Behaviour behaviour in slot.GetComponentsInChildren<Behaviour>())
                    behaviour.enabled = true;
            }

            ViewMoneyDrop();

            _itemDropList.Clear();
        }

        void ViewMoneyDrop()
        {
            if (moneyDropTotal <= 0) return;
            GameObject slot = Instantiate(trans.GetChild(0).gameObject);
            slot.transform.SetParent(trans);
            slot.transform.localScale = new Vector3(1, 1, 1);
            slot.transform.Find("Count").GetComponent<Text>().text = "X " + moneyDropTotal;
            Sprite[] loadMoneySprite = Resources.LoadAll<Sprite>("Sprites/UI/ui");
            slot.transform.Find("Icon").GetComponent<Image>().sprite = loadMoneySprite.Single(s => s.name == "ui_27");
            slot.SetActive(true);
            foreach (Behaviour behaviour in slot.GetComponentsInChildren<Behaviour>())
                behaviour.enabled = true;
            _core._player.currentMoney += moneyDropTotal;
            moneyDropTotal = 0;
        }
        
        public void ExpDrop()
        {
            double expDrop = 0;
            for( int i =0; i < _battleCon._monster.Count; i++)
            {
                expDrop += _battleCon._monster[i].getStatus().getExpDrop();
            }
            _core._player._heroIsPlaying.getStatus().setExp(expDrop);
        }

        public void ConfirmBtn()
        {
            this.gameObject.SetActive(false);
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
            if (_core._player.currentStayDunBlock == _core._dungeon[_core._player.currentDungeonFloor-1].data.bossBlock)
            {
                _core._player.currentDungeonFloor = _core._player.currentDungeonFloor + 1;
                if(_core._player.currentDungeonFloor > _core._dungeon.Length)
                {
                    _core._player.currentDungeonFloor = 1;
                    _core._player.currentStayDunBlock = _core._dungeon[_core._player.currentDungeonFloor - 1].data.warpBlock;
                    _core.OpenScene(_GameState.LAND);
                }
                else
                {
                    DungeonBlock newBlock = new DungeonBlock(_core._dungeon[_core._player.currentDungeonFloor - 1].data.warpBlock,
                        1,0);
                    _core._dungeon[_core._player.currentDungeonFloor - 1].blockIsPlayed.Add(newBlock);
                    _core._player.currentStayDunBlock = _core._dungeon[_core._player.currentDungeonFloor - 1].data.warpBlock;
                    _core.OpenScene(_GameState.MAP);
                }
            }
            else
            {
                _core.OpenScene(_GameState.MAP);
            }
        }

        public int getLevel(double exp)
        {
            return (int)((Math.Sqrt(100 * ((2 * exp) + 25)) + 50) / 100);
        }


        public void OnDisable()
        {
        }
    }
}
