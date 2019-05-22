using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroPanel : MonoBehaviour
    {
        MainCore _core;

        public GameObject[] _heroAvatarList;

        private void Awake()
        {
            //_heroIconSlot = transform.Find("GridView").Find("HeroIconSlot").gameObject;
        }
        public void OnEnable()
        {

        }

        void Start()
        {
            _core = Camera.main.GetComponent<MainCore>();
        }

        public GameObject _heroIconSlot;
        Sprite[] loadSprite = null;
        string getSpriteSet = "";
        
        public GameObject LoadHeroIcon(HeroStore hero)
        {
            GameObject slot = Instantiate(_heroIconSlot);
            slot.transform.SetParent(transform.Find("GridView"));
            slot.transform.localScale = new Vector3(1, 1, 1);
            if (getSpriteSet != hero.hero.spriteSet)
            {
                getSpriteSet = hero.hero.spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            slot.transform.Find("IconImage").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + hero.hero.spriteName);
            slot.transform.Find("Level").GetComponent<Text>().text = "เลเวล" + hero.level;
            if(_core==null)
                _core = Camera.main.GetComponent<MainCore>();
            _core.SetSpriteType(slot.transform.Find("TypeImage").GetComponent<Image>(), hero.hero.type);
            return slot;
        }

    }
}