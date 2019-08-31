using controller;
using system;
using UnityEngine;

namespace battle
{
    public class EventActive
    {
        BattleController getBatt()
        {
            return Camera.main.GetComponent<GameCore>().getBattCon();
        }

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
            getBatt().FocusHero().getStatus()._eventBonusDmg = getBatt()._evenAttack;
            getBatt().FocusMonster().getStatus()._eventBonusDmg = getBatt()._evenAttack;
        }
        public void Event5()
        {
            //Debug.Log("event 5");
            getBatt()._currentEvent = _Event.Wind;
		}

        public void Event6()
        {
            //Debug.Log("event 6");
            getBatt()._currentEvent = _Event.Rain;
        }

    }
}
