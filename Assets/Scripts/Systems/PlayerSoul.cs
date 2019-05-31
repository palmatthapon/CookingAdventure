using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSoul : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform fill;

    public void AddFill(int input)
    {
        float cal = (float)((100 - input )/ 100.0);
        fill.sizeDelta = new Vector2(0, cal * transform.GetComponent<RectTransform>().rect.height);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.Find("SoulText").gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.Find("SoulText").gameObject.SetActive(false);
    }
}
