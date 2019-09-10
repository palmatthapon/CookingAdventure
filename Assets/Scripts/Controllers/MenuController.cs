using system;
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

        public GameObject _itemMenu;
        public GameObject _playerInfoPanel;
        public GameObject _attackMenu;
        public GameObject _shopMenu;
        public GameObject _cookMenu;
        public GameObject _farmMenu;
        public GameObject _mapBtn;
        public GameObject _itemDetail;

        float lastTimeClick;

        void Awake()
        {
            //Fetch the RectTransform from the GameObject
            _rtrans = GetComponent<RectTransform>();
            _core = Camera.main.GetComponent<GameCore>();
            gridViewTrans = transform.Find("MenuMask").Find("GridView");
        }

        void Update()
        {
            if (_core._gameMode == GAMESTATE.SECRETSHOP)
            {
                OnTouchSecretShop();
            }
        }

        void OnTouchSecretShop()
        {
            //-----touch collider2d room-----------
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (OnTouchFindTag("Shop"))
                {
                    _core.getMenuCon()._shopMenu.SetActive(true);
                }
            }
        }
        
        public bool OnTouchFindTag(string tag)
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
                if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
                {
                    currentTimeClick = 0;
                    return true;
                }
                lastTimeClick = currentTimeClick;
            }
            return false;
        }

        public void setIconMapBtn(string name)
        {
            _mapBtn.GetComponent<Image>().sprite = _core._iconMenu.Single(s => s.name == name);
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
            _core._actionMode = ACTIONSTATE.Item;
            if (_itemMenu.activeSelf)
            {
                gridViewTrans.Find("BagButton").GetComponent<Image>().sprite = _core._iconMenu.Single(s => s.name == "bag24x24");
                _itemMenu.SetActive(false);
                _core.getCampCon().setAllowTouch(true);
            }
            else
            {
                gridViewTrans.Find("BagButton").GetComponent<Image>().sprite = _core._iconMenu.Single(s => s.name == "bagOpen24x24");
                _itemMenu.SetActive(true);
                _core.getItemCon().ViewItem(_itemMenu.transform,"item");
                _core.getItemCon()._money.text = _core._player.currentMoney.ToString();
                _core.getCampCon().setAllowTouch(false);
                if (_core._gameMode == GAMESTATE.BATTLE)
                    _attackMenu.SetActive(false);
                if (_core._gameMode == GAMESTATE.LAND)
                    _core.getLandCon().FocusCamp();
            }
        }
        public void BagViewItem()
        {
            _itemMenu.transform.Find("RawMaterialButton").GetComponent<Image>().sprite = _itemMenu.transform.Find("ItemButton").GetComponent<Image>().sprite;
            _itemMenu.transform.Find("ItemButton").GetComponent<Image>().sprite = _core._uiSprite2.Single(s => s.name == "HeadPanel");
            _itemMenu.transform.Find("RawMaterialButton").GetComponent<Button>().enabled = true;
            _itemMenu.transform.Find("ItemButton").GetComponent<Button>().enabled = false;
            _core.getItemCon().ViewItem(_itemMenu.transform, "item");
        }

        public void BagViewRawMaterial()
        {
            _itemMenu.transform.Find("ItemButton").GetComponent<Image>().sprite = _itemMenu.transform.Find("RawMaterialButton").GetComponent<Image>().sprite;
            _itemMenu.transform.Find("RawMaterialButton").GetComponent<Image>().sprite = _core._uiSprite2.Single(s => s.name == "HeadPanel");
            _itemMenu.transform.Find("RawMaterialButton").GetComponent<Button>().enabled = false;
            _itemMenu.transform.Find("ItemButton").GetComponent<Button>().enabled = true;
            _core.getItemCon().ViewItem(_itemMenu.transform, "rawmaterial");

        }

        public void OpenCamp()
        {
            if(_core._player.currentStayDunBlock==_core._dungeon[_core._player.currentDungeonFloor - 1].data.warpBlock)
            {
                _core.OpenScene(GAMESTATE.LAND);
            }
            else
            {
                _core.OpenScene(GAMESTATE.CAMP);

            }
        }
        
        public void OpenMap()
        {
            _core._actionMode = ACTIONSTATE.Map;
            if (_core._gameMode == GAMESTATE.BATTLE)
            {
                if (!_core.getBattCon()._isEscape)
                {
                    _core.getBattCon()._isEscape = true;
                    if (UseCrystal(2))
                    {
                        if (Random.Range(0f, 1f) <= _core.getMapCon()._escapeRate)
                        {
                            _core.OpenScene(GAMESTATE.MAP);
                            _core.getMapCon()._dunBlock[_core._player.currentStayDunBlock].AddEscaped(1);
                        }
                        else
                        {
                            _core.getBattCon().ShowDamage("Escape Fail!", _core.getBattCon().getTargetOfMonster().getAvatarTrans().position);
                            _core.getBattCon().EndTurnSpeed();
                            _core.getMapCon()._escapeRate += 0.05f;
                        }

                    }
                    else
                    {
                        _core.NotifyCrystal("Not enough crystal!");
                    }
                }

            }
            else if (_core._gameMode == GAMESTATE.MAP)
            {
                _core.getMapCon().FocusPosition();
            }
            else
            {
                _playerInfoPanel.SetActive(false);
                _itemMenu.SetActive(false);
                _core.OpenScene(GAMESTATE.MAP);
            }
            
        }
        public void OpenAttackMenu()
        {
            _attackMenu.SetActive(!_attackMenu.activeSelf);
        }
        
        public void OpenSystem()
        {
            if (_core._subMenuPanel.activeSelf)
            {
                if (!_core._settingPanel.activeSelf)
                    _core._subMenuPanel.SetActive(false);
            }
            else
            {
                _core.getSubMenuCore().OpenGameMenu();
            }
        }

        public void OpenCook()
        {
            _cookMenu.SetActive(true);
            if(_core._gameMode == GAMESTATE.LAND)
                _core.getLandCon().FocusCamp();
        }
        
        public void OpenShop()
        {
            if (_shopMenu.activeSelf)
            {
                _shopMenu.SetActive(false);
            }
            else
            {
                if (_core._gameMode == GAMESTATE.LAND)
                    _core.getLandCon().FocusShop();
                _shopMenu.SetActive(true);
            }
        }
        public void OpenGate()
        {
            if (_core._gameMode == GAMESTATE.LAND)
                _core.getLandCon().FocusWarp();
            _core.getLandCon()._gatePanel.SetActive(true);
        }
        
        public void CloseBag()
        {
            gridViewTrans.Find("BagButton").GetComponent<Image>().sprite = _core._iconMenu.Single(s => s.name == "bag24x24");
            _itemMenu.SetActive(false);
            _core.getCampCon().setAllowTouch(true);
        }

        public void CloseGate()
        {
            _core.getLandCon()._gatePanel.SetActive(false);
            _core.getCampCon().setAllowTouch(true);
        }

        public bool CheckCrystal(int point)
        {
            if (_core._gameMode != GAMESTATE.BATTLE) return true;
            if (_core.getBattCon().Crystal - point < 0)
            {
                return false;
            }
            return true;
        }

        public bool UseCrystal(int point)
        {
            if (_core._gameMode != GAMESTATE.BATTLE) return true;
            if (_core.getBattCon().Crystal - point < 0)
            {
                return false;
            }
            _core.getBattCon().Crystal = _core.getBattCon().Crystal - point;
            if (_core.getBattCon().Crystal == 0)
            {
                _core.getBattCon().EndTurnSpeed();
            }
            return true;
        }

        Vector2 padding = new Vector2(20, 20);

        public void ViewItemDetail(Sprite icon,string txt)
        {
            _itemDetail.GetComponentInChildren<Text>().text = txt;
            _itemDetail.transform.Find("Icon").GetComponent<Image>().sprite = icon;
            _itemDetail.SetActive(true);
        }

    }
}

