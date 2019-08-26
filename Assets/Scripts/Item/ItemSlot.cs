﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using model;
using shop;
using controller;

namespace item
{
    public class ItemSlot : EventTrigger
    {
        public ItemStore _item;
        GameCore _core;
        ShopController _shopCon;
        CookController _cookCon;
        ItemController _itemCon;
        //Vector2 padding = new Vector2(20, 20);

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _shopCon = _core._menuPanel.GetComponent<ShopController>();
            _cookCon = _core._cookMenu.GetComponent<CookController>();
            _itemCon = _core._menuPanel.GetComponent<ItemController>();
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _core.SetTalk(_item.item.name + "\n<" + _item.item.detail + ">");
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _core._talkPanel.SetActive(false);
        }

        public override void OnPointerClick(PointerEventData data)
        {
            
            if (_core._shopPanel.activeSelf)
            {
                //Debug.Log("Item id " + _item.id);
                _shopCon._itemShopIsSelect = _item;
                
            }
            else
            {
                _itemCon._itemStoreIdSelect = _item;
            }
            if (_core._gameMode == _GameState.BATTLE || _core._gameMode == _GameState.CAMP)
            {
                _itemCon.UseBtn();
            }
            else
            {
                _core.OpenSubMenu(_SubMenu.Item);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (_core._actionMode == _ActionState.Cook)
            {
                if (_item.amount == 0) return;
                _item.obj.transform.Find("Count").GetComponent<Text>().text = (--_item.amount).ToString();
                _cookCon.GetRawMaterial(this.transform.Find("Icon").GetComponent<Image>().sprite, _item);
                
            }

        }
        
    }
}