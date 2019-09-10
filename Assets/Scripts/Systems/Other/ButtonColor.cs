using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDeselectHandler
{
    public Text theText;
    Color32 oldColor;

    bool _btnPressed = false;

    private void Awake()
    {
        theText = transform.GetComponentInChildren<Text>();
        oldColor = theText.color;
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("1");
        theText.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("2");
        theText.color = Color.white;
        _btnPressed = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("3");
        _btnPressed = false;
        theText.color = oldColor;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("4");
        if (_btnPressed)
        {
            theText.color = Color.white;
        }
        else
        {
            theText.color = oldColor;
        }
    }

    public void OnDisable()
    {
        Debug.Log("5");
        theText.color = oldColor;
        _btnPressed = false;
    }


}
