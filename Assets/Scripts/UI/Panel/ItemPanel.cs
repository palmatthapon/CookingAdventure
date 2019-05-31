using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPanel : MonoBehaviour
{

    MainCore _core;
    LandController _landCon;
    Calculate _cal;

    private void Awake()
    {
        _core = Camera.main.GetComponent<MainCore>();
        _cal = new Calculate();
    }

    private void OnEnable()
    {
        _core._ActionMode = _ActionStatus.Item;
        _core._itemCon.ViewItem(this.gameObject);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
