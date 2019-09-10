using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour
{
    public Animator mPopupAnim;
    public Text mPopupTxt;
    public Image mIcon;

    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = mPopupAnim.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
    }

    public void SetPopupText(string text)
    {

        mPopupTxt.text = text;
    }

    public void SetIcon(Sprite icon)
    {

        mIcon.sprite = icon;
    }


}


