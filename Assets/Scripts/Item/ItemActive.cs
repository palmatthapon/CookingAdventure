
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
        GameCore _core;
        BattleController _battleCon;
        AttackController _attackCon;

        void Run()
        {
            if(_core==null)
                _core = Camera.main.GetComponent<GameCore>();
            if (_battleCon == null)
                _battleCon = _core._battleObj.GetComponent<BattleController>();
            if(_attackCon == null)
                _attackCon = _core._attackPanel.GetComponent<AttackController>();
        }
        
        //how to name for HeroPassiveAbility Method, Use all caps e.g. HELLOWORLD 
        public bool SmallPosion(Hero hero)
        {
            Run();
            if (hero.GetStatus().currentHP > 0)
            {
                hero.GetStatus().currentHP = hero.GetStatus().currentHP + 20;
                if (_core._gameMode == _GameStatus.BATTLE)
                {
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.GetStoreId() == hero.GetStoreId())
                        {
                            //h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.GetStatus().currentHP * 1 / hero.GetStatus().hpMax);
                            break;
                        }
                    }
                }
                _core.CreateEffect(_core._healEffect, hero.obj.transform);
                return true;
            }
            else
            {
                _core.OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool MediumPosion(Hero hero)
        {
            Run();
            if (hero.GetStatus().currentHP > 0)
            {
                hero.GetStatus().currentHP = hero.GetStatus().currentHP + 50;
                if(_core._gameMode == _GameStatus.BATTLE)
                {
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.GetStoreId() == hero.GetStoreId())
                        {
                            //h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.GetStatus().currentHP * 1 / hero.GetStatus().hpMax);
                            break;
                        }
                    }
                }
                
                _core.CreateEffect(_core._healEffect, hero.obj.transform);
                return true;
            }
            else
            {
                _core.OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool LargePosion(Hero hero)
        {
            Run();
            if (hero.GetStatus().currentHP > 0)
            {
                hero.GetStatus().currentHP = hero.GetStatus().currentHP + 100;
                if (_core._gameMode == _GameStatus.BATTLE)
                {
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.GetStoreId() == hero.GetStoreId())
                        {
                            //h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.GetStatus().currentHP * 1 / hero._status.hpMax);
                            break;
                        }
                    }
                }
                _core.CreateEffect(_core._healEffect, hero.obj.transform);
                return true;
            }
            else
            {
                _core.OpenErrorNotify("ใช้กับฮีโร่ที่ตายไม่ได้!");
                return false;
            }
        }

        public bool ReCrystal(Hero hero)
        {
            Run();
            //Debug.Log("Re actionPoint ");
            return true;
        }

        public bool Antidote(Hero hero)
        {
            Run();
            //Debug.Log("Re actionPoint ");
            return true;
        }

        public bool Revive(Hero hero)
        {
            Run();
            if (hero.GetStatus().currentHP == 0)
            {
                hero.GetStatus().currentHP = 1;
                if (_core._gameMode == _GameStatus.BATTLE)
                {
                    int index = 0;
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.GetStoreId() == hero.GetStoreId())
                        {
                            //h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.GetStatus().currentHP * 1 / hero.GetStatus().hpMax);
                            h.Revive();
                            _battleCon._hero.Add(h);
                            _battleCon._hero = _battleCon._hero.OrderBy(o => o.GetSlot()).ToList();
                            break;
                        }
                        index++;
                    }
                }
                _core.OpenTrueNotify("ชุบชีวิตฮีโร่สำเร็จ!");
                _attackCon.UpdateAttackSlot();
                return true;
            }
            else
            {
                _core.OpenErrorNotify("ใช้กับฮีโร่นี้ไม่ได้!");
                return false;
            }
        }
    }
}
