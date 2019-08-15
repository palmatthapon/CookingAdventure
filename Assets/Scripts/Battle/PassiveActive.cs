
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using model;
using controller;

namespace skill
{
    public class PassiveActive
    {
        BattleController _battleCon;
        GameCore _core;

        void Run()
        {
            if (_core == null)
                _core = Camera.main.GetComponent<GameCore>();
            if (_battleCon == null)
                _battleCon = _core._battleObj.GetComponent<BattleController>();
            _battleCon.FocusHero().GetStatus().SetDefaultPassive();
        }

        public void Passive1(_Model type)
        {
            Run();
        }

        public void Passive2(_Model type)
        {
            Run();
            if (_battleCon.GetTargetFocus().GetStatus().currentHP < _battleCon.GetTargetFocus().GetStatus().hpMax / 2)
            {
                _battleCon.GetTargetFocus().GetStatus()._passiveBonusATK = 1.25f;
            }
            Debug.Log("Passive2...");
        }
        

    }

}
