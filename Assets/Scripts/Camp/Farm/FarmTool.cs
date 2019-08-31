using system;
using UnityEngine;
using UnityEngine.EventSystems;

public class FarmTool : EventTrigger
{
    GameCore _core;
    //Vector2 padding = new Vector2(20, 20);

    void Start()
    {
        _core = Camera.main.GetComponent<GameCore>();
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
        if (_core._actionMode == _ActionState.Farming)
        {
            _core.getFarmCon().GetRawMaterial();

        }

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (_core._actionMode == _ActionState.Farming)
        {
            _core.getFarmCon().PutRawMaterial();

        }

    }
}
