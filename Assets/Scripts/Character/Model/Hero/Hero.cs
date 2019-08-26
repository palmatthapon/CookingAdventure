using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace model
{
    [System.Serializable]
    public class Hero : Model
    {
        int _storeId;
        Status _status;
        
        Vector3 endPosition;
        Vector3 oldPos;
        bool startInjur;
        bool injur;
        float speed = 4f;

        public Hero(int storeId, int slot,double exp, ModelDataSet data)
        {
            setModel(slot, data,GameCore.call()._heroAvatar);
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
            GameCore.call()._playerLifePanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            
        }

        public void HateActive()
        {
            if (_status.hate == 100)
            {
                _status.hate = 0;
                GameCore.call()._attackCon.LoadUltimate(getSlot());
            }
            GameCore.call()._playerLifePanel.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
        }

        public int getCounterATKDmg(Monster target)
        {
            return _status.CalDmgCounterAttack(getSlot(),target.getStatus(),target.getSlot());
        }
        
        public void Revive()
        {
            GameCore.call()._playerLifePanel.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 150);
            getAnim().SetTrigger("IsBorn");
            
        }
        
        public void Dead()
        {
            getAvatarTrans().parent.localScale = getOriginalSize();
            _status.hate = 0;
            getBatCon()._hero.Remove(this);
            if (getBatCon()._hero.ToList().Count == 0)
            {
                getBatCon()._waitEndTurn = false;
                getBatCon().OnBattleEnd(false);
            }
            else
            {
                if(getBatCon()._monster.ToList().Count == 0)
                {
                    getBatCon()._waitEndTurn = false;
                }
                else
                {
                    Debug.Log("runAI 3");
                    getBatCon()._focusHero++;
                    getBatCon().RunMonAI();
                }
            }
        }

        public void AddDefenseList(int crystal)
        {
            getbuffCon().AddDefense(crystal);
        }

        public void Attack(bool ultimate = false,bool questionFail=false,int skillStack=1)
        {
            getAnim().SetTrigger("IsAttack");
            if (ultimate)
                _status.setCurrentSkill(_status.attack[1]);
            else
                _status.setCurrentSkill(_status.attack[0]);
            
            Monster target = getBatCon().FocusMonster();
            OnPassiveWorking(getStatus().passive.ToString(), _Model.PLAYER);

            for (int i = 0; i < _status.getCurrentSkill().buff.Count; i++)
            {
                _status.getCurrentSkill().buff[i].startTime = getBatCon()._turnAround;
                _status.getCurrentSkill().buff[i].originalSize = getOriginalSize();
                getbuffCon().AddBuff(_status.getCurrentSkill().buff[i]);
            }
            
            if (_status.getCurrentSkill().skill.type != _Attack.BUFF)
            {
                //Damage = Damage + 500;//for tester.

                _status.setCurrentDmg(_status.CalDmgSkill(_status.getCurrentSkill().skill.type, _status.getCurrentSkill().skill.bonusDmg, target.getStatus(), skillStack));

                
                int hpOld = target.getStatus().currentHP;
                if (questionFail)
                    _status.setCurrentDmg(_status.getCurrentDmg() / 2);
                //Debug.Log("hero damage final " + _damage);
                target.getStatus().currentHP -= _status.getCurrentDmg();
                getBatCon()._damage_of_each_hero[getBatCon().FocusMonster().getSlot(), getSlot()] += hpOld - target.getStatus().currentHP;
            }
            else
            {
                _status.setCurrentDmg(_status.getCurrentDmg() * 0);
            }
            
            target.getStatus().hate += _status.getCurrentSkill().hate;
            _status.hate += (_status.getCurrentSkill().hate / 3)* skillStack;
            
            getBatCon().AttackEffect(_status.getCurrentSkill(), _status.getCurrentDmg(), _Model.MONSTER);
            
        }

        public void LoadAvatar()
        {
            LoadSprite();
            _status.ResetHP();
            _status.hate = 0;
            GameCore.call()._playerLifePanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            OnPassiveWorking(getStatus().passive.ToString(), _Model.PLAYER);
        }
        
        public void PlayInjury(int dmg)
        {
            injur = true;
            oldPos = getAvatarPos();
            endPosition = new Vector3(getAvatarPos().x + 0.8f, getAvatarPos().y, getAvatarPos().z);

            setOriginalMat(getAvatarMat());
            setAvatarMat(getBatCon()._injuryMat);

            getAnim().SetTrigger("IsInjury");
            startInjur = true;
            OnPassiveWorking(getStatus().passive.ToString(),_Model.PLAYER);

            getBatCon().ShowDamage(dmg, getAvatarPos());

            if (_status.currentHP <= 0)
            {
                getAnim().SetTrigger("IsDead");
                Dead();
            }
            else
            {
                //Debug.Log("runAI 2");
                if (!getBatCon()._counterATKSuccess)
                {
                    getBatCon()._focusHero++;
                }
                getBatCon().RunMonAI();
            }
            
            
        }

        public bool CheckInjury()
        {
            if (getAnim() == null) return false;
            if (startInjur && getAvatarPos() != endPosition)
            {
                setAvatarPos(Vector3.MoveTowards(getAvatarPos(), endPosition, speed * Time.deltaTime));
                return true;
            }
            else if (injur)
            {
                injur = false;
                endPosition = oldPos;
                return true;
            }
            if (startInjur)
            {
                Debug.Log("check injury");
                setAvatarMat(getOriginalMat());
            }
            startInjur = false;
            return false;
        }

        
    }
}

