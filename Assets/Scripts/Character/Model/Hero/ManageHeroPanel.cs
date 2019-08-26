
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;
using model;
using item;
using controller;

namespace model
{
    public class ManageHeroPanel : MonoBehaviour
    {
        /*
        GameCore _core;
        BattleController _battleCon;
        ItemController _itemCon;

        int _heroIsSelect;
        float _spacing;
        
        public GameObject _confirmBtn;
        public GameObject _ManageTeamSlot;
        public GameObject _ManageTeamBlockSlot;
        Transform _manageMask;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _battleCon = _core._battleObj.GetComponent<BattleController>();
            _itemCon = _core._mainMenu.GetComponent<ItemController>();
            smoothTime = 0.05f;
            _manageMask = transform.Find("ManageMask").Find("GridView");
            _recManageTeamSlot = _ManageTeamSlot.GetComponent<RectTransform>();
            _spacing = _manageMask.GetComponent<HorizontalLayoutGroup>().spacing;
        }

        void OnEnable()
        {
            _heroList = new List<Hero>();
            LoadData();
            _heroIsSelect = 0;
            _confirmBtn.SetActive(_core._ActionMode != _ActionState.Item ? false : true);
            LoadHeroIcon();
            ShowInfoHero(_heroList[_heroIsSelect]);
            
                
        }
        List<Hero> _heroList;

        void LoadData()
        {
            if(_core._gameMode != _GameState.BATTLE)
            {
                for(int i=0;i< _core._heroStore.Count; i++)
                {
                    _heroList.Add(_core._heroStore[i]);
                }
            }
            else
            {
                _heroList.Add(_core._heroIsPlaying);
            }
        }
        
        private void LateUpdate()
        {
            if (_nextCheck)
                NextImg();
            if (_prevCheck)
                PrevImg();
        }

        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        void LoadHeroIcon()
        {
            

            CreateBlockSlot(_manageMask);
            
            for (int i = 0; i < _heroList.Count; i++)
            {
                GameObject slot = Instantiate(_ManageTeamSlot);
                slot.transform.SetParent(_manageMask);
                slot.transform.localScale = new Vector3(1, 1, 1);

                if (getSpriteSet != _heroList[i].getSpriteSet())
                {
                    getSpriteSet = _heroList[i].getSpriteSet();
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                }
                slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _heroList[i].getSpriteName());

                if (_heroList[i].getStatus().currentHP <= 0)
                    slot.transform.Find("Death").gameObject.SetActive(true);
                else
                    slot.transform.Find("Death").gameObject.SetActive(false);

                _heroList[i].setIconManageHero(slot);

            }

            CreateBlockSlot(_manageMask);

            _manageMask.localPosition = Vector3.zero;
            if (_core._gameMode == _GameState.BATTLE)
            {
                _manageMask.localPosition = new Vector3(_manageMask.localPosition.x - (_recManageTeamSlot.rect.width + _spacing) * (1 + _heroIsSelect) + _spacing, _manageMask.localPosition.y, _manageMask.localPosition.z);
            }
            else
            {
                _manageMask.localPosition = new Vector3(_manageMask.localPosition.x - _recManageTeamSlot.rect.width, _manageMask.localPosition.y, _manageMask.localPosition.z);

            }
        }

        void CreateBlockSlot(Transform trans)
        {
            for(int i = 0; i < 3; i++)
            {
                GameObject slot = Instantiate(_ManageTeamBlockSlot);
                slot.transform.SetParent(trans);
                slot.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        Vector3 _newPosition;
        RectTransform _recManageTeamSlot;
        bool _nextCheck = false;
        bool _prevCheck = false;

        public void PrevHero()
        {
            if (_heroIsSelect <= 0) return;
            _heroList[_heroIsSelect].getIconManageHero().transform.Find("SelectEffect").gameObject.SetActive(false);
            _heroIsSelect--;
            if (_core._ActionMode == _ActionState.Item || _heroList[_heroIsSelect].getStoreId() != -1 && 0 != _heroIsSelect && _heroList[_heroIsSelect].getStatus().currentHP > 0)
            {
                _confirmBtn.SetActive(true);
            }
            else
            {
                _confirmBtn.SetActive(false);
            }
            _nextCheck = false;
            _newPosition = new Vector3(_manageMask.localPosition.x + _recManageTeamSlot.rect.width + _spacing, _manageMask.localPosition.y, _manageMask.localPosition.z);
            ShowInfoHero(_heroList[_heroIsSelect]);
            _prevCheck = true;
        }

        public void NextHero()
        {
            if (_heroIsSelect >= _heroList.Count - 1) return;
            _heroList[_heroIsSelect].getIconManageHero().transform.Find("SelectEffect").gameObject.SetActive(false);
            _heroIsSelect++;
            if (_core._ActionMode == _ActionState.Item || _heroList[_heroIsSelect].getStoreId() != -1 && 0 != _heroIsSelect && _heroList[_heroIsSelect].getStatus().currentHP > 0)
            {
                _confirmBtn.SetActive(true);
            }
            else
            {
                _confirmBtn.SetActive(false);
            }
            _prevCheck = false;
            _newPosition = new Vector3(_manageMask.localPosition.x - (_recManageTeamSlot.rect.width + _spacing), _manageMask.localPosition.y, _manageMask.localPosition.z);
            ShowInfoHero(_heroList[_heroIsSelect]);
            _nextCheck = true;
        }
        public float smoothTime = 0.5f;
        private Vector3 tranVelocity = Vector3.zero;

        void PrevImg()
        {
            if ((int)_manageMask.localPosition.x > (int)_newPosition.x)
            {
                _prevCheck = false;
                return;
            }
            else
            {
                _manageMask.localPosition = Vector3.SmoothDamp(_manageMask.localPosition, _newPosition, ref tranVelocity, smoothTime);
            }
        }

        void NextImg()
        {
            if ((int)_manageMask.localPosition.x < (int)_newPosition.x)
            {
                _nextCheck = false;
                return;
            }
            else
            {
                _manageMask.localPosition = Vector3.SmoothDamp(_manageMask.localPosition, _newPosition, ref tranVelocity, smoothTime);
            }
        }

        void ShowInfoHero(Hero _hero)
        {
            if(_hero.getStoreId() == -1)
            {
                _core._infoPanel.SetActive(false);
                return;
            }
            _hero.getIconManageHero().transform.Find("SelectEffect").gameObject.SetActive(true);
                _core.SetInfo(_hero.getStatus().name + " Lv. " + _hero.getStatus().level
                    +(_hero.getStatus().currentHP < _hero.getStatus().hpMax / 2 ? "<color=#ff0000><เลือด " : "<color=#01b140><เลือด ") + _hero.getStatus().currentHP + " </color><color=#01b140>/" + _hero.getStatus().hpMax + "></color>"
                    + "\n<โจมตี " + _hero.getStatus().ATK+">"
                    + "<โจมตีเวทย์ " + _hero.getStatus().MATK + ">"
                    + "<เกาะ " + _hero.getStatus().DEF  + ">"
                    + "<เกาะเวทย์ " + _hero.getStatus().MDEF  + ">"
                    );
            
        }
        
        public void ConfirmBtn()
        {
            if (_core._ActionMode == _ActionState.Item)
            {
                if(_heroList[_heroIsSelect].getStoreId() == -1)
                {
                    _core.OpenErrorNotify("ไม่มีฮีโร่ที่ใช้งานได้!");
                    return;
                }
                if( _core.CheckCrystal(Constants._crystalItem))
                {
                    foreach (ItemStore item in _core._itemStore.ToList())
                    {
                        if (_itemCon._itemStoreIdSelect.id == item.id)
                        {
                            if (CallItemFunction((_Item)_itemCon._itemStoreIdSelect.itemId, _heroList[_heroIsSelect]))
                            {
                                
                                item.amount -= 1;
                                if(_heroList[_heroIsSelect].getStatus().currentHP > 0)
                                {
                                    _heroList[_heroIsSelect].getIconManageHero().transform.Find("Death").gameObject.SetActive(false);
                                }
                                ShowInfoHero(_heroList[_heroIsSelect]);
                                if (item.amount == 0)
                                {
                                    item.obj.transform.Find("Select").gameObject.SetActive(false);
                                    Destroy(item.obj);
                                    _core._itemStore.Remove(item);
                                    CancelBtn();
                                    return;
                                }
                                item.obj.transform.Find("Count").GetComponent<Text>().text = item.amount.ToString();
                                _battleCon._battleState = _BattleState.Finish;
                                _core.UseCrystal(Constants._crystalItem);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    //_core.CallSubMenu(_SubMenu.Alert, "คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย " + Constants._crystalItem);
                    _core.OpenErrorNotify("คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย " + Constants._crystalItem);
                }
            }
            else
            {
                if (_core.UseCrystal(Constants._crystalTeam))
                {
                    _core._infoPanel.SetActive(false);
                }
                else
                {
                    //_core.CallSubMenu(_SubMenu.Alert, "คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย " + Constants._crystalTeam);
                    _core.OpenErrorNotify("คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย " + Constants._crystalTeam);
                }
                
            }
            
        }

        public void CancelBtn()
        {
            if (_core._ActionMode == _ActionState.Item)
            {
                _itemCon._itemStoreIdSelect.obj.transform.Find("Select").gameObject.SetActive(false);
                _itemCon._itemStoreIdSelect = null;
                if(_core._gameMode == _GameState.BATTLE)
                {
                    _core._itemPanel.SetActive(false);
                    _core._attackPanel.SetActive(true);
                }  
            }
            else
            {
            }
            _core._manageHeroPanel.SetActive(false);
            _core._infoPanel.SetActive(false);
        }

        public bool CallItemFunction(_Item methodId,params object[] args)
        {
            string methodName = methodId.ToString();

            Type type = typeof(ItemActive);
            MethodInfo method = type.GetMethod(methodName);
            ItemActive c = new ItemActive();
            return (bool)method.Invoke(c, args);
        }

        private void OnDisable()
        {
            _nextCheck = false;
            _prevCheck = false;
            _newPosition = Vector3.zero;
            foreach (Transform child in _manageMask)
            {
                Destroy(child.gameObject);
            }
        }*/
    }
}

