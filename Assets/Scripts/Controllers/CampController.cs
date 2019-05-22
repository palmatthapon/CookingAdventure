using CollectionData;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class CampController : MonoBehaviour
    {
        MainCore _core;
        
        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
        }

        void OnEnable()
        {
            //_core._storyPanelTxt.text = "รอบๆแถวนี้เหมาะที่จะตั้งแคมป์เจ้าเห็นด้วยไหม?";
            if (!_core.isPaused)
            {
                _core._campfireObj.transform.Find("Point Light").gameObject.SetActive(true);
                foreach (Transform child in _core._campfireObj.transform.Find("Fire"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        
        void SetPanel(bool set)
        {
            _core._mainMenuBG.SetActive(set);
            _core._campPanel.SetActive(set);
            //_core._menuPanel.transform.parent.gameObject.SetActive(set);
        }

        private void OnDisable()
        {
            SetPanel(false);
        }


    }

}
