using CollectionData;
using Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controller
{
    public class ForestShopController : MonoBehaviour
    {
        MainCore _core;

        public GameObject _forestShopNpc;
        public GameObject _forestShopPanel;
        
        Sprite _bgSprite;

        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
        }

        void OnEnable()
        {
            _bgSprite = _core._bgList[Random.Range(0, _core._bgList.Length)];
            transform.GetComponent<SpriteRenderer>().sprite = _bgSprite;
            transform.Find("BGLeft").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            transform.Find("BGRight").GetComponent<SpriteRenderer>().sprite = _bgSprite;

            Camera.main.transform.position = new Vector3(_forestShopNpc.transform.position.x, -0.1f, Camera.main.transform.position.z);
            SetPanel(true);
        }

        void Update()
        {
            if (_core._gameMode == _GameStatus.FORESTSHOP)
            {
                OnTouch();
            }
        }

        void SetPanel(bool set)
        {
            _forestShopPanel.SetActive(set);
        }

        void OnTouch()
        {
            //-----touch collider2d room-----------
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (!_find)
                {
                    OnTouchFindTag("Shop");
                }
            }
            else
            {
                _find = false;
            }
        }
        bool _find = false;
        float lastTimeClick;

        public void OnTouchFindTag(string tag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
#endif
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
            if (hit.transform != null && hit.transform.tag == tag)
            {
                float currentTimeClick = Time.time;
                _core.SetTalk("ยินดีต้อนรับสู่ร้านค้ากลางป่า เจ้าจะซื้ออะไรควรคิดให้ดีๆก่อน เพราะของๆข้ามันราคาแพง!!");
                //Debug.Log("Time " + currentTimeClick);
                if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
                {
                    _find = true;
                    if (tag == "Shop")
                    {
                        _core._shopPanel.SetActive(true);
                    }
                    currentTimeClick = 0;
                }
                lastTimeClick = currentTimeClick;

            }

        }

        public void OpenShop()
        {
            _core.SetTalk("ยินดีต้อนรับสู่ร้านค้ากลางป่า เจ้าจะซื้ออะไรควรคิดให้ดีๆก่อน เพราะของๆข้ามันราคาแพง!!");
            _core._shopPanel.SetActive(true);
        }

        private void OnDisable()
        {
            _core._talkPanel.SetActive(false);
            _core._shopPanel.SetActive(false);
            SetPanel(false);
        }
    }
}

