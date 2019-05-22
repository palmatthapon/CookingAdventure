using CollectionData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;
using Core;
using UI;

namespace CollectionData
{
    public class PassiveAbility
    {
        BattleController _battleCon;
        MainCore _core;

        void Run()
        {
            if (_core == null)
                _core = Camera.main.GetComponent<MainCore>();
            if (_battleCon == null)
                _battleCon = _core._battleObj.GetComponent<BattleController>();
            _battleCon.GetHeroFocus()._passiveBonusATK = 1;
            _battleCon.GetHeroFocus()._passiveBonusMATK = 1;
            _battleCon.GetHeroFocus()._passiveBonusDEF = 1;
            _battleCon.GetHeroFocus()._passiveBonusMDEF = 1;
            _battleCon.GetMonFocus()._passiveBonusATK = 1;
            _battleCon.GetMonFocus()._passiveBonusMATK = 1;
            _battleCon.GetMonFocus()._passiveBonusDEF = 1;
            _battleCon.GetMonFocus()._passiveBonusMDEF = 1;
        }

        public void Passive1(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp == 0)
                {
                    if (_battleCon._currentEvent == _event.Wind)
                    {
                        //_battleCon.ShowPassive("[บุตรพระพาย]ทำงาน...", _Model.PLAYER);
                        _battleCon.GetHeroFocus().hp += 50;
                        _core.CreateEffect(_core._healEffect, _battleCon.GetHeroFocus()._avatar.transform);
                    }
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp == 0)
                {
                    if (_battleCon._currentEvent == _event.Wind)
                    {
                        //_battleCon.ShowPassive("[บุตรพระพาย]ทำงาน...", _Model.MONSTER);
                        _battleCon.GetMonFocus().hp += 50;
                        _core.CreateEffect(_core._healEffect, _core._monPanel.GetComponent<MonPanel>()._monAvatarList[0].transform);
                    }
                }
            }
            //Debug.Log("Passive1...");
        }

        public void Passive2(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp < _battleCon.GetHeroFocus().hero.hpMax / 2)
                {
                    _battleCon.GetHeroFocus()._passiveBonusATK = 1.25f;
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp < _battleCon.GetMonFocus().hpMax / 2)
                {
                    _battleCon.GetMonFocus()._passiveBonusATK = 1.25f;
                }
            }
            //Debug.Log("Passive2...");
        }

        public void Passive3(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp > _battleCon.GetHeroFocus().hero.hpMax / 2)
                {
                    _battleCon.GetHeroFocus()._passiveBonusMDEF = 1.25f;
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp > _battleCon.GetMonFocus().hpMax / 2)
                {
                    _battleCon.GetMonFocus()._passiveBonusMDEF = 1.25f;
                }
            }
            //Debug.Log("Passive3");
        }

        public void Passive4(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp == 0)
                {
                    if (_battleCon.GetHeroFocus().Hate > 75)
                    {
                        //_battleCon.ShowPassive("[ผู้เป็นอมตะ]ทำงาน...", _Model.PLAYER);
                        _battleCon.GetHeroFocus().hp += _battleCon.GetHeroFocus().hero.hpMax/2;
                    }
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp == 0)
                {
                    if (_battleCon.GetMonFocus().Hate > 75)
                    {
                        //_battleCon.ShowPassive("[ผู้เป็นอมตะ]ทำงาน...", _Model.MONSTER);
                        _battleCon.GetMonFocus().hp += _battleCon.GetMonFocus().hpMax/2;
                    }
                }
            }
            //Debug.Log("Passive4");
        }

        public void Passive5(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().skill == _battleCon.GetHeroFocus().hero.attack[1])
                {
                    if (_battleCon.GetMonFocus().hp <= _battleCon.GetMonFocus().hpMax / 4)
                    {
                        //_battleCon.ShowPassive("[ความสามารถติดตัว]ศรปลิดชีพทำงาน...", _Model.PLAYER);
                        _battleCon.GetHeroFocus()._passiveBonusMATK = 2;
                    }
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().skill == _battleCon.GetMonFocus().attack[1])
                {
                    if (_battleCon.GetHeroFocus().hp <= _battleCon.GetHeroFocus().hero.hpMax / 4)
                    {
                        //_battleCon.ShowPassive("[ความสามารถติดตัว]ศรปลิดชีพทำงาน...", _Model.MONSTER);
                        _battleCon.GetMonFocus()._passiveBonusATK = 2;
                    }
                }
            }
            //Debug.Log("Passive5");
        }

        public void Passive6(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp == 0)
                {
                    foreach(Hero hero in _battleCon._hero)
                    {
                        if(hero.hero.id != _battleCon.GetHeroFocus().hero.id)
                        {
                            
                            hero.hp += _battleCon.GetHeroFocus().hero.hpMax / 2;
                            _core.CreateEffect(_core._healEffect, hero._avatar.transform);
                        }
                    }
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp == 0)
                {
                    foreach (Monster mons in _battleCon._monster)
                    {
                        if (mons.slotId != _battleCon.GetMonFocus().slotId)
                        {
                            mons.hp += _battleCon.GetMonFocus().hpMax / 2;
                            _core.CreateEffect(_core._healEffect, mons._avatar.transform);
                        }
                    }
                }
            }
            //Debug.Log("Passive6");
        }

        public void Passive7(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp == 0)
                {
                    foreach(Hero hero in _battleCon._hero)
                    {
                        if(hero.hp > 0)
                        {
                            hero.Hate += _battleCon.GetHeroFocus().Hate;
                        }
                    }
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp == 0)
                {
                    foreach (Monster mon in _battleCon._monster)
                    {
                        if (mon.hp > 0)
                        {
                            mon.Hate += _battleCon.GetMonFocus().Hate;
                        }
                    }
                }
            }
            //Debug.Log("Passive7");
        }

        public void Passive8(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp < _battleCon.GetHeroFocus().hero.hpMax / 4)
                {
                    _battleCon.GetHeroFocus()._passiveBonusDEF = 1.25f;
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp < _battleCon.GetMonFocus().hpMax / 4)
                {
                    _battleCon.GetMonFocus()._passiveBonusDEF = 1.25f;
                }
            }
            //Debug.Log("Passive8");
        }

        public void Passive9(_Model type)
        {
            Run();
            if (type == _Model.PLAYER)
            {
                if (_battleCon.GetHeroFocus().hp < _battleCon.GetHeroFocus().hero.hpMax / 3)
                {
                    if (_battleCon._counterATKSuccess)
                    {
                        _battleCon.GetHeroFocus().hp += _battleCon._counterATKDamage / 2;
                    }
                }
            }
            else
            {
                if (_battleCon.GetMonFocus().hp < _battleCon.GetMonFocus().hpMax / 3)
                {
                    if (_battleCon._counterATKSuccess)
                    {
                        _battleCon.GetMonFocus().hp += _battleCon._counterATKDamage / 3;
                    }
                }
            }
            //Debug.Log("Passive9");
        }


    }

}
