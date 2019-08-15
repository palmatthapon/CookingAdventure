
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using model;

namespace model
{
    public class HeroSlot : EventTrigger
    {
        public Hero _hero;
        CharacterController _teamCon;
        GameCore _core;
        

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _teamCon = _core._CharacterPanel.GetComponent<CharacterController>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            //_teamCon._heroSwapIsSelect = _hero;
            string[] passive = _core._passiveDatail[(int)_hero.GetStatus().passive - 1].Split(':');
            

            _core.SetTalk(_hero.GetStatus().name + " Lv. " + _hero.GetStatus().level + " <color=#01b140><เลือด "+ _hero.GetStatus().hpMax + "></color>\n<color=#ff0000><โจมตี " + _hero.GetStatus().ATK + "></color><color=#1876d2><โจมตีเวทย์ " + _hero.GetStatus().MATK + "></color><color=#ff0000><เกาะ " + _hero.GetStatus().DEF + "></color><color=#1876d2><เกาะเวทย์ " + _hero.GetStatus().MDEF + "></color>"
            + "\n[โจมตีทั่วไป] " + _hero.GetStatus().attack[0].skill.name + "(" + (_hero.GetStatus().attack[0].skill.type == _Attack.PHYSICAL ? "กายภาพ" : "เวทย์") + ")" + "(" + _hero.GetStatus().attack[0].skill.bonusDmg * 100 + "%)"
                + " [ท่าไม้ตาย] " + _hero.GetStatus().attack[1].skill.name + "(" + (_hero.GetStatus().attack[1].skill.type == _Attack.PHYSICAL ? "กายภาพ" : "เวทย์") + ")" + "(" + _hero.GetStatus().attack[1].skill.bonusDmg * 100 + "%)"
                + "\n[ความสามารถติดตัว] " + passive[1]);
            
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
