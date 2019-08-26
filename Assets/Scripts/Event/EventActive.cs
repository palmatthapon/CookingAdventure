
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

        public void Event1()
        {
            //Debug.Log("event 1");
            
		}
        public void Event2()
        {
            //Debug.Log("event 2");
            
        }
        public void Event3()
        {
            //Debug.Log("event 3");
            
        }
        public void Event4()
        {
            //Debug.Log("event 4");
            GameCore.call()._battleCon.FocusHero().getStatus()._eventBonusDmg = GameCore.call()._battleCon._evenAttack;
            GameCore.call()._battleCon.FocusMonster().getStatus()._eventBonusDmg = GameCore.call()._battleCon._evenAttack;
        }
        public void Event5()
        {
            //Debug.Log("event 5");
            GameCore.call()._battleCon._currentEvent = _Event.Wind;
		}

        public void Event6()
        {
            //Debug.Log("event 6");
            GameCore.call()._battleCon._currentEvent = _Event.Rain;
        }

    }
}
