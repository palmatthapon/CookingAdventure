using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;
using CollectionData;
using UnityEngine.UI;
using System.Linq;

namespace UI
{
    public class ShopPanel : MonoBehaviour
    {
        MainCore _core;
        LandController _landCon;
        Calculate _cal;
        PlayerInfoPanel _plyInfoPan;

        public GameObject _shop,_item;
        public GameObject _itemSlot;
        public GameObject _shopSlot;

        void OnEnable()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _cal = new Calculate();
            ViewItem();
            LoadShop();
            _plyInfoPan = _core._playerInfoPanel.GetComponent<PlayerInfoPanel>();
            _plyInfoPan.SetObjPanel(transform.gameObject);

        }

        void ViewItem()
        {
            Transform trans = _item.transform.Find("ItemMask").Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }

            Sprite[] loadSprite = null;
            string nameSpriteSet = "";
            foreach (ItemStore item in _core._itemStore)
            {
                GameObject itemSlot = Instantiate(_itemSlot);
                itemSlot.transform.SetParent(trans);
                itemSlot.transform.localScale = new Vector3(1, 1, 1);
                ItemSlot itemComp = itemSlot.GetComponent<ItemSlot>();
                itemComp._item = item;
                itemSlot.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                if (nameSpriteSet != item.item.spriteSet)
                {
                    nameSpriteSet = item.item.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                itemSlot.transform.Find("ItemImage").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.item.spriteName);
                item.obj = itemSlot;
            }
        }
        List<ItemShop> _itemShopList;

        void LoadShop()
        {
            if (_core._gameMode == _GameStatus.LAND)
            {
                _itemShopList = _core._landShopList;
            }
            else
            {
                string[] item = _core._dungeon[_core._currentDungeonLayer].dungeon.shopList.Split(',');
                _itemShopList = new List<ItemShop>();
                for (int i = 0; i < item.Length; i++)
                {
                    foreach (ItemDataSet data in _core.dataItemList)
                    {
                        if (_cal.IntParseFast(item[i]) == data.id)
                        {
                            ItemShop newItem = new ItemShop();
                            newItem.item = data;
                            newItem.buyCount = 0;
                            _itemShopList.Add(newItem);
                            break;
                        }
                    }
                }
            }
            ViewShop();
        }

        void ViewShop()
        {
            Sprite[] loadSprite = null;
            string nameSpriteSet = "";
            Transform trans = _shop.transform.Find("ShopMask").Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (int a = 0; a < _itemShopList.Count; a++)
            {
                GameObject itemSlot = Instantiate(_shopSlot);
                itemSlot.transform.SetParent(trans);
                itemSlot.transform.localScale = new Vector3(1, 1, 1);
                ShopSlot itemComp = itemSlot.GetComponent<ShopSlot>();
                ItemStore data = new ItemStore();
                data.item = _itemShopList[a].item;
                data.itemId = _itemShopList[a].item.id;
                itemComp._item = data;
                if (_core._gameMode == _GameStatus.LAND)
                {
                    itemSlot.transform.Find("Price").GetComponent<Text>().text = (_itemShopList[a].item.price + (_itemShopList[a].buyCount / 10) *5).ToString();
                }
                else
                {
                    itemSlot.transform.Find("Price").GetComponent<Text>().text = (_itemShopList[a].item.price + (_itemShopList[a].item.price / 2) * _itemShopList[a].buyCount).ToString();
                }
                if (nameSpriteSet != _itemShopList[a].item.spriteSet)
                {
                    nameSpriteSet = _itemShopList[a].item.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                itemSlot.transform.Find("ItemImage").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == _itemShopList[a].item.spriteName);
                if (_core._cutscene != null)
                {
                    if (a == 1)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(itemSlot.transform);

                }
            }
        }

        public void SellBtn()
        {
            SellItem();
        }
        public ItemStore _itemShopIsSelect;

        void SellItem()
        {
            foreach (ItemStore item in _core._itemStore.ToList())
            {
                if (_itemShopIsSelect.id == item.id)
                {
                    item.amount += - 1;
                    _core._currentMoney += (item.item.price / 2);
                    _core._playerInfoPanel.transform.Find("GridView").Find("Money").GetComponent<Text>().text = _core._currentMoney.ToString();
                    _core.SubMenuCancelBtn();
                    foreach (ItemShop itemShop in _itemShopList)
                    {
                        if (itemShop.id == _itemShopIsSelect.itemId)
                        {
                            if(itemShop.buyCount>=0)
                                itemShop.buyCount--;
                            ViewShop();
                            break;
                        }
                    }
                    _itemShopIsSelect = null;
                    if (item.amount == 0)
                    {
                        Destroy(item.obj);
                        _core._itemStore.Remove(item);
                        return;
                    }
                    item.obj.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                    break;
                }
            }
            if (_core._cutscene != null)
            {
                _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._landObj.GetComponent<LandController>()._landPanel.transform.Find("GridView").Find("WarpButton"),
                            true, "ตอนนี้เจ้าคงพอซื้อขายไอเทมเป็นแล้วซินะ ต่อไปเรามาเริ่มออกเดินทางกันดีกว่านะ");
            }
        }

        public void BuyBtn()
        {
           
            BuyItem();
        }

        void BuyItem()
        {
            int totalPrice = 0;
            foreach (ItemShop itemShop in _itemShopList)
            {
                if (itemShop.item.id == _itemShopIsSelect.itemId)
                {
                    if (_core._gameMode == _GameStatus.LAND)
                    {
                        totalPrice = _itemShopIsSelect.item.price + (itemShop.buyCount / 10) * 5;
                    }
                    else
                    {
                        totalPrice = _itemShopIsSelect.item.price + (_itemShopIsSelect.item.price / 4) * itemShop.buyCount;
                    }

                    if (_core._currentMoney < totalPrice)
                    {
                        //_core.CallSubMenu(_SubMenu.Alert, "จำนวนเงินของเจ้าไม่พอใช้ง่าย!");
                        _core.OpenErrorNotify("จำนวนเงินของเจ้าไม่พอใช้ง่าย!");
                        return;
                    }
                    _core._currentMoney += -totalPrice;
                    _core._playerInfoPanel.transform.Find("GridView").Find("Money").GetComponent<Text>().text = _core._currentMoney.ToString();
                    itemShop.buyCount++;
                    ViewShop();
                    break;
                }
            }

            bool haveItem = false;
            foreach (ItemStore item in _core._itemStore)
            {
                if (_core._cutscene != null)
                {
                    if(item.itemId == 1)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(item.obj.transform);
                }
                if (_itemShopIsSelect.itemId == item.itemId)
                {
                    if (item.amount == 99) break;
                    item.amount++;
                    item.obj.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                    _core.SubMenuCancelBtn();
                    haveItem = true;
                    break;
                }
            }
            
            if (!haveItem)
            {
                _core._itemStore = _core._itemStore.OrderBy(o => o.id).ToList();
                _itemShopIsSelect.id = _core._itemStore[_core._itemStore.Count - 1].id + 1;
                //Debug.Log("add newitem id : " + _itemShopIsSelect.id);
                _itemShopIsSelect.amount = 1;
                _core._itemStore.Add(_itemShopIsSelect);
                ViewItem();
                _core.SubMenuCancelBtn();
            }
            _itemShopIsSelect = null;
        }
    }
}

