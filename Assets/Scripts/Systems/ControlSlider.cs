using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSlider : MonoBehaviour
{
    public RectTransform fill;

    public void AddFill(float input)
    {
        RectTransform rt = fill;
        rt.sizeDelta = new Vector2((input * transform.GetComponent<RectTransform>().rect.width) / 1, 0);
    }
}
