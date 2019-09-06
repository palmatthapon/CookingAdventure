using UnityEngine;
using controller;
using system;

namespace battle
{
    public class PassiveActive
    {
        BattleController getBattCon()
        {
            return Camera.main.GetComponent<GameCore>().getBattCon();
        }
        public void Passive1(_Model type)
        {
        }

        public void Passive2(_Model type)
        {
            if (getBattCon().getTarget().getStatus().currentHP < getBattCon().getTarget().getStatus().currentHPMax / 2)
            {
                getBattCon().getTarget().getStatus()._passiveBonusATK = 1.25f;
            }
        }
        

    }

}
