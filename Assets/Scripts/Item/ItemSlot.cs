
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
            _shopCon = _core._mainMenu.GetComponent<ShopController>();
            _cookCon = _core._cookMenu.GetComponent<CookController>();
            _itemCon = _core._mainMenu.GetComponent<ItemController>();
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
            if (_core._manageHeroPanel.activeSelf) return;
            if (_core._shopPanel.activeSelf)
            {
                //Debug.Log("Item id " + _item.id);
                _shopCon._itemShopIsSelect = _item;
                
            }
            else
            {
                _itemCon._itemStoreIdSelect = _item;
            }
            if (_core._gameMode == _GameStatus.BATTLE || _core._gameMode == _GameStatus.CAMP)
            {
                _itemCon.UseBtn();
            }
            else
            {
                _core.CallSubMenu(_SubMenu.Item);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (_core._ActionMode == _ActionStatus.Cook)
            {
                _cookCon.GetRawMaterial(this.transform.Find("Icon").GetComponent<Image>().sprite);

            }

        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (_core._ActionMode == _ActionStatus.Cook)
            {
                _cookCon.PutRawMaterial();

            }
            
        }

        

    }
}