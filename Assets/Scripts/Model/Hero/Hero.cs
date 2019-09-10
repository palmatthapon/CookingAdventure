using system;
using System.Linq;
using UnityEngine;

namespace model
{
    public class Hero : Model
    {
        int _storeId;
        Status _status;
        
        public Hero(int storeId, int slot,double exp, ModelDataSet data)
        {
            setModel(data);
            _storeId = storeId;
            _status = new Status(exp, data);

        }
        
        public int getStoreId()
        {
            return _storeId;
        }

        public Status getStatus()
        {
            return _status;
        }
        
        public void UpdateHPBar()
        {
            getCore()._playerLifePanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            getCore()._playerLifePanel.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
        }
        
        protected override void Dead()
        {
            base.Dead();
            _status.hate = 0;
            getBattCon().OnBattleEnd(false);
        }
        

        public void Attack(int slot,int skillStack=1)
        {
            getAnim().SetTrigger("IsAttack");

            _status.setCurrentSkill(_status.attack[slot]);

            Monster target = getBattCon().getTargetOfHero();
            OnPassiveWorking(getStatus().passive.ToString(), MODEL.PLAYER);

            Debug.Log("bonusdmg skill " + _status.getCurrentSkill().bonusDmg);

            target.getStatus().setDamageReceived(_status.CalDmgSkill(_status.getCurrentSkill().type, _status.getCurrentSkill().bonusDmg, target.getStatus(), skillStack));
            
            target.getStatus().hate += _status.getCurrentSkill().getHate();

            _status.hate += (_status.getCurrentSkill().getHate() / 3)* skillStack;

            CreateAttackEffect(_status.getCurrentSkill(), MODEL.MONSTER);
            
        }

        public void CreateAvatar(int slot)
        {
            setSlot(slot);
            setAvatar(getBattCon()._heroAvatar[slot]);
            LoadAvatar();
            _status.ResetHP();
            _status.hate = 0;
            getCore()._playerLifePanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            OnPassiveWorking(getStatus().passive.ToString(), MODEL.PLAYER);
        }

        public void PlayInjury()
        {

            RunInjury( new Vector3(getAvatarTrans().position.x + 0.8f, getAvatarTrans().position.y, getAvatarTrans().position.z));
            
            OnPassiveWorking(getStatus().passive.ToString(), MODEL.PLAYER);

            getBattCon().ShowDamage(_status.getDamageReceived(), getAvatarTrans().position);
            _status.currentHP = _status.currentHP - _status.getDamageReceived();
            UpdateHPBar();
            if (_status.currentHP <= 0)
            {
                Dead();
            }
            else
            {
                if (getBattCon()._crystalMon>0)
                {
                    Debug.Log("runAI 3");
                    getBattCon().RunMonsterAI();
                }
                else
                {
                    getBattCon()._battleState = BATTLESTATE.Finish;
                }
                
            }
        }
    }
}

