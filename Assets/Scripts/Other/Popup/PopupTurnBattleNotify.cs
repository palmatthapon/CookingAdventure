using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupTurnBattleNotify : MonoBehaviour
    {

        public Animator mPopupTurnBattleAnim;
        Text mPopupNotifyTxt;
        Text mPopupTurnTxt;

        void OnEnable()
        {
            AnimatorClipInfo[] clipInfo = mPopupTurnBattleAnim.GetCurrentAnimatorClipInfo(0);
            Destroy(gameObject, clipInfo[0].clip.length);
            mPopupNotifyTxt = mPopupTurnBattleAnim.transform.Find("NotifyText").GetComponent<Text>();
            mPopupTurnTxt = mPopupTurnBattleAnim.transform.Find("TurnText").GetComponent<Text>();
        }

        public void SetNotifyText(string text)
        {
            mPopupNotifyTxt.text = text;
        }

        public void SetTurnText(string text)
        {
            mPopupTurnTxt.text = text;
        }
    }
}
