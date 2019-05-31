using Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Controller;
using CollectionData;
using System.Reflection;
using System;

namespace UI
{
    public class ManageHeroPanel : MonoBehaviour
    {

        MainCore _core;
        BattleController _battleCon;

        int _heroIsSelect;
        float _spacing;
        
        public GameObject _confirmBtn;
        public GameObject _ManageTeamSlot;
        public GameObject _ManageTeamBlockSlot;
        Transform _manageMask;

        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _battleCon = _core._battleObj.GetComponent<BattleController>();
            smoothTime = 0.05f;
            _manageMask = transform.Find("ManageMask").Find("GridView");
            _recManageTeamSlot = _ManageTeamSlot.GetComponent<RectTransform>();
            _spacing = _manageMask.GetComponent<HorizontalLayoutGroup>().spacing;
        }

        void OnEnable()
        {
            _teamList = new List<HeroStore>();
            LoadData();
            _heroIsSelect = 0;
            _confirmBtn.SetActive(_core._ActionMode != _ActionStatus.Item ? false : true);
            LoadHeroIcon();
            ShowInfoHero(_teamList[_heroIsSelect]);
            
                
        }
        List<HeroStore> _teamList;

        void LoadData()
        {
            if(_core._gameMode != _GameStatus.BATTLE)
            {
                for(int i=0;i< _core._heroStore.Count; i++)
                {
                    _teamList.Add(_core._heroStore[i]);
                }
            }
            else
            {
                 _teamList = _core._teamSetup[_core._currentTeamIsSelect - 1].position;
            }
        }
        
        private void LateUpdate()
        {
            if (_nextCheck)
                NextImg();
            if (_prevCheck)
                PrevImg();
        }

        void LoadHeroIcon()
        {
            Sprite[] loadSprite = null;
            string getSpriteSet = "";

            CreateBlockSlot(_manageMask);
            
            for (int i = 0; i < _teamList.Count; i++)
            {
                GameObject slot = Instantiate(_ManageTeamSlot);
                slot.transform.SetParent(_manageMask);
                slot.transform.localScale = new Vector3(1, 1, 1);

                if (_teamList[i].id == -1)
                {
                    if (getSpriteSet != "hero")
                    {
                        getSpriteSet = "hero";
                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                    }

                    slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_Shadow");
                    slot.transform.Find("Type").gameObject.SetActive(false);
                    slot.transform.Find("Death").gameObject.SetActive(false);
                }
                else
                {
                    if (getSpriteSet != _teamList[i].hero.spriteSet)
                    {
                        getSpriteSet = _teamList[i].hero.spriteSet;
                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                    }
                    slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _teamList[i].hero.spriteName);
                    slot.transform.Find("Type").gameObject.SetActive(true);
                    _core.SetSpriteType(slot.transform.Find("Type").GetComponent<Image>(), _teamList[i].hero.type);
                    if (_teamList[i].hp <= 0)
                        slot.transform.Find("Death").gameObject.SetActive(true);
                    else
                        slot.transform.Find("Death").gameObject.SetActive(false);
                }
                _teamList[i].obj = slot;

            }

            CreateBlockSlot(_manageMask);

            _manageMask.localPosition = Vector3.zero;
            if (_core._gameMode == _GameStatus.BATTLE)
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
            _teamList[_heroIsSelect].obj.transform.Find("SelectEffect").gameObject.SetActive(false);
            _heroIsSelect--;
            if (_core._ActionMode == _ActionStatus.Item || _teamList[_heroIsSelect].id != -1 && 0 != _heroIsSelect && _teamList[_heroIsSelect].hp >0)
            {
                _confirmBtn.SetActive(true);
            }
            else
            {
                _confirmBtn.SetActive(false);
            }
            _nextCheck = false;
            _newPosition = new Vector3(_manageMask.localPosition.x + _recManageTeamSlot.rect.width + _spacing, _manageMask.localPosition.y, _manageMask.localPosition.z);
            ShowInfoHero(_teamList[_heroIsSelect]);
            _prevCheck = true;
        }

        public void NextHero()
        {
            if (_heroIsSelect >= _teamList.Count - 1) return;
            _teamList[_heroIsSelect].obj.transform.Find("SelectEffect").gameObject.SetActive(false);
            _heroIsSelect++;
            if (_core._ActionMode == _ActionStatus.Item || _teamList[_heroIsSelect].id != -1 && 0 != _heroIsSelect && _teamList[_heroIsSelect].hp >0)
            {
                _confirmBtn.SetActive(true);
            }
            else
            {
                _confirmBtn.SetActive(false);
            }
            _prevCheck = false;
            _newPosition = new Vector3(_manageMask.localPosition.x - (_recManageTeamSlot.rect.width + _spacing), _manageMask.localPosition.y, _manageMask.localPosition.z);
            ShowInfoHero(_teamList[_heroIsSelect]);
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

        void ShowInfoHero(HeroStore _hero)
        {
            if(_hero.id == -1)
            {
                _core._infoPanel.SetActive(false);
                return;
            }
            _hero.obj.transform.Find("SelectEffect").gameObject.SetActive(true);
                _core.SetInfo(_hero.hero.name + " Lv. " + _hero.level
                    +(_hero.hp < _hero.hpMax / 2 ? "<color=#ff0000><เลือด " : "<color=#01b140><เลือด ") + _hero.hp + " </color><color=#01b140>/" + _hero.hpMax + "></color>"
                    + "\n<โจมตี " + _hero.ATK+">"
                    + "<โจมตีเวทย์ " + _hero.MATK + ">"
                    + "<เกาะ " + _hero.DEF  + ">"
                    + "<เกาะเวทย์ " + _hero.MDEF  + ">"
                    );
            
        }



        public void ConfirmBtn()
        {
            if (_core._ActionMode == _ActionStatus.Item)
            {
                if(_teamList[_heroIsSelect].id == -1)
                {
                    _core.OpenErrorNotify("ไม่มีฮีโร่ที่ใช้งานได้!");
                    return;
                }
                if( _core.CheckCrystal(Constants._crystalItem))
                {
                    foreach (ItemStore item in _core._itemStore.ToList())
                    {
                        if (_core._itemCon._itemStoreIdSelect.id == item.id)
                        {
                            if (CallItemFunction((_item)_core._itemCon._itemStoreIdSelect.itemId, _teamList[_heroIsSelect]))
                            {
                                if (_core._cutscene != null)
                                {
                                    _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._manageHeroPanel.transform.Find("CancelButton"));
                                }
                                item.amount -= 1;
                                if(_teamList[_heroIsSelect].hp > 0)
                                {
                                    _teamList[_heroIsSelect].obj.transform.Find("Death").gameObject.SetActive(false);
                                }
                                ShowInfoHero(_teamList[_heroIsSelect]);
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
            if (_core._ActionMode == _ActionStatus.Item)
            {
                _core._itemCon._itemStoreIdSelect.obj.transform.Find("Select").gameObject.SetActive(false);
                _core._itemCon._itemStoreIdSelect = null;
                if(_core._gameMode == _GameStatus.BATTLE)
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

        public bool CallItemFunction(_item methodId,params object[] args)
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
        }
    }
}

