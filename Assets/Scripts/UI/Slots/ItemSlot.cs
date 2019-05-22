using Core;
using CollectionData;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ItemSlot : EventTrigger
    {
        public ItemStore _item;
        MainCore _core;
        ItemController _itemCon;
        ShopPanel _shopPan;
        //Vector2 padding = new Vector2(20, 20);

        void Start()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _itemCon = _core._itemPanel.GetComponent<ItemController>();
            _shopPan = _core._shopPanel.GetComponent<ShopPanel>();
        }
        float lastTimeClick;

        public override void OnPointerClick(PointerEventData data)
        {
            if (_core._manageHeroPanel.activeSelf) return;
            _core.SetTalk(_item.item.name + "\n<" + _item.item.detail + ">");
            if (_core._shopPanel.activeSelf)
            {
                //Debug.Log("Item id " + _item.id);
                _shopPan._itemShopIsSelect = _item;
                
            }
            else
            {
                _itemCon._itemStoreIdSelect = _item;
            }
            float currentTimeClick = data.clickTime;
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
            {
                if (_core._cutscene != null)
                {
                    if (_item.itemId == 1)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._subMenuPanel.transform.Find("GridView").Find("SellButton"));
                }
                _core._talkPanel.SetActive(false);
                if (_core._gameMode == _GameStatus.BATTLE || _core._gameMode == _GameStatus.CAMP)
                {
                    _itemCon.UseBtn();
                }
                else
                {
                    _core.CallSubMenu(_SubMenu.Item);
                }
                
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