using item;
using system;
using shop;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace controller
{
    public class ShopController : MonoBehaviour
    {
        GameCore _core;

        public Text _headName;

        public GameObject _shopList, _itemList;

        public GameObject _secretShopNpc;
        public Text _moneyText;
        
        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        private void OnEnable()
        {
            _core._actionMode = ACTIONSTATE.Shop;
            if (_core._gameMode == GAMESTATE.SECRETSHOP)
            {
                _headName.text = "Secret Shop";
            }
            else
            {
                _headName.text = "Shop";
            }
            LoadShop();
            _core.getItemCon().ViewItem(_itemList.transform, "item");
            _core.getCampCon().setAllowTouch(false);
            _moneyText.text = _core._player.currentMoney.ToString();
        }
        
        public void MoveCameraToSecretShop()
        {
            Camera.main.transform.position = new Vector3(_secretShopNpc.transform.position.x, -0.1f, Camera.main.transform.position.z);
        }
        
        public void ViewItem()
        {
            Transform trans = _itemList.transform.Find("ItemMask").Find("GridView");
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

            Sprite[] loadSprite = null;
            string nameSpriteSet = "";
            foreach (ItemStore item in _core._itemStore)
            {
                GameObject itemSlot = Instantiate(clone);
                itemSlot.transform.SetParent(trans);
                itemSlot.transform.localScale = new Vector3(1, 1, 1);
                ItemSlot itemComp = itemSlot.GetComponent<ItemSlot>();
                itemComp._item = item;
                itemSlot.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                if (nameSpriteSet != item.data.spriteSet)
                {
                    nameSpriteSet = item.data.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.data.spriteName);
                item.obj = itemSlot;
            }
        }
        List<ItemShop> _itemShopList;

        public void LoadShop()
        {
            if (_core._gameMode == GAMESTATE.LAND)
            {
                _itemShopList = _core._landShopList;
            }
            else
            {
                string[] item = _core._dungeon[_core._player.currentDungeonFloor].data.shopList.Split(',');
                _itemShopList = new List<ItemShop>();
                for (int i = 0; i < item.Length; i++)
                {
                    foreach (ItemDataSet data in _core.dataItemList)
                    {
                        if (Calculator.IntParseFast(item[i]) == data.id)
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

        public void ViewShop()
        {
            Sprite[] loadSprite = null;
            string nameSpriteSet = "";
            Transform trans = _shopList.transform.Find("ShopMask").Find("GridView");
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

            for (int a = 0; a < _itemShopList.Count; a++)
            {
                GameObject slot = Instantiate(clone);
                slot.transform.SetParent(trans);
                slot.transform.localScale = new Vector3(1, 1, 1);
                ShopSlot script = slot.GetComponent<ShopSlot>();
                ItemStore item = new ItemStore();
                item.data = _itemShopList[a].item;
                item.itemId = _itemShopList[a].item.id;
                script._item = item;
                if (_core._gameMode == GAMESTATE.LAND)
                {
                    slot.transform.Find("Price").GetComponent<Text>().text = (_itemShopList[a].item.price + (_itemShopList[a].buyCount / 10) * 5).ToString();
                }
                else
                {
                    slot.transform.Find("Price").GetComponent<Text>().text = (_itemShopList[a].item.price + (_itemShopList[a].item.price / 2) * _itemShopList[a].buyCount).ToString();
                }
                if (nameSpriteSet != _itemShopList[a].item.spriteSet)
                {
                    nameSpriteSet = _itemShopList[a].item.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                slot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == _itemShopList[a].item.spriteName);
                slot.GetComponentInChildren<Text>().text = _itemShopList[a].item.name;
                slot.SetActive(true);
                foreach (Behaviour behaviour in slot.GetComponentsInChildren<Behaviour>())
                    behaviour.enabled = true;
            }
        }
        public ItemStore _itemShopIsSelect;

        public void SellItem()
        {
            foreach (ItemStore item in _core._itemStore.ToList())
            {
                if (_itemShopIsSelect.id == item.id)
                {
                    item.amount += -1;
                    _core._player.currentMoney += (item.data.price / 2);
                    _core.getSubMenuCore().Cancel();
                    foreach (ItemShop itemShop in _itemShopList)
                    {
                        if (itemShop.id == _itemShopIsSelect.itemId)
                        {
                            if (itemShop.buyCount >= 0)
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

        }
        Sprite iconCoin;

        public void BuyItem()
        {
            int totalPrice = 0;
            foreach (ItemShop itemShop in _itemShopList)
            {
                if (itemShop.item.id == _itemShopIsSelect.itemId)
                {
                    if (_core._gameMode == GAMESTATE.LAND)
                    {
                        totalPrice = _itemShopIsSelect.data.price + (itemShop.buyCount / 10) * 5;
                    }
                    else
                    {
                        totalPrice = _itemShopIsSelect.data.price + (_itemShopIsSelect.data.price / 4) * itemShop.buyCount;
                    }

                    if (_core._player.currentMoney < totalPrice)
                    {
                        _core.Notify(iconCoin == null ? Resources.Load<Sprite>("Sprites/Icon16x16/coin16x16") : iconCoin, "not enough money!");
                        return;
                    }
                    _core._player.currentMoney += -totalPrice;
                    itemShop.buyCount++;
                    ViewShop();
                    break;
                }
            }

            bool haveItem = false;
            foreach (ItemStore item in _core._itemStore)
            {

                if (_itemShopIsSelect.itemId == item.itemId)
                {
                    if (item.amount == 99) break;
                    item.amount++;
                    item.obj.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                    _core.getSubMenuCore().Cancel();
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
                _core.getItemCon().ViewItem(_itemList.transform,"item");
                _core.getSubMenuCore().Cancel();
            }
            _itemShopIsSelect = null;
        }

        public void OpenShop()
        {
            _core.getMenuCon()._shopMenu.SetActive(true);
        }

        public void Cancel()
        {
            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _core.getCampCon().setAllowTouch(true);
        }
    }
}

