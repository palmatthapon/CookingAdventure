using UI;
using Core;
using CollectionData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class ItemController : MonoBehaviour
    {
        MainCore _core;
        
        public GameObject _itemSlot;
        public Text _money;

        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
        }

        void OnEnable()
        {
            //_core._storyPanelTxt.text = "ลองค้นกระเป๋าดูดีๆ อาจจะเจอของที่เจ้าตามหา!";
            _money.text = _core._currentMoney.ToString();
            ViewItem();
        }

        private void LateUpdate()
        {
        }

        void ViewItem()
        {
            Transform trans = _core._itemPanel.transform.Find("ItemMask").Find("GridView");
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
                itemSlot.transform.Find("ItemImage").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == item.item.spriteName);
                item.obj = itemSlot;
                if (_core._cutscene != null)
                {
                    if(item.itemId == 2)
                    _core._cutscene.GetComponent<Cutscene>().TutorialPlay(itemSlot.transform);

                }
            }
        }

        public void UseBtn()
        {
            if (_core._cutscene != null)
            {
                 _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._manageHeroPanel.transform.Find("ConfirmButton"));
            }
            UseItem();
        }

        public ItemStore _itemStoreIdSelect;

        void UseItem()
        {
            _itemStoreIdSelect.obj.transform.Find("Select").gameObject.SetActive(true);
            _core._manageHeroPanel.SetActive(true);
            _core.SubMenuCancelBtn();
        }
        
        private void OnDisable()
        {
        }
    }

}
