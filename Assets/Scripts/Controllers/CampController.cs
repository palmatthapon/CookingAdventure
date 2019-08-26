using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace controller
{
    public class CampController : MonoBehaviour
    {
        GameCore _core;
        
        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        void OnEnable()
        {
            Camera.main.orthographicSize = 0.8f;
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
                if (_core._gameMode == _GameState.CAMP || _core._gameMode == _GameState.LAND)
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
                    OnTouchFindTag("Farm");
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
                        _core._cookMenu.SetActive(true);
                    }
                    else if (tag == "Hero")
                    {
                        _core._PlayerInfoPanel.SetActive(true);
                    }
                    else if (tag == "Farm")
                    {
                        _core._farmMenu.SetActive(true);
                    }

                    currentTimeClick = 0;
                }
                lastTimeClick = currentTimeClick;

            }

        }

        void SetPanel(bool set)
        {

        }

        private void OnDisable()
        {
            SetPanel(false);
        }

        


    }

}
