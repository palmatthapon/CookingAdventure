
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using model;
using item;
using controller;

namespace shop
{
    public class ShopPanel : MonoBehaviour
    {
        GameCore _core;
        LandController _landCon;
        ShopController _shopCon;
        ItemController _itemCon;
        Calculate _cal;

        public GameObject _itemList;
        public GameObject _itemSlot;
        public GameObject _shopSlot;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = new Calculate();
            _itemCon = _core._menuPanel.GetComponent<ItemController>();
            _shopCon = _core._menuPanel.GetComponent<ShopController>();
        }

        void OnEnable()
        {
            _itemCon.ViewItem(_itemList);
            _shopCon.LoadShop();

        }

        

        public void SellBtn()
        {
            _shopCon.SellItem();
        }
        

        

        public void BuyBtn()
        {
           
            _shopCon.BuyItem();
        }

        

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}

