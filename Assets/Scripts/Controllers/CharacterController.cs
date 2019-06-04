
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UI;
using model;

namespace controller
{
    public class CharacterController : MonoBehaviour
    {
        GameCore _core;

        public GameObject _heroSlot;
        public GameObject _shadowSlot;
        public GameObject _teamIconSlot;
        public GameObject _characterIcon;
        
        List<HeroStore> _teamList;
        List<HeroStore> _heroList;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }
        
        void OnEnable()
        {
            LoadData();

            LoadCharacterIcon(false);
        }

        void LoadData()
        {
            _teamList = _core._teamSetup[_core._currentTeamIsSelect - 1].position;
            _heroList = _core._heroStore;
        }
        

        public void LoadCharacterIcon(bool loadAvatar=true)
        {
            Sprite[] loadSprite = null;
            string getSpriteSet = "";

            if (!_characterIcon)
                _characterIcon = transform.Find("InfoBG").Find("Icon").gameObject;

            _characterIcon.transform.Find("Level").gameObject.SetActive(false);

            if (getSpriteSet != _teamList[0].hero.spriteSet)
            {
                getSpriteSet = _teamList[0].hero.spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            _characterIcon.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _teamList[0].hero.spriteName);
            _characterIcon.transform.Find("Type").gameObject.SetActive(true);
            _core.SetSpriteType(_characterIcon.transform.Find("Type").GetComponent<Image>(), _teamList[0].hero.type);
            _characterIcon.transform.Find("Death").gameObject.SetActive(false);
            _core._CharacterPanel.transform.Find("InfoBG").Find("NameText").GetComponent<Text>().text = _teamList[0].hero.name;
            if (loadAvatar)
            {
                _core.LoadCampAvatar(_characterIcon);
            }
                
        }

        
        
        public HeroStore _heroWaitRevive;

        public void OnHeroRevive()
        {
            if(_heroWaitRevive != null)
            {
                _heroWaitRevive.hp = 1;
            }
            LoadCharacterIcon(false);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
        
        
    }
}

