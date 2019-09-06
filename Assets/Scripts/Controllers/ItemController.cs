﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using item;
using System.Reflection;
using system;

namespace controller
{
    public class ItemController : MonoBehaviour
    {
        GameCore _core;
        
        public Text _money;

        public ItemStore _itemStoreIdSelect;

        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        public void ViewItem(Transform panel,string type)
        {
            Transform trans = panel.Find("ItemMask").Find("GridView");
            GameObject clone = trans.GetChild(0).gameObject;
            int count = 1;
            foreach (Transform child in trans)
            {
                if (count > 1)
                {
                    GameObject.Destroy(child.gameObject);
                }
                count++;
            }
            
            foreach (ItemStore item in _core._itemStore.ToList())
            {
                if (type == item.data.spriteSet)
                {
                    GameObject slot = Instantiate(clone);
                    slot.transform.SetParent(trans);
                    slot.transform.localScale = new Vector3(1, 1, 1);
                    ItemSlot script = slot.GetComponent<ItemSlot>();
                    script._item = item;
                    slot.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                    if (getSpriteSet != item.data.spriteSet)
                    {
                        getSpriteSet = item.data.spriteSet;

                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + getSpriteSet);
                    }
                    slot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.data.spriteName);
                    slot.SetActive(true);
                    foreach (Behaviour behaviour in slot.GetComponentsInChildren<Behaviour>())
                            behaviour.enabled = true;
                    item.obj = slot;
                }
            }
        }
        
        public void UseItem()
        {
            Debug.Log("Use item");
            if (_core._actionMode == _ActionState.Item)
            {
                if (_core.getMenuCon().CheckCrystal(Constants._crystalItem))
                {
                    foreach (ItemStore item in _core._itemStore.ToList())
                    {
                        if (_itemStoreIdSelect.id == item.id)
                        {
                            if (CallItemActive((_Item)_itemStoreIdSelect.itemId, _core._player._heroIsPlaying))
                            {

                                item.amount -= 1;
                                if (item.amount == 0)
                                {
                                    Destroy(item.obj);
                                    _core._itemStore.Remove(item);
                                    _core.getSubMenuCore().Cancel();
                                    return;
                                }
                                item.obj.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                                _core.getBattCon()._battleMode = _BattleState.Finish;
                                _core.getMenuCon().UseCrystal(Constants._crystalItem);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    _core.OpenErrorNotify("คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย " + Constants._crystalItem);
                }
            }
            else
            {
                if (_core.getMenuCon().UseCrystal(Constants._crystalTeam))
                {
                    _core._infoPanel.SetActive(false);
                }
                else
                {
                    _core.OpenErrorNotify("คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย " + Constants._crystalTeam);
                }

            }
        }
        
        bool CallItemActive(_Item methodId, params object[] args)
        {
            string methodName = methodId.ToString();

            Type type = typeof(ItemActive);
            MethodInfo method = type.GetMethod(methodName);
            ItemActive c = new ItemActive();
            return (bool)method.Invoke(c, args);
        }

        
    }

}
