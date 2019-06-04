using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace menu
{
    public class MenuPanel : MonoBehaviour
    {
        GameCore _core;

        float _spacing;
        RectTransform rectCurrent;
        RectTransform recTarget;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }


        void OnEnable()
        {
            
            Transform tran = transform.Find("GridView");
            recTarget = tran.Find("ItemButton").GetComponent<RectTransform>();
            _spacing = transform.Find("GridView").GetComponent<VerticalLayoutGroup>().spacing;
            transform.Find("GridView").localPosition = new Vector3(0, 0, 0);
            
        }
        

        public void UpBtn()
        {
            rectCurrent = transform.Find("GridView").GetComponent<RectTransform>();
            rectCurrent.localPosition = new Vector3(rectCurrent.localPosition.x, rectCurrent.localPosition.y - (recTarget.rect.height + _spacing), rectCurrent.localPosition.z);
            //Debug.Log("a " + rectCurrent.localPosition.x);
        }
        
        public void DownBtn()
        {
            rectCurrent = transform.Find("GridView").GetComponent<RectTransform>();
            rectCurrent.localPosition = new Vector3(rectCurrent.localPosition.x, rectCurrent.localPosition.y + recTarget.rect.height + _spacing, rectCurrent.localPosition.z);
            //Debug.Log("a "+ rectCurrent.localPosition.x);
        }
        
    }
}
