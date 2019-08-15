
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace model
{
    public class MonPanel : MonoBehaviour
    {
        GameCore _core;

        public GameObject[] _monAvatarList;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }
        public void OnEnable()
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

            if (getSpriteSet != mon.GetData().spriteSet)
            {
                getSpriteSet = mon.GetData().spriteSet;
                if (mon.GetData().spriteSet.Contains("monster"))
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Monster/" + getSpriteSet);
                }
                else
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                }
            }
            slot.transform.Find("IconImage").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + mon.GetData().spriteName);
            slot.transform.Find("Level").GetComponent<Text>().text = "Lv. " + mon.GetStatus().level;
            if (_core == null)
                _core = Camera.main.GetComponent<GameCore>();
            return slot;
        }
        
    }
}

