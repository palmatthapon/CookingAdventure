using controller;
using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmTool : EventTrigger
{
    GameCore _core;
    FarmController _farmCon;
    //Vector2 padding = new Vector2(20, 20);

    void Start()
    {
        _core = Camera.main.GetComponent<GameCore>();
        _farmCon = _core._farmMenu.GetComponent<FarmController>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _core._talkPanel.SetActive(false);
    }

    public override void OnPointerClick(PointerEventData data)
    {
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (_core._actionMode == _ActionState.Farm)
        {
            _farmCon.GetRawMaterial();

        }

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (_core._actionMode == _ActionState.Farm)
        {
            _farmCon.PutRawMaterial();

        }

    }
}
