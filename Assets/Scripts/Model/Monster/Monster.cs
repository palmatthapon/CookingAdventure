using system;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace model
{
    public class Monster : Model
    {
        Status _status;
        
        public string[] patternAttack;

        public Monster(int level, ModelDataSet data)
        {
            setModel(data);
            _status = new Status(level, data);
        }

        public Monster Copy()
        {
            Monster copy = new Monster(getStatus().getLvl(),getData());
            copy._status = this._status;
            return copy;
        }

        public Status getStatus()
        {
            return _status;
        }

        public void setStatus(Status sts)
        {
            _status = sts;
        }
        
        public void UpdateHPBar()
        {
            getBattCon()._monsterHPPanel.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.currentHPMax);
            getBattCon()._monsterHPPanel.transform.Find("HatePanel").Find("HateValue").GetComponent<Image>().fillAmount = (float)_status.hate * 1 / 100;

        }

        protected override void Dead()
        {
            base.Dead();
            _status.hate = 0;
            getBattCon()._monsterSlot = getBattCon()._monsterSlot.Where(val => val != getSlot()).ToArray();
            if (getBattCon()._monsterSlot.Length == 0)
            {
                getBattCon()._waitEndTurn = false;
                getBattCon().OnBattleEnd(true);
            }
            else {
                getBattCon().OnNextTargetOfHero();
            }
            
        }

        public void Attack(Hero target)
        {
            if (getBattCon()._crystalMon <= 0)
            {
                getBattCon()._waitEndTurn = true;
                return;
            }
            getAnim().SetTrigger("IsAttack");
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            int useCrystal = Random.Range(1, 11);
            _status.setCurrentSkill(_status.attack[0]);
            
            if (_status.hate >= 100)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    _status.hate = 0;
                    useCrystal = _status.attack[1].crystal;
                    _status.setCurrentSkill(_status.attack[1]);
                }
            }
            int crtBefore = getBattCon()._crystalMon;
            getBattCon()._crystalMon -= useCrystal;

            target.getStatus().setDamageReceived(_status.CalDmgSkill(_status.getCurrentSkill().type, _status.getCurrentSkill().bonusDmg, target.getStatus(), 1));

            target.getStatus().hate += _status.getCurrentSkill().getHate();
            CreateAttackEffect(_status.getCurrentSkill(), _Model.PLAYER);

            _status.hate += _status.getCurrentSkill().getHate() / 3;
        }

        public string getAvatatName()
        {
            return getBattCon()._monAvatar[getSlot()].name;
        }

        public void CreateAvatar(int slot)
        {
            setSlot(slot);
            Debug.Log("avatar slot " + slot + " " + getBattCon()._monAvatar[slot].name);
            setAvatar(getBattCon()._monAvatar[slot]);
            LoadAvatar();
            _status.hate = 0;
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            getBattCon()._battleMode = _BattleState.Finish;
        }

        public void PlayInjury()
        {
            
            RunInjury(new Vector3(getAvatarTrans().position.x - 0.8f, getAvatarTrans().position.y, getAvatarTrans().position.z));
            
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            getBattCon().ShowDamage(_status.getDamageReceived(), getAvatarTrans().position);
            _status.currentHP = _status.currentHP - _status.getDamageReceived();
            UpdateHPBar();
            if (_status.currentHP <= 0)
            {
                Dead();
            }
            else
            {
                getBattCon().OnNextTargetOfHero();
            }
        }
    }
}

