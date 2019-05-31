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
        ShopPanel _shopPan;
        CookController _cookCon;
        //Vector2 padding = new Vector2(20, 20);

        void Start()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _shopPan = _core._shopPanel.GetComponent<ShopPanel>();
            _cookCon = _core._cookMenu.GetComponent<CookController>();
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _core.SetTalk(_item.item.name + "\n<" + _item.item.detail + ">");
        }

        float lastTimeClick;

        public override void OnPointerClick(PointerEventData data)
        {
            if (_core._manageHeroPanel.activeSelf) return;
            if (_core._shopPanel.activeSelf)
            {
                //Debug.Log("Item id " + _item.id);
                _shopPan._itemShopIsSelect = _item;
                
            }
            else
            {
                _core._itemCon._itemStoreIdSelect = _item;
            }
            if (_core._gameMode == _GameStatus.BATTLE || _core._gameMode == _GameStatus.CAMP)
            {
                _core._itemCon.UseBtn();
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

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _core._talkPanel.SetActive(false);
        }

    }
}