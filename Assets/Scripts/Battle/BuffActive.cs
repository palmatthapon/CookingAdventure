
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using model;
using controller;

namespace skill
{
    public class BuffActive
    {
        
        public void Buff1(Buff data)
        {
            if (data.remove)
            {
                    
            }
            //Debug.Log("Buff1");
        }

        public void Buff2(Buff data)
        {
            if (data.remove)
            {

            }
            //Debug.Log("Buff2");
        }

        public void Buff3(Buff data)
        {
            if (data.remove)
            {
                

            }
            //Debug.Log("Buff3");
        }
        
        public void Buff4(Buff data)
        {
            if (data.remove)
            {

            }
            //Debug.Log("Buff4");
        }

        public void Buff5(Buff data)
        {
            /*
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    foreach (Hero h in _battleCon._hero)
                    {
                        h._status._buffBonusDEF = 1.25f;
                    }
                }
                else
                {
                    foreach (Monster m in _battleCon._monster)
                    {
                        m._status._buffBonusDEF = 1.25f;
                    }
                }
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    foreach (Hero h in _battleCon._hero)
                    {
                        h._status._buffBonusDEF = 1;
                    }
                }
                else
                {
                    foreach (Monster m in _battleCon._monster)
                    {
                        m._status._buffBonusDEF = 1;
                    }
                }
            }*/

            //Debug.Log("Buff5");
        }

        public void Buff6(Buff data)
        {
            /*
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.FocusHero()._status._buffBonusATK = 1.25f;
                else
                    _battleCon.FocusMonster()._status._buffBonusATK = 1.25f;
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.FocusHero()._status._buffBonusATK = 1;
                else
                    _battleCon.FocusMonster()._status._buffBonusATK = 1;
            }*/
            //Debug.Log("Buff6");
        }

        public void Buff7(Buff data)
        {
            /*
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    _battleCon.FocusHero()._avatar.transform.parent.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    _battleCon.FocusHero().UpdateCurrentHPMax(100);
                }
                else
                {
                    _core._monPanel.GetComponent<MonPanel>()._monAvatarList[0].transform.parent.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    _battleCon.FocusMonster().UpdateCurrentHPMax(100);

                }
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    _battleCon.FocusHero()._avatar.transform.parent.localScale = data.originalSize;
                    int hp = _battleCon.FocusHero()._status.currentHP * _battleCon.FocusHero()._status.hpMax / _battleCon.FocusHero()._status.hpMax;
                    _battleCon.FocusHero()._status.hpMax = _battleCon.FocusHero()._status.hpMax;
                    _battleCon.FocusHero()._status.currentHP = hp;
                }
                else
                {
                    _core._monPanel.GetComponent<MonPanel>()._monAvatarList[0].transform.parent.localScale = data.originalSize;
                    int hp = _battleCon.FocusMonster()._status.currentHP * _battleCon.FocusMonster()._originalHPMax / _battleCon.FocusMonster()._status.hpMax;
                    _battleCon.FocusMonster()._status.hpMax = _battleCon.FocusMonster()._originalHPMax;
                    _battleCon.FocusMonster()._status.currentHP = hp;
                }
            }*/
            //Debug.Log("Buff7");
        }

        public void Buff8(Buff data)
        {
            /*
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.FocusHero()._status._buffBonusMATK = 1.25f;
                else
                    _battleCon.FocusMonster()._status._buffBonusMATK = 1.25f;
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.FocusHero()._status._buffBonusMATK = 1;
                else
                    _battleCon.FocusMonster()._status._buffBonusMATK = 1;
            }*/
            //Debug.Log("Buff6");
        }
    }

}
