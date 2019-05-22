using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : EventTrigger
{
    MainCore _core;
    ItemController _itemCon;
    ShopPanel _shopPan;

    public ItemStore _item;
    
    void Start()
    {
        _core = Camera.main.GetComponent<MainCore>();
        //_itemCon = _core._itemPanel.GetComponent<ItemController>();
        _shopPan = _core._shopPanel.GetComponent<ShopPanel>();
    }
    float lastTimeClick;

    public override void OnPointerClick(PointerEventData data)
    {
        //Debug.Log("Item id " + _item.id);
        _shopPan._itemShopIsSelect = _item;
        _core.SetTalk(_item.item.name + "\n<" + _item.item.detail+">");
        float currentTimeClick = data.clickTime;
        if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
        {
            if (_core._cutscene != null)
            {
                if (_item.itemId == 2)
                    _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._subMenuPanel.transform.Find("GridView").Find("BuyButton"));

            }
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
