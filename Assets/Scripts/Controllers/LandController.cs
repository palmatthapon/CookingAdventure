using model;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace controller
{
    public class LandController : MonoBehaviour
    {
        GameCore _core;
        MapController _mapCon;

        public GameObject _warpObj,_campfireObj,_shopObj;
        public GameObject _landPanel;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        void OnEnable()
        {
            Camera.main.orthographicSize = 0.8f;
            SetPanel(true);
        }

        void Update()
        {
            if (!_core.isPaused)
            {
                if (_core._gameMode == _GameState.LAND)
                {
                    if (!_core._shopPanel.activeSelf && !_core._gatePanel.activeSelf && !_core._CharacterPanel.activeSelf && !_core._subMenuPanel.activeSelf)
                    {
                        CameraMove();
                    }
                    OnTouch();
                }

                if (StartMoveCameraSmooth)
                {
                    MoveCameraSmooth();
                }
            }
        }

        bool _npcTalk;

        void SetPanel(bool set)
        {
            _landPanel.SetActive(set);
                
        }

        public float speed = 0.005F;
        Vector3 delta = Vector3.zero;
        Vector3 lastPos = Vector3.zero;
        public float rightBound;
        public float leftBound;

        public void CameraMove()
        {
            Vector2 touchDeltaPosition = Camera.main.transform.position;
            Vector3 pos = Camera.main.transform.position;
#if UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                
                if (pos.x >= leftBound || pos.x <= rightBound)
                {
                    touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                }
                //Right
                if (pos.x >= rightBound)
                {
                    touchDeltaPosition.x = rightBound;
                }
                //Left
                if (pos.x <= leftBound)
                {
                    touchDeltaPosition.x = leftBound;
                }
                
                Camera.main.transform.Translate(-touchDeltaPosition.x * speed, 0, 0);
            }
#endif
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                lastPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                delta = Input.mousePosition - lastPos;
                //Debug.Log(leftBound + "/" + pos.x + "/" + rightBound);
                if (pos.x >= leftBound || pos.x <= rightBound)
                {
                    touchDeltaPosition = delta;
                }
                //Right
                if (pos.x >= rightBound)
                {
                    touchDeltaPosition.x = rightBound;
                }
                //Left
                if (pos.x <= leftBound)
                {
                    touchDeltaPosition.x = leftBound;
                }
                
                Camera.main.transform.Translate(-touchDeltaPosition.x * speed, 0, 0);
                lastPos = Input.mousePosition;
            }
#endif
            
        }
        void OnTouch()
        {
            //-----touch collider2d room-----------
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (!_find)
                {
                    _find = true;
                    OnTouchFindTag("Gate");
                    OnTouchFindTag("Shop");
                    OnTouchFindTag("CampHero");
                }
            }
            else
            {
                _find = false;
            }
        }
        bool _find = false;
        float lastTimeClick=0;

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
                    if (tag == "Gate")
                    {
                        _core._gatePanel.SetActive(true);
                    }
                    else if (tag == "Shop")
                    {
                        _core._shopPanel.SetActive(true);
                        if (_core._shopPanel.activeSelf)
                        {
                            FocusPosition(hit.transform);
                        }
                    }else if (tag == "CampHero")
                    {
                        _core._CharacterPanel.SetActive(true);
                    }
                       
                    currentTimeClick = 0;
                }
                lastTimeClick = currentTimeClick;
                
            }

        }

        public void FocusPosition(Transform trans)
        {
            Camera.main.transform.position = new Vector3(trans.position.x,
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z);
        }

        public void FocusWarpBtn()
        {
            startMarker = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            endMarker = new Vector3(_warpObj.transform.position.x, startMarker.y, startMarker.z);
            journeyLength = Vector3.Distance(startMarker, endMarker);
            startTime = Time.time;
            StartMoveCameraSmooth = true;
            //FocusPosition(_warpObj.transform);
            _core._talkPanel.SetActive(false);
            _core._gatePanel.SetActive(true);
            _core._CharacterPanel.SetActive(false);
            _core._shopPanel.SetActive(false);
        }

        public void FocusCampBtn()
        {
            startMarker = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            endMarker = new Vector3(_campfireObj.transform.position.x, startMarker.y, startMarker.z);
            journeyLength = Vector3.Distance(startMarker, endMarker);
            startTime = Time.time;
            StartMoveCameraSmooth = true;
            //FocusPosition(_campfireObj.transform);
            _core._talkPanel.SetActive(false);
            _core._CharacterPanel.SetActive(true);
            _core._gatePanel.SetActive(false);
            _core._shopPanel.SetActive(false);
        }

        public void FocusShopBtn()
        {
            startMarker = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            endMarker = new Vector3(_shopObj.transform.position.x,startMarker.y,startMarker.z);
            journeyLength = Vector3.Distance(startMarker, endMarker);
            startTime = Time.time;
            StartMoveCameraSmooth = true;
            //FocusPosition(_shopObj.transform);
            _core._talkPanel.SetActive(false);
            _core._shopPanel.SetActive(true);
            _core._CharacterPanel.SetActive(false);
            _core._gatePanel.SetActive(false);
        }
        Vector3 startMarker;
        Vector3 endMarker;
        public float speedSmooth = 1f;
        private float startTime;
        private float journeyLength;
        bool StartMoveCameraSmooth = false;

        void MoveCameraSmooth()
        {
            
            if (Camera.main.transform.position.x == endMarker.x)
            {
                StartMoveCameraSmooth = false;
            }
            else
            {
                float distCovered = (Time.time - startTime) * speedSmooth;
                float fracJourney = distCovered / journeyLength;
                Camera.main.transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
            }
        }

        private void OnDisable()
        {
            SetPanel(false);
        }
    }
}
