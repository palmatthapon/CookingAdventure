using controller;
using model;
using Model;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace controller
{
    public class MenuController : MonoBehaviour
    {
        GameCore _core;
        RectTransform _rtrans;
        public GameObject _playerLifePanel;
        public Transform gridViewTrans;

        void Awake()
        {
            //Fetch the RectTransform from the GameObject
            _rtrans = GetComponent<RectTransform>();
            _core = Camera.main.GetComponent<GameCore>();
            gridViewTrans = transform.Find("MenuMask").Find("GridView");
        }

        public GameObject _mapBtn;

        private void OnEnable()
        {
            
            transform.GetComponent<PlayerController>().LoadCampAvatar();
        }

        public void setIconMapBtn(string name)
        {
            _mapBtn.GetComponent<Image>().sprite = _core._uiSprite2.Single(s => s.name == name);
        }

        bool _switchOff = false;
        float _oldHeight;

        public void SwitchShow()
        {
            _switchOff = !_switchOff;
            if (_switchOff)
            {
                _oldHeight = _rtrans.sizeDelta.y;
                _playerLifePanel.transform.Find("UltimateSlider").gameObject.SetActive(false);
                _playerLifePanel.transform.Find("HPSlider").gameObject.SetActive(false);
                transform.Find("MenuMask").gameObject.SetActive(false);
                SetHeight(0);
            }
            else
            {
                SetHeight(_oldHeight);
                _playerLifePanel.transform.Find("UltimateSlider").gameObject.SetActive(true);
                _playerLifePanel.transform.Find("HPSlider").gameObject.SetActive(true);
                transform.Find("MenuMask").gameObject.SetActive(true);
            }

        }

        void SetHeight(float height)
        {
            _rtrans.sizeDelta = new Vector2(_rtrans.sizeDelta.x, height);
        }

        void SetTop(RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public void OpenBag()
        {
            Debug.Log("Open bag");
            _core._actionMode = _ActionState.Item;
            if (_core._itemPanel.activeSelf)
            {
                gridViewTrans.Find("BagButton").GetComponent<Image>().sprite = _core._uiSprite2.Single(s => s.name == "bagClose");
                _core._itemPanel.SetActive(false);
                if (_core._gameMode == _GameState.BATTLE)
                    _core._attackPanel.SetActive(true);
            }
            else
            {
                gridViewTrans.Find("BagButton").GetComponent<Image>().sprite = _core._uiSprite2.Single(s => s.name == "bagOpen");
                _core.OpenActionPanel(_core._itemPanel);
            }
        }

        public void OpenMap()
        {
            _core._actionMode = _ActionState.Map;
            if (_core._gameMode == _GameState.BATTLE)
            {
                if (!_core._battleCon._isEscape)
                {
                    _core._battleCon._isEscape = true;
                    if (_core.UseCrystal(2))
                    {
                        if (Random.Range(0f, 1f) <= _core._mapCon._escapeRate)
                        {
                            _core.LoadScene(_GameState.MAP);
                            _core.CalEscapeRoom();
                        }
                        else
                        {
                            _core._battleCon.ShowDamage("Escape Fail!", _core._battleCon.FocusHero().getAvatarPos());
                            _core._battleCon.EndTurnSpeed();
                            _core._mapCon._escapeRate += 0.05f;
                        }

                    }
                    else
                    {
                        _core.OpenErrorNotify("คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย 2");
                    }
                }

            }
            else if (_core._gameMode == _GameState.MAP)
            {
                _core._mapCon.FocusPosition();
            }
            else
            {
                _core._CharacterPanel.SetActive(false);
                _core._itemPanel.SetActive(false);
                _core.LoadScene(_GameState.MAP);
            }


        }
        
        public void OpenSystem()
        {
            if (_core._subMenuMode == _SubMenu.GameMenu && _core._subMenuPanel.activeSelf)
            {
                if (!_core._settingPanel.activeSelf)
                    _core._subMenuPanel.SetActive(false);
            }
            else
            {
                _core.OpenSubMenu(_SubMenu.GameMenu);
            }
        }

        public void OpenCook()
        {
            _core._cookMenu.SetActive(true);
        }

    }
}

