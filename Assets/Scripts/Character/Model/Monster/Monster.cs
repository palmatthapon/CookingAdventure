using system;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace model
{
    [System.Serializable]
    public class Monster : Model
    {
        Status _status;
        
        public string[] patternAttack;

        public Monster(int level, ModelDataSet data)
        {
            setModel(data);
            _status = new Status(level, data);
        }
        
        public Status getStatus()
        {
            return _status;
        }
        
        public void HPActive()
        {
            getBattCon().UpdateMonsterHP();
        }

       
        public void HateActive()
        {
            //_icon.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
            
        }
        
        public void Dead()
        {
            getAvatarTrans().parent.localScale = getOriginalSize();
            _status.hate = 0;
            getBattCon()._monster.Remove(this);
            if (getBattCon()._monster.ToList().Count == 0)
            {
                getBattCon()._waitEndTurn = false;
                getBattCon().OnBattleEnd(true);
            }
            else
            {
                getBattCon()._focusMonster++;
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
                    useCrystal = _status.attack[1].data.crystal;
                    _status.setCurrentSkill(_status.attack[1]);
                }
            }
            int crtBefore = getBattCon()._crystalMon;
            getBattCon()._crystalMon -= useCrystal;

            target.getStatus().setDamageReceived(_status.CalDmgSkill(_status.getCurrentSkill().data.type, _status.getCurrentSkill().data.bonusDmg, target.getStatus(), 1));

            target.getStatus().hate += _status.getCurrentSkill().hate;
            CreateAttackEffect(_status.getCurrentSkill().data, _Model.PLAYER);

            _status.hate += _status.getCurrentSkill().hate / 3;
        }
        public void LoadAvatar(int slot)
        {
            setSlot(slot);
            setAvatar(getBattCon()._monAvatar[slot]);
            LoadSprite();
            _status.hate = 0;
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            getBattCon()._battleMode = _BattleState.Finish;
        }

        public void PlayInjury()
        {
            
            RunInjury(new Vector3(getAvatarPos().x - 0.8f, getAvatarPos().y, getAvatarPos().z));
            
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            getBattCon().ShowDamage(_status.getDamageReceived(), getAvatarPos());
            if (_status.currentHP <= 0)
            {
                getAnim().SetTrigger("IsDead");
                Dead();
            }
            else
            {
                getBattCon()._focusMonster++;
            }
        }
    }
}

