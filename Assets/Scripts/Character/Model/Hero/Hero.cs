using system;
using System.Linq;
using UnityEngine;

namespace model
{
    [System.Serializable]
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
        
        public void UpdateCurrentHPMax(int h)
        {
            _status.currentHPMax += h;
            _status.currentHP += h;
        }

        public void HPActive()
        {
            getCore()._playerLifePanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            
        }

        public void HateActive()
        {
            if (_status.hate == 100)
            {
                _status.hate = 0;
                getAtkCon().LoadUltimate(getSlot());
            }
            getCore()._playerLifePanel.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
        }
        
        public void Dead()
        {
            getAvatarTrans().parent.localScale = getOriginalSize();
            _status.hate = 0;
            getBattCon()._hero.Remove(this);
            if (getBattCon()._hero.ToList().Count == 0)
            {
                getBattCon()._waitEndTurn = false;
                getBattCon().OnBattleEnd(false);
            }
            else
            {
                if(getBattCon()._monster.ToList().Count == 0)
                {
                    getBattCon()._waitEndTurn = false;
                }
                else
                {
                    Debug.Log("runAI 3");
                    getBattCon()._focusHero++;
                    getBattCon().RunMonAI();
                }
            }
        }
        

        public void Attack(bool ultimate = false,int skillStack=1)
        {
            getAnim().SetTrigger("IsAttack");
            if (ultimate)
                _status.setCurrentSkill(_status.attack[1]);
            else
                _status.setCurrentSkill(_status.attack[0]);
            
            Monster target = getBattCon().FocusMonster();
            OnPassiveWorking(getStatus().passive.ToString(), _Model.PLAYER);

            target.getStatus().setDamageReceived(_status.CalDmgSkill(_status.getCurrentSkill().data.type, _status.getCurrentSkill().data.bonusDmg, target.getStatus(), skillStack));
            
            target.getStatus().hate += _status.getCurrentSkill().hate;

            _status.hate += (_status.getCurrentSkill().hate / 3)* skillStack;

            CreateAttackEffect(_status.getCurrentSkill().data, _Model.MONSTER);
            
        }

        public void LoadAvatar(int slot)
        {
            setSlot(slot);
            setAvatar(getBattCon()._heroAvatar[slot]);
            LoadSprite();
            _status.ResetHP();
            _status.hate = 0;
            getCore()._playerLifePanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            OnPassiveWorking(getStatus().passive.ToString(), _Model.PLAYER);
            getBattCon()._battleMode = _BattleState.Finish;
        }

        public void PlayInjury()
        {

            RunInjury( new Vector3(getAvatarPos().x + 0.8f, getAvatarPos().y, getAvatarPos().z));
            
            OnPassiveWorking(getStatus().passive.ToString(),_Model.PLAYER);

            getBattCon().ShowDamage(_status.getDamageReceived(), getAvatarPos());

            if (_status.currentHP <= 0)
            {
                getAnim().SetTrigger("IsDead");
                Dead();
            }
            else
            {
                getBattCon().RunMonAI();
            }
        }
    }
}

