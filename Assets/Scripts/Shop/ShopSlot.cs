using UnityEngine;
using UnityEngine.EventSystems;
using system;
using controller;
using UnityEngine.UI;

namespace shop
{
    public class ShopSlot : EventTrigger
    {
        GameCore _core;

        public ItemStore _item;
        float lastTimeClick;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }
        
        public override void OnPointerClick(PointerEventData data)
        {
            //Debug.Log("Item id " + _item.id);
            _core.getShopCon()._itemShopIsSelect = _item;
            _core.getMenuCon().ViewItemDetail(this.transform.Find("Icon").GetComponent<Image>().sprite,_item.data.name + "\n<" + _item.data.detail + ">");
            float currentTimeClick = data.clickTime;
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
            {
                _core.getSubMenuCore().OpenBuyShop();
            }
            lastTimeClick = currentTimeClick;
        }
        public override void OnDeselect(BaseEventData data)
        {
            if (_core.getMenuCon()._itemDetail.activeSelf)
            {
                _core.getMenuCon()._itemDetail.SetActive(false);
            }
        }
    }
}

