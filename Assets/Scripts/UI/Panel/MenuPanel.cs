using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MenuPanel : MonoBehaviour
    {
        MainCore _core;

        float _spacing;
        RectTransform rectCurrent;
        RectTransform recTarget;

        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
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

        int tutorialCount = 0;
        public void DownBtn()
        {
            if (_core._cutscene != null)
            {
                tutorialCount++;
                //if (tutorialCount == 1)
                    //_core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._menuPanel.transform.Find("GridView").Find("DefenseButton"), true,
                            //"คราวนี้มาลองตั้งท่าป้องกันดูบ้างซิ_หากเจ้าป้องกันสำเร็จ_ฮีโร่ของเจ้าจะโจมตีสวนกลับด้วยดาเมจที่ไม่สนเกาะป้องกันของมอนสเตอร์เลยนะ");
                //if (tutorialCount == 3)
                    //_core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._menuPanel.transform.Find("GridView").Find("ItemButton"), true,
                           //"ดูเหมือนเลือดของเจ้าจะลดลงนะ ลองใช้ไอเทมเพิ่มเลือดดูไหมละ");
                //if (tutorialCount == 6)
                    //_core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._menuPanel.transform.Find("GridView").Find("TeamButton"), true,
                           //"ลองมาสลับฮีโร่เป็นตัวอื่นดูบ้างไหม");
            }
            
            rectCurrent = transform.Find("GridView").GetComponent<RectTransform>();
            rectCurrent.localPosition = new Vector3(rectCurrent.localPosition.x, rectCurrent.localPosition.y + recTarget.rect.height + _spacing, rectCurrent.localPosition.z);
            //Debug.Log("a "+ rectCurrent.localPosition.x);
        }
        
    }
}
