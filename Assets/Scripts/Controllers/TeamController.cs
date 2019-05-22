using Core;
using CollectionData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace Controller
{
    public class TeamController : MonoBehaviour
    {
        MainCore _core;
        PlayerInfoPanel _plyInfoPan;

        public GameObject _heroSlot;
        public GameObject _shadowSlot;
        public GameObject _teamIconSlot;
        public GameObject[] _teamIconSetup = new GameObject[5];

        public HeroStore _heroSwapIsSelect = null;
        public Dropdown _sortingDropdown;
        List<HeroStore> _teamList;
        List<HeroStore> _heroList;

        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
        }
        
        void OnEnable()
        {
           // _core._storyPanelTxt.text = "เจ้าสามารถจัดทีมไว้ได้หลากหลาย เพื่อเลือกใช้ในสถานการณ์ที่เหมาะสม!";
            LoadData();
            transform.Find("Icon").GetComponentInChildren<Text>().text = "ทีม " + _core._currentTeamIsSelect.ToString();
            RefeshViewHeroInStore(true);
            LoadTeamIcon(false);
            if(_core._gameMode == _GameStatus.LAND || _core._gameMode == _GameStatus.CAMP)
            {
                _plyInfoPan = _core._playerInfoPanel.GetComponent<PlayerInfoPanel>();
                _plyInfoPan.SetObjPanel(transform.gameObject);
            }
        }
        private void Start()
        {
            _sortingDropdown.onValueChanged.AddListener(delegate {
                SortingHeroInStore(_sortingDropdown.value);
            });
        }

        void LoadData()
        {
            _teamList = _core._teamSetup[_core._currentTeamIsSelect - 1].position;
            _heroList = _core._heroStore;
        }
        Sorting _sortingIsSelect;

        void SortingHeroInStore(int value,bool sorting=false)
        {
            //Debug.Log("dropdown " + value);
            _sortingIsSelect = (Sorting)value;
            Transform trans = _core._teamPanel.transform.Find("TeamStoreMask").Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
            trans.DetachChildren();
            if ((Sorting)value == Sorting.Type)
            {
                _heroList = _heroList.OrderByDescending(o => o.hero.type).ToList();
                //foreach (HeroStore hero in _heroList)
                //{
                // Debug.Log(hero.hero.type);
                //}
            }
            else if ((Sorting)value == Sorting.Level)
            {
                _heroList = _heroList.OrderByDescending(o => o.level).ToList();
                //foreach(HeroStore hero in _heroList)
                //{
                   // Debug.Log(hero.level);
                //}
            }
            else if ((Sorting)value == Sorting.HP)
            {
                _heroList = _heroList.OrderByDescending(o => o.hp).ToList();
                //foreach (HeroStore hero in _heroList)
                //{
                    //Debug.Log(hero.hp);
                //}
            }
            ViewHeroInStore(sorting);
        }

        public void LoadTeamIcon(bool loadAvatar=true)
        {
            Sprite[] loadSprite = null;
            string getSpriteSet = "";
            Transform trans = _core._teamPanel.transform.Find("TeamSetup").Find("GridView");
            for (int i = 0; i < 5; i++)
            {
                if(trans.childCount < 5)
                {
                    _teamIconSetup[i] = Instantiate(_teamIconSlot);
                    _teamIconSetup[i].transform.SetParent(trans);
                    _teamIconSetup[i].transform.localScale = new Vector3(1, 1, 1);
                }
                TeamIconSlot hero = _teamIconSetup[i].GetComponent<TeamIconSlot>();
                hero._hero = _teamList[i];
                hero._teamSlot = i;

                if (_core._currentTeamIsSelect ==1 && i==0)
                    _teamIconSetup[i].GetComponent<Image>().enabled = true;
                else
                    _teamIconSetup[i].GetComponent<Image>().enabled = false;

                _teamIconSetup[i].transform.Find("Level").gameObject.SetActive(false);
                if (_teamList[i].id == -1)
                {
                    if (getSpriteSet != "hero")
                    {
                        getSpriteSet = "hero";
                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                    }

                    _teamIconSetup[i].transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_Shadow");
                    _teamIconSetup[i].transform.Find("Type").gameObject.SetActive(false);
                    _teamIconSetup[i].transform.Find("Death").gameObject.SetActive(false);
                }
                else
                {
                    if (getSpriteSet != _teamList[i].hero.spriteSet)
                    {
                        getSpriteSet = _teamList[i].hero.spriteSet;
                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                    }
                    _teamIconSetup[i].transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _teamList[i].hero.spriteName);
                    _teamIconSetup[i].transform.Find("Type").gameObject.SetActive(true);
                    _core.SetSpriteType(_teamIconSetup[i].transform.Find("Type").GetComponent<Image>(), _teamList[i].hero.type);
                    _teamIconSetup[i].transform.Find("Death").gameObject.SetActive(false);

                }
            }
            if (loadAvatar)
            {
                _core.LoadCampAvatar(_teamIconSetup);
            }
                
        }

        void RefeshViewHeroInStore(bool sorting=false)
        {
            Transform trans = _core._teamPanel.transform.Find("TeamStoreMask").Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
            trans.DetachChildren();
            SortingHeroInStore(_sortingDropdown.value, sorting);
        }

        void ViewHeroInStore(bool sorting = false)
        {
            Transform trans = _core._teamPanel.transform.Find("TeamStoreMask").Find("GridView");
            if (trans.childCount != 0) return;
            
            if (sorting)
            {
                int count = 0;
                int pos = 0;
                foreach (HeroStore hero in _teamList)
                {
                    if (hero.id == -1)
                    {
                        count++;
                        continue;
                    }
                    for (int i = 0; i < _heroList.Count; i++)
                    {
                        if (_teamList[count].id == _heroList[i].id)
                        {
                            HeroStore tmp = _heroList[i];
                            _heroList[i] = _heroList[pos];
                            _heroList[pos] = tmp;
                            break;
                        }
                    }
                    pos++;
                    count++;
                }
            }

            Sprite[] loadSprite = null;
            string getSpriteSet = "";

            GameObject ShadowSlot = Instantiate(_shadowSlot);
            ShadowSlot.transform.SetParent(trans);
            ShadowSlot.transform.localScale = new Vector3(1, 1, 1);
            ShadowSlot shadow = ShadowSlot.GetComponent<ShadowSlot>();
            HeroStore newhero = new HeroStore();
            newhero.id = -1;
            shadow._hero = newhero;

            for (int i = 0; i < _heroList.Count; i++)
            {
                GameObject slot = Instantiate(_heroSlot);
                slot.transform.SetParent(trans);
                slot.transform.localScale = new Vector3(1, 1, 1);
                HeroSlot hero = slot.GetComponent<HeroSlot>();
                hero._hero = _heroList[i];

                if (getSpriteSet != _heroList[i].hero.spriteSet)
                {
                    getSpriteSet = _heroList[i].hero.spriteSet;
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                }

                slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _heroList[i].hero.spriteName);
                if (_sortingIsSelect == Sorting.HP)
                {
                    slot.transform.Find("Level").GetComponent<Text>().text = _heroList[i].hp.ToString();
                }
                else
                {
                    slot.transform.Find("Level").GetComponent<Text>().text = "เลเวล" + _heroList[i].level;
                }
                

                for (int j = 0; j < _teamList.Count; j++)
                {
                    if (_teamList[j].id == -1) continue;
                    if (_heroList[i].id == _teamList[j].id)
                    {
                        slot.transform.Find("Select").gameObject.SetActive(true);
                        break;
                    }
                }

                _core.SetSpriteType(slot.transform.Find("Type").GetComponent<Image>(), _heroList[i].hero.type);
                _heroList[i].obj = slot;
                if (_core._cutscene != null)
                {
                    if(_heroList[i].heroId == 9)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(slot.transform);
                }

            }
        }
        

        public void PreviousTeam()
        {
            _core._currentTeamIsSelect = --_core._currentTeamIsSelect < 1 ? _core._teamSetup.Count : _core._currentTeamIsSelect;
            LoadData();
            transform.Find("Icon").GetComponentInChildren<Text>().text = "ทีม "+_core._currentTeamIsSelect.ToString();
            LoadTeamIcon();
            RefeshViewHeroInStore(true);
        }

        public void NextTeam()
        {
            _core._currentTeamIsSelect = ++_core._currentTeamIsSelect > _core._teamSetup.Count ? 1 : _core._currentTeamIsSelect;
            LoadData();
            transform.Find("Icon").GetComponentInChildren<Text>().text = "ทีม " + _core._currentTeamIsSelect.ToString();
            LoadTeamIcon();
            RefeshViewHeroInStore(true);
        }

        public void ChangeHeroInTeam(int slot)
        {
            if (_heroSwapIsSelect == null|| _heroSwapIsSelect.id==0) return;
            //Debug.Log("swap 1 "+ _heroSwapIsSelect.id);
            if (_heroSwapIsSelect.id == -1)
            {
                //Debug.Log("swap 2");
                if (slot==0 &&_core._currentTeamIsSelect == 1)
                {
                    //Debug.Log("swap 3");
                    _core.CallSubMenu(_SubMenu.Alert, "ไม่สามารถปล่อยให้ตำแหน่งหัวหน้าทีมว่างได้!");
                    return;
                }
                //Debug.Log("swap 4");
                _teamList[slot] = _heroSwapIsSelect;
                _heroSwapIsSelect = null;
                LoadTeamIcon();
                RefeshViewHeroInStore();
            }
            else
            {
                //Debug.Log("swap 5");
                if (_teamList[slot].id == -1)
                {
                    //Debug.Log("swap 6");
                    bool have = false;
                    for(int i = 0; i < _teamList.Count; i++)
                    {
                        if(_heroSwapIsSelect.id == _teamList[i].id)
                        {
                            if (i == 0)
                            {
                                _core.CallSubMenu(_SubMenu.Alert, "ไม่สามารถปล่อยให้ตำแหน่งหัวหน้าทีมว่างได้!");
                                return;
                            }
                            HeroStore tmp = _teamList[i];
                            _teamList[i] = _teamList[slot];
                            _teamList[slot] = tmp;
                            _heroSwapIsSelect = null;
                            LoadTeamIcon();
                            RefeshViewHeroInStore();
                            have = true;
                            break;
                        }
                    }
                    if (!have)
                    {
                        _teamList[slot] = _heroSwapIsSelect;
                        _heroSwapIsSelect = null;
                        LoadTeamIcon();
                        RefeshViewHeroInStore();
                    }

                }
                else
                {
                    //Debug.Log("swap 7");
                    if (_teamList[slot].id == _heroSwapIsSelect.id)
                    {
                    }
                    else
                    {
                        for (int i = 0; i < _teamList.Count; i++)
                        {
                            if (_teamList[i].id == _heroSwapIsSelect.id)
                            {
                                HeroStore tmp = _teamList[i];
                                _teamList[i] = _teamList[slot];
                                _teamList[slot] = tmp;
                                break;
                            }
                        }
                        _teamList[slot] = _heroSwapIsSelect;
                        _heroSwapIsSelect = null;
                        LoadTeamIcon();
                        RefeshViewHeroInStore();
                    }
                }
                
            }
            
        }
        public HeroStore _heroWaitRevive;

        public void OnHeroRevive()
        {
            if(_heroWaitRevive != null)
            {
                _heroWaitRevive.hp = 1;
            }
            LoadTeamIcon(false);
        }

        void Destroy()
        {
            _sortingDropdown.onValueChanged.RemoveAllListeners();
        }
        
    }
}

