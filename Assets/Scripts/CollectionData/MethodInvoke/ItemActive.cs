using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CollectionData
{
    public class ItemActive
    {
        MainCore _core;
        BattleController _battleCon;
        SelectAttackController _selectATKCon;

        void Run()
        {
            if(_core==null)
                _core = Camera.main.GetComponent<MainCore>();
            if (_battleCon == null)
                _battleCon = _core._battleObj.GetComponent<BattleController>();
            if(_selectATKCon == null)
                _selectATKCon = _core._attackPanel.GetComponent<SelectAttackController>();
        }
        
        //how to name for HeroPassiveAbility Method, Use all caps e.g. HELLOWORLD 
        public bool SmallPosion(HeroStore hero)
        {
            Run();
            if (hero.hp > 0)
            {
                hero.hp = hero.hp + 20;
                if (_core._gameMode == _GameStatus.BATTLE)
                {
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.hero.id == hero.id)
                        {
                            h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.hp * 1 / hero.hpMax);
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

        public bool MediumPosion(HeroStore hero)
        {
            Run();
            if (hero.hp > 0)
            {
                hero.hp = hero.hp + 50;
                if(_core._gameMode == _GameStatus.BATTLE)
                {
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.hero.id == hero.id)
                        {
                            h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.hp * 1 / hero.hpMax);
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

        public bool LargePosion(HeroStore hero)
        {
            Run();
            if (hero.hp > 0)
            {
                hero.hp = hero.hp + 100;
                if (_core._gameMode == _GameStatus.BATTLE)
                {
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.hero.id == hero.id)
                        {
                            h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.hp * 1 / hero.hpMax);
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

        public bool ReCrystal(HeroStore hero)
        {
            Run();
            //Debug.Log("Re actionPoint ");
            return true;
        }

        public bool Antidote(HeroStore hero)
        {
            Run();
            //Debug.Log("Re actionPoint ");
            return true;
        }

        public bool Revive(HeroStore hero)
        {
            Run();
            if (hero.hp == 0)
            {
                hero.hp = 1;
                if (_core._gameMode == _GameStatus.BATTLE)
                {
                    int index = 0;
                    foreach (Hero h in _battleCon._heroData)
                    {
                        if (h.hero.id == hero.id)
                        {
                            h._icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.hp * 1 / hero.hpMax);
                            h.Revive();
                            _battleCon._hero.Add(h);
                            _battleCon._hero = _battleCon._hero.OrderBy(o => o.slotId).ToList();
                            break;
                        }
                        index++;
                    }
                }
                foreach (Hero r in _battleCon._hero)
                {
                    Debug.Log(r.hero.hero.name +" "+r.slotId);
                }
                _core.OpenTrueNotify("ชุบชีวิตฮีโร่สำเร็จ!");
                _selectATKCon.UpdateAttackSlot();
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
