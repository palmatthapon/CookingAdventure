using system;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace item
{
    public class ItemSlot : EventTrigger
    {
        GameCore _core;

        public ItemStore _item;
        float lastTimeClick;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _core.SetTalk(_item.data.name + "\n<" + _item.data.detail + ">");
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _core._talkPanel.SetActive(false);
        }

        public override void OnPointerClick(PointerEventData data)
        {
            float currentTimeClick = data.clickTime;
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
            {
                if (_core.getMenuCon()._shopMenu.activeSelf)
                {
                    _core.getShopCon()._itemShopIsSelect = _item;

                }
                else
                {
                    _core.getItemCon()._itemStoreIdSelect = _item;
                }
                if (_core._gameMode == _GameState.BATTLE)
                {
                    _core.getItemCon().UseItem();
                }
                else if (_core._actionMode == _ActionState.Shop)
                {
                    _core.getSubMenuCore().OpenSellItem();
                }
                else
                {
                    _core.getSubMenuCore().OpenUseItem();
                }
            }
            lastTimeClick = currentTimeClick;
            
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (_core._actionMode == _ActionState.Cooking)
            {
                if (_item.amount == 0) return;
                _item.obj.transform.Find("Count").GetComponent<Text>().text = (--_item.amount).ToString();
                _core.getCookCon().GetRawMaterial(this.transform.Find("Icon").GetComponent<Image>().sprite, _item);
                
            }

        }
        
    }
}