using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupText : MonoBehaviour
    {
        
        public Animator mPopupAnim;
        public Text mPopupTxt;

        void OnEnable()
        {
            AnimatorClipInfo[] clipInfo = mPopupAnim.GetCurrentAnimatorClipInfo(0);
            Destroy(gameObject, clipInfo[0].clip.length);
        }

        public void SetPopupText(string text)
        {

            mPopupTxt.text = text;
        }
    }
}

