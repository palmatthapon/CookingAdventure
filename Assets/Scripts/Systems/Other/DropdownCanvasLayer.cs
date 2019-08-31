using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownCanvasLayer : EventTrigger
{
    
    public override void OnPointerClick(PointerEventData data)
    {
        GameObject droplist = GameObject.Find("Dropdown List");

        if (droplist != null)
        {
            droplist.GetComponent<Canvas>().sortingLayerName = LayerMask.LayerToName(transform.gameObject.layer);
        }
    }
    
}
