
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using model;
using controller;

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
        _itemCon = _core._mainMenu.GetComponent<ItemController>();
    }

    private void OnEnable()
    {
        _core._ActionMode = _ActionStatus.Item;
        _itemCon.ViewItem(this.gameObject);
    }

    public void Close()
    {
        _core._itemBtn.GetComponent<Image>().sprite = _core._BagIcon[0];
        this.gameObject.SetActive(false);
    }
}
