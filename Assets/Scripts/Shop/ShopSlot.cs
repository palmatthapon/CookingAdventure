
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using model;
using controller;

namespace shop
{
    public class ShopSlot : EventTrigger
    {
        GameCore _core;
        ShopController _shopCon;

        public ItemStore _item;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _shopCon = _core._mainMenu.GetComponent<ShopController>();
        }
        float lastTimeClick;

        public override void OnPointerClick(PointerEventData data)
        {
            //Debug.Log("Item id " + _item.id);
            _shopCon._itemShopIsSelect = _item;
            _core.SetTalk(_item.item.name + "\n<" + _item.item.detail + ">");
            float currentTimeClick = data.clickTime;
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
            {

                _core.CallSubMenu(_SubMenu.Shop);
            }
            lastTimeClick = currentTimeClick;
        }
        public override void OnDeselect(BaseEventData data)
        {
            if (_core._talkPanel.activeSelf)
            {
                _core._talkPanel.SetActive(false);
            }
        }
    }
}

