using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CollectionData
{
    public class EventActive
    {
        BattleController _battleCon;
        MainCore _core;

        void Run()
        {
            if (_core == null)
                _core = Camera.main.GetComponent<MainCore>();
            if (_battleCon == null)
                _battleCon = _core._battleCon;
            _battleCon.GetHeroFocus()._eventBonusDmg = 1;
            _battleCon.GetMonFocus()._eventBonusDmg = 1;
        }

        public void Event1()
        {
            Run();
            //Debug.Log("event 1");
            if(_battleCon.GetHeroFocus().hero.hero.type == _Character.HAMMER)
            {
                _battleCon.GetHeroFocus()._eventBonusDmg = _battleCon._evenAttack;
            }
            if(_battleCon.GetMonFocus().type == _Character.HAMMER)
            {
                _battleCon.GetMonFocus()._eventBonusDmg = _battleCon._evenAttack;
            }
		}
        public void Event2()
        {
            Run();
            //Debug.Log("event 2");
            if (_battleCon.GetHeroFocus().hero.hero.type == _Character.SCISSORS)
            {
                _battleCon.GetHeroFocus()._eventBonusDmg = _battleCon._evenAttack;
            }
            if (_battleCon.GetMonFocus().type == _Character.SCISSORS)
            {
                _battleCon.GetMonFocus()._eventBonusDmg = _battleCon._evenAttack;
            }
        }
        public void Event3()
        {
            Run();
            //Debug.Log("event 3");
            if (_battleCon.GetHeroFocus().hero.hero.type == _Character.PAPER)
            {
                _battleCon.GetHeroFocus()._eventBonusDmg = _battleCon._evenAttack;
            }
            if (_battleCon.GetMonFocus().type == _Character.PAPER)
            {
                _battleCon.GetMonFocus()._eventBonusDmg = _battleCon._evenAttack;
            }
        }
        public void Event4()
        {
            Run();
            //Debug.Log("event 4");
            _battleCon.GetHeroFocus()._eventBonusDmg = _battleCon._evenAttack;
            _battleCon.GetMonFocus()._eventBonusDmg = _battleCon._evenAttack;
        }
        public void Event5()
        {
            Run();
            //Debug.Log("event 5");
            _battleCon._currentEvent = _event.Wind;
		}

        public void Event6()
        {
            Run();
            //Debug.Log("event 6");
            _battleCon._currentEvent = _event.Rain;
        }

    }
}
