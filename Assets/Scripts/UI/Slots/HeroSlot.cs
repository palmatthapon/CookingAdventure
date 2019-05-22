using Core;
using CollectionData;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class HeroSlot : EventTrigger
    {
        public HeroStore _hero;
        TeamController _teamCon;
        MainCore _core;
        

        void Start()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _teamCon = _core._teamPanel.GetComponent<TeamController>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            _teamCon._heroSwapIsSelect = _hero;
            string[] passive = _core._passiveDatail[(int)_hero.passive - 1].Split(':');
            
            
                if (_core._cutscene != null)
                {
                    if (_hero.id == 2)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_teamCon._teamIconSetup[1].transform);
                }

            _core.SetTalk(_hero.hero.name + " เลเวล " + _hero.level + " <color=#01b140><เลือด "+ _hero.hpMax + "></color>\n<color=#ff0000><โจมตี " + _hero.ATK + "></color><color=#1876d2><โจมตีเวทย์ " + _hero.MATK + "></color><color=#ff0000><เกาะ " + _hero.DEF + "></color><color=#1876d2><เกาะเวทย์ " + _hero.MDEF + "></color>"
            + "\n[โจมตีทั่วไป] " + _hero.attack[0].skill.name + "(" + (_hero.attack[0].skill.type == _Attack.PHYSICAL ? "กายภาพ" : "เวทย์") + ")" + "(" + _hero.attack[0].skill.bonusDmg * 100 + "%)"
                + " [ท่าไม้ตาย] " + _hero.attack[1].skill.name + "(" + (_hero.attack[1].skill.type == _Attack.PHYSICAL ? "กายภาพ" : "เวทย์") + ")" + "(" + _hero.attack[1].skill.bonusDmg * 100 + "%)"
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
                _teamCon._heroSwapIsSelect = null;
            }

            if (_core._talkPanel.activeSelf)
            {
                _core._talkPanel.SetActive(false);
            }
        }


    }

}
