
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
    public class PlayerController : MonoBehaviour
    {
        GameCore _core;

        public GameObject _heroSlot;
        public GameObject _shadowSlot;
        public GameObject _teamIconSlot;
        
        Material[] _mats;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _mats = Resources.LoadAll("Sprites/Character/Hero/", typeof(Material)).Cast<Material>().ToArray();
        }
        
        void OnEnable()
        {

        }

        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void LoadPlayerIcon(bool loadAvatar=true,GameObject playerIcon=null)
        {
            if (!playerIcon) return;

            playerIcon.transform.Find("Level").gameObject.SetActive(false);

            if (getSpriteSet != _core._heroIsPlaying.GetData().spriteSet)
            {
                getSpriteSet = _core._heroIsPlaying.GetData().spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            Debug.Log("playerIcon"+_core._heroIsPlaying.GetData().spriteName);
            playerIcon.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + _core._heroIsPlaying.GetData().spriteName);
            
            playerIcon.transform.Find("Death").gameObject.SetActive(false);
            _core._CharacterPanel.transform.Find("InfoBG").Find("NameText").GetComponent<Text>().text = _core._heroIsPlaying.GetData().name;

            if (loadAvatar)
            {
                LoadCampAvatar();
            }
                
        }

        public void LoadCampAvatar()
        {
            if (getSpriteSet != _core._heroIsPlaying.GetData().spriteSet)
            {
                getSpriteSet = _core._heroIsPlaying.GetData().spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            Debug.Log("camp avatar " + _core._heroIsPlaying.GetData().spriteName);
            _core._campAvatar[0].GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == _core._heroIsPlaying.GetData().spriteName);
            _core._campAvatar[0].GetComponent<SpriteRenderer>().material = _mats.Single(s => s.name == getSpriteSet);
        }
        
    }
}

