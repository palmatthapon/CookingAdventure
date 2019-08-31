using system;
using UnityEngine;

namespace controller
{
    public class LandController : MonoBehaviour
    {
        GameCore _core;
        MapController _mapCon;

        public GameObject _warpObj,_campfireObj,_shopObj;
        public GameObject _landPanel;
        public GameObject _gatePanel;

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
            if (!_core.IsPaused)
            {
                if (_core._gameMode == _GameState.LAND)
                {
                    if (_core.getCampCon().getAllowTouch())
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
                
                if (_core.getMenuCon().OnTouchFindTag("Gate"))
                {
                    _gatePanel.SetActive(true);
                }
                else if (_core.getMenuCon().OnTouchFindTag("Shop"))
                {
                    _core.getMenuCon()._shopMenu.SetActive(true);
                }
            }
        }
        
        public void FocusWarp()
        {
            startMarker = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            endMarker = new Vector3(_warpObj.transform.position.x, startMarker.y, startMarker.z);
            journeyLength = Vector3.Distance(startMarker, endMarker);
            startTime = Time.time;
            StartMoveCameraSmooth = true;
        }

        public void FocusCamp()
        {
            startMarker = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            endMarker = new Vector3(_campfireObj.transform.position.x, startMarker.y, startMarker.z);
            journeyLength = Vector3.Distance(startMarker, endMarker);
            startTime = Time.time;
            StartMoveCameraSmooth = true;
        }

        public void FocusShop()
        {
            startMarker = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            endMarker = new Vector3(_shopObj.transform.position.x,startMarker.y,startMarker.z);
            journeyLength = Vector3.Distance(startMarker, endMarker);
            startTime = Time.time;
            StartMoveCameraSmooth = true;
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
