
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using model;
using player;
using monster;
using controller;

namespace skill
{
    public class BuffActive
    {
        BattleController _battleCon;
        GameCore _core;

        void run()
        {
            if (_core == null)
                _core = Camera.main.GetComponent<GameCore>();
            if (_battleCon == null)
                _battleCon = _core._battleObj.GetComponent<BattleController>();
        }
        
        public void Buff1(Buff data)
        {
            run();
            if (data.remove)
            {
                    
            }
            //Debug.Log("Buff1");
        }

        public void Buff2(Buff data)
        {
            run();
            if (data.remove)
            {

            }
            //Debug.Log("Buff2");
        }

        public void Buff3(Buff data)
        {
            run();
            if (data.remove)
            {
                

            }
            //Debug.Log("Buff3");
        }
        
        public void Buff4(Buff data)
        {
            run();
            if (data.remove)
            {

            }
            //Debug.Log("Buff4");
        }

        public void Buff5(Buff data)
        {
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    foreach (Hero h in _battleCon._hero)
                    {
                        h._buffBonusDEF = 1.25f;
                    }
                }
                else
                {
                    foreach (Monster m in _battleCon._monster)
                    {
                        m._buffBonusDEF = 1.25f;
                    }
                }
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    foreach (Hero h in _battleCon._hero)
                    {
                        h._buffBonusDEF = 1;
                    }
                }
                else
                {
                    foreach (Monster m in _battleCon._monster)
                    {
                        m._buffBonusDEF = 1;
                    }
                }
            }

            //Debug.Log("Buff5");
        }

        public void Buff6(Buff data)
        {
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.GetHeroFocus()._buffBonusATK = 1.25f;
                else
                    _battleCon.GetMonFocus()._buffBonusATK = 1.25f;
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.GetHeroFocus()._buffBonusATK = 1;
                else
                    _battleCon.GetMonFocus()._buffBonusATK = 1;
            }
            //Debug.Log("Buff6");
        }

        public void Buff7(Buff data)
        {
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    _battleCon.GetHeroFocus()._avatar.transform.parent.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    _battleCon.GetHeroFocus().UpdatehpMax(100);
                }
                else
                {
                    _core._monPanel.GetComponent<MonPanel>()._monAvatarList[0].transform.parent.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    _battleCon.GetMonFocus().UpdatehpMax(100);

                }
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                {
                    _battleCon.GetHeroFocus()._avatar.transform.parent.localScale = data.originalSize;
                    int hp = _battleCon.GetHeroFocus().hp * _battleCon.GetHeroFocus()._originalHPMax / _battleCon.GetHeroFocus().hero.hpMax;
                    _battleCon.GetHeroFocus().hero.hpMax = _battleCon.GetHeroFocus()._originalHPMax;
                    _battleCon.GetHeroFocus().hp = hp;
                }
                else
                {
                    _core._monPanel.GetComponent<MonPanel>()._monAvatarList[0].transform.parent.localScale = data.originalSize;
                    int hp = _battleCon.GetMonFocus().hp * _battleCon.GetMonFocus()._originalHPMax / _battleCon.GetMonFocus().hpMax;
                    _battleCon.GetMonFocus().hpMax = _battleCon.GetMonFocus()._originalHPMax;
                    _battleCon.GetMonFocus().hp = hp;
                }
            }
            //Debug.Log("Buff7");
        }

        public void Buff8(Buff data)
        {
            run();
            if (!data.remove)
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.GetHeroFocus()._buffBonusMATK = 1.25f;
                else
                    _battleCon.GetMonFocus()._buffBonusMATK = 1.25f;
            }
            else
            {
                if ((data.whoUse == _Model.PLAYER && data.forMe) || (data.whoUse == _Model.MONSTER && !data.forMe))
                    _battleCon.GetHeroFocus()._buffBonusMATK = 1;
                else
                    _battleCon.GetMonFocus()._buffBonusMATK = 1;
            }
            //Debug.Log("Buff6");
        }
    }

}
