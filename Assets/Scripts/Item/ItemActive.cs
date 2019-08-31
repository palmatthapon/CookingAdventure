﻿
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
        GameCore getCore()
        {
            return Camera.main.GetComponent<GameCore>();
        }
        
        //how to name for HeroPassiveAbility Method, Use all caps e.g. HELLOWORLD 
        public bool SmallPosion(Hero hero)
        {
            if (hero.getStatus().currentHP > 0)
            {
                hero.getStatus().currentHP = hero.getStatus().currentHP + 20;
                getCore().CreateEffect(getCore()._healEffect, hero.getAvatarTrans());
                return true;
            }
            else
            {
                getCore().OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool MediumPosion(Hero hero)
        {
            if (hero.getStatus().currentHP > 0)
            {
                hero.getStatus().currentHP = hero.getStatus().currentHP + 50;
                getCore().CreateEffect(getCore()._healEffect, hero.getAvatarTrans());
                return true;
            }
            else
            {
                getCore().OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool LargePosion(Hero hero)
        {
            if (hero.getStatus().currentHP > 0)
            {
                hero.getStatus().currentHP = hero.getStatus().currentHP + 100;
                getCore().CreateEffect(getCore()._healEffect, hero.getAvatarTrans());
                return true;
            }
            else
            {
                getCore().OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
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
