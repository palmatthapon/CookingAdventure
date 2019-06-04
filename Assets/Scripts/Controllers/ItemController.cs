
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using model;
using item;

namespace controller
{
    public class ItemController : MonoBehaviour
    {
        GameCore _core;
        
        public GameObject _itemSlot;
        public Text _money;
        

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        void OnEnable()
        {
            //_core._storyPanelTxt.text = "ลองค้นกระเป๋าดูดีๆ อาจจะเจอของที่เจ้าตามหา!";
            _money.text = _core._currentMoney.ToString();
        }

        private void LateUpdate()
        {
        }

        public void ViewItem(GameObject obj)
        {
            Transform trans = obj.transform.Find("ItemMask").Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
            
            Sprite[] loadSprite =null;
            string nameSpriteSet="";
            foreach (ItemStore item in _core._itemStore)
            {
                GameObject itemSlot = Instantiate(_itemSlot);
                itemSlot.transform.SetParent(trans);
                itemSlot.transform.localScale = new Vector3(1, 1, 1);
                ItemSlot itemComp = itemSlot.GetComponent<ItemSlot>();
                itemComp._item = item;
                itemSlot.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                if (_core._gameMode == _GameStatus.BATTLE)
                    itemSlot.transform.Find("Select").localScale = new Vector3(1.2f, 1.2f, 1);
                if (nameSpriteSet != item.item.spriteSet)
                {
                    nameSpriteSet = item.item.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Item/" + nameSpriteSet);
                }
                itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.item.spriteName);
                item.obj = itemSlot;
                
            }
        }

        public void UseBtn()
        {
            UseItem();
        }

        public ItemStore _itemStoreIdSelect;

        void UseItem()
        {
            _itemStoreIdSelect.obj.transform.Find("Select").gameObject.SetActive(true);
            _core._manageHeroPanel.SetActive(true);
            _core.SubMenuCancelBtn();
        }
        
        
    }

}
