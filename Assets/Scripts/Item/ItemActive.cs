
using controller;
using model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace item
{
    public class ItemActive
    {
       
        
        //how to name for HeroPassiveAbility Method, Use all caps e.g. HELLOWORLD 
        public bool SmallPosion(Hero hero)
        {
            if (hero.getStatus().currentHP > 0)
            {
                hero.getStatus().currentHP = hero.getStatus().currentHP + 20;
                GameCore.call().CreateEffect(GameCore.call()._healEffect, hero.getAvatarTrans());
                return true;
            }
            else
            {
                GameCore.call().OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool MediumPosion(Hero hero)
        {
            if (hero.getStatus().currentHP > 0)
            {
                hero.getStatus().currentHP = hero.getStatus().currentHP + 50;
                GameCore.call().CreateEffect(GameCore.call()._healEffect, hero.getAvatarTrans());
                return true;
            }
            else
            {
                GameCore.call().OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool LargePosion(Hero hero)
        {
            if (hero.getStatus().currentHP > 0)
            {
                hero.getStatus().currentHP = hero.getStatus().currentHP + 100;
                GameCore.call().CreateEffect(GameCore.call()._healEffect, hero.getAvatarTrans());
                return true;
            }
            else
            {
                GameCore.call().OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool ReCrystal(Hero hero)
        {
            //Debug.Log("Re actionPoint ");
            return true;
        }

        public bool Antidote(Hero hero)
        {
            //Debug.Log("Re actionPoint ");
            return true;
        }

    }
}
