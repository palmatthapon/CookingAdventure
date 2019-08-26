
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using model;
using controller;

namespace skill
{
    public class PassiveActive
    {
        public void Passive1(_Model type)
        {
        }

        public void Passive2(_Model type)
        {
            if (GameCore.call()._battleCon.GetTargetFocus().getStatus().currentHP < GameCore.call()._battleCon.GetTargetFocus().getStatus().currentHPMax / 2)
            {
                GameCore.call()._battleCon.GetTargetFocus().getStatus()._passiveBonusATK = 1.25f;
            }
            Debug.Log("Passive2...");
        }
        

    }

}
