
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
    public class MonPanel : MonoBehaviour
    {
        MainCore _core;

        public GameObject[] _monAvatarList;

        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
        }
        public void OnEnable()
        {
        }

        void Start()
        {

        }
        
        public GameObject _monIconSlot;
        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public GameObject LoadMonIcon(Monster mon)
        {
            GameObject slot = Instantiate(_monIconSlot);
            slot.transform.SetParent(transform.Find("GridView"));
            slot.transform.localScale = new Vector3(1, 1, 1);

            if (getSpriteSet != mon.spriteSet)
            {
                getSpriteSet = mon.spriteSet;
                if (mon.spriteSet.Contains("monster"))
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Monster/" + getSpriteSet);
                }
                else
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                }
            }
            slot.transform.Find("IconImage").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + mon.spriteName);
            slot.transform.Find("Level").GetComponent<Text>().text = "Lv. " + mon.level;
            if (_core == null)
                _core = Camera.main.GetComponent<MainCore>();
            _core.SetSpriteType(slot.transform.Find("TypeImage").GetComponent<Image>(), mon.type);
            return slot;
        }
        
    }
}

