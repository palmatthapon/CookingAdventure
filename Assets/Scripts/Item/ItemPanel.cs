
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using model;
using controller;
using System.Linq;

public class ItemPanel : MonoBehaviour
{
    GameCore _core;
    LandController _landCon;
    ItemController _itemCon;
    Calculate _cal;

    private void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
        _cal = new Calculate();
        _itemCon = _core._menuPanel.GetComponent<ItemController>();
    }

    private void OnEnable()
    {
        _core._actionMode = _ActionState.Item;
        _itemCon.ViewItem(this.gameObject);
    }

    public void Close()
    {
        _core._menuCon.gridViewTrans.Find("BagButton").GetComponent<Image>().sprite = _core._uiSprite2.Single(s => s.name == "bagClose"); ;
        this.gameObject.SetActive(false);
    }
}
