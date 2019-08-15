
using controller;
using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
    public class EventActive
    {
        BattleController _battleCon;
        GameCore _core;

        void Run()
        {
            if (_core == null)
                _core = Camera.main.GetComponent<GameCore>();
            if (_battleCon == null)
                _battleCon = _core._battleCon;
            _battleCon.FocusHero().GetStatus()._eventBonusDmg = 1;
            _battleCon.FocusMonster().GetStatus()._eventBonusDmg = 1;
        }

        public void Event1()
        {
            Run();
            //Debug.Log("event 1");
            if(_battleCon.FocusHero().GetData().type == _Character.HAMMER)
            {
                _battleCon.FocusHero().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            }
            if(_battleCon.FocusMonster().GetData().type == _Character.HAMMER)
            {
                _battleCon.FocusMonster().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            }
		}
        public void Event2()
        {
            Run();
            //Debug.Log("event 2");
            if (_battleCon.FocusHero().GetData().type == _Character.SCISSORS)
            {
                _battleCon.FocusHero().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            }
            if (_battleCon.FocusMonster().GetData().type == _Character.SCISSORS)
            {
                _battleCon.FocusMonster().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            }
        }
        public void Event3()
        {
            Run();
            //Debug.Log("event 3");
            if (_battleCon.FocusHero().GetData().type == _Character.PAPER)
            {
                _battleCon.FocusHero().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            }
            if (_battleCon.FocusMonster().GetData().type == _Character.PAPER)
            {
                _battleCon.FocusMonster().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            }
        }
        public void Event4()
        {
            Run();
            //Debug.Log("event 4");
            _battleCon.FocusHero().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
            _battleCon.FocusMonster().GetStatus()._eventBonusDmg = _battleCon._evenAttack;
        }
        public void Event5()
        {
            Run();
            //Debug.Log("event 5");
            _battleCon._currentEvent = _Event.Wind;
		}

        public void Event6()
        {
            Run();
            //Debug.Log("event 6");
            _battleCon._currentEvent = _Event.Rain;
        }

    }
}
