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

        void Update()
        {
            if (!_core.isPaused)
            {
                if (_core._gameMode == _GameStatus.CAMP)
                {
                    OnTouch();
                }
                
            }
        }


        void OnTouch()
        {
            //-----touch collider2d room-----------
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (!_find)
                {
                    _find = true;
                    OnTouchFindTag("Item");
                    OnTouchFindTag("Cook");
                    OnTouchFindTag("Hero");
                }
            }
            else
            {
                _find = false;
            }
        }
        bool _find = false;
        float lastTimeClick = 0;

        public void OnTouchFindTag(string tag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
#endif
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
            if (hit.transform != null && hit.transform.tag == tag)
            {
                float currentTimeClick = Time.time;
                //Debug.Log("Time " + currentTimeClick);
                if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
                {
                    if (tag == "Item")
                    {
                        _core._itemPanel.SetActive(true);
                    }
                    else if (tag == "Cook")
                    {
                        Debug.Log("Open Cook function");
                    }
                    else if (tag == "Hero")
                    {
                        _core._teamPanel.SetActive(true);
                    }

                    currentTimeClick = 0;
                }
                lastTimeClick = currentTimeClick;

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
