﻿using system;
using UnityEngine;
using UnityEngine.EventSystems;

namespace model
{
    public class HeroSlot : EventTrigger
    {
        public Hero _hero;
        GameCore _core;
        

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            //_teamCon._heroSwapIsSelect = _hero;
            string[] passive = _core._passiveDatail[(int)_hero.getStatus().passive - 1].Split(':');
            

            _core.SetTalk(_hero.getName() + " Lv. " + _hero.getStatus().getLvl() + " <color=#01b140><hp "+ _hero.getStatus().currentHPMax + "></color>\n<color=#ff0000><attack " + _hero.getStatus().getATK() + "></color><color=#1876d2><magic attack " + _hero.getStatus().getMATK() + "></color><color=#ff0000><defense " + _hero.getStatus().getDEF() + "></color><color=#1876d2><magic defense " + _hero.getStatus().getMDEF() + "></color>"
            + "\n[โจมตีทั่วไป] " + _hero.getStatus().attack[0].name + "(" + (_hero.getStatus().attack[0].type == _Attack.PHYSICAL ? "กายภาพ" : "เวทย์") + ")" + "(" + _hero.getStatus().attack[0].bonusDmg * 100 + "%)"
                + " [ท่าไม้ตาย] " + _hero.getStatus().attack[1].name + "(" + (_hero.getStatus().attack[1].type == _Attack.PHYSICAL ? "กายภาพ" : "เวทย์") + ")" + "(" + _hero.getStatus().attack[1].bonusDmg * 100 + "%)"
                + "\n[ความสามารถติดตัว] " + passive[0]);
            
        }


        public override void OnDeselect(BaseEventData data)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
#endif
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
            if (!(hit.transform != null && hit.transform.tag == "IconTeam"))
            {
                //_teamCon._heroSwapIsSelect = null;
            }

            if (_core._talkPanel.activeSelf)
            {
                _core._talkPanel.SetActive(false);
            }
        }


    }

}