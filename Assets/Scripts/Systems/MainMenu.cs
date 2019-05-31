using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    RectTransform rectTransform;

    void Start()
    {
        //Fetch the RectTransform from the GameObject
        rectTransform = GetComponent<RectTransform>();
    }

    bool _switchOff=false;

    public void SwitchShow()
    {
        _switchOff = !_switchOff;
        if (_switchOff)
        {
            SetTop(rectTransform, 270);
        }
        else
        {
            SetTop(rectTransform, 100);
        }
        
    }

    void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
}
