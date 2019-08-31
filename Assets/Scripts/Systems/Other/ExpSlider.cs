using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpSlider : MonoBehaviour
{
    public RectTransform fillRectExp;
    public RectTransform fillRectExpAdd;

    public void controlFillRectExp(float input)
    {
        RectTransform rt = fillRectExp;
        rt.sizeDelta = new Vector2((input*transform.GetComponent<RectTransform>().rect.width)/1, 0);
    }

    public void controlFillRectExpAdd(float input)
    {
        RectTransform rt = fillRectExpAdd;
        rt.sizeDelta = new Vector2((input * transform.GetComponent<RectTransform>().rect.width)/1, 0);

    }
}
