using system;
using System.Linq;
using UnityEngine;

namespace controller
{
    public class CampController : MonoBehaviour
    {
        GameCore _core;
        Material[] _mats;

        public GameObject[] _campAvatar;
        public GameObject _campfire;

        bool _allowTouch = true;


        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _mats = Resources.LoadAll("Sprites/Character/Hero/", typeof(Material)).Cast<Material>().ToArray();
        }

        void OnEnable()
        {
            Camera.main.orthographicSize = 0.8f;
        }

        void Update()
        {
            if (_core.IsPaused)
            {
                _campfire.transform.Find("Point Light").gameObject.SetActive(false);
                foreach (Transform child in _campfire.transform.Find("Fire"))
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                _campfire.transform.Find("Point Light").gameObject.SetActive(true);
                foreach (Transform child in _campfire.transform.Find("Fire"))
                {
                    child.gameObject.SetActive(true);
                }

                if (_core._gameMode == GAMESTATE.CAMP || _core._gameMode == GAMESTATE.LAND)
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
                if (_allowTouch)
                {
                    if (_core.getMenuCon().OnTouchFindTag("Item"))
                    {
                        _core.getMenuCon().OpenBag();
                    }
                    else if (_core.getMenuCon().OnTouchFindTag("Cook"))
                    {
                        _core.getMenuCon()._cookMenu.SetActive(true);
                    }
                    else if (_core.getMenuCon().OnTouchFindTag("Hero"))
                    {
                        _core.getMenuCon()._playerInfoPanel.SetActive(true);
                    }
                    else if (_core.getMenuCon().OnTouchFindTag("Farm"))
                    {
                        _core.getMenuCon()._farmMenu.SetActive(true);
                    }
                }
            }
        }
       
        Sprite[] loadSprite = null;
        string getSpriteSet = "";
        public void LoadCampAvatar()
        {
            if (getSpriteSet != _core._player._heroIsPlaying.getSpriteSet())
            {
                getSpriteSet = _core._player._heroIsPlaying.getSpriteSet();
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            Debug.Log("camp avatar " + _core._player._heroIsPlaying.getSpriteName());
            _campAvatar[0].GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == _core._player._heroIsPlaying.getSpriteName());
            _campAvatar[0].GetComponent<SpriteRenderer>().material = _mats.Single(s => s.name == getSpriteSet);
        }

        public void setAllowTouch(bool set)
        {
            _allowTouch = set;
        }

        public bool getAllowTouch()
        {
            return _allowTouch;
        }
        private void OnDisable()
        {
        }

        


    }

}
