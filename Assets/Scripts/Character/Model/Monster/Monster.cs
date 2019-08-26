
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using controller;
using skill;

namespace model
{
    [System.Serializable]
    public class Monster : Model
    {
        Status _status;

        Sprite[] loadSprite;
        string monsSpriteSet = "";

        Vector3 endPosition;
        Vector3 oldPos;
        bool startInjur;
        bool injur;
        float speed = 4f;
        
        
        public GameObject _icon;
        
        public string[] patternAttack;

        int _attackCount = 0;
        float CriRate = 0.2f;

        public Monster(int slot, int level, ModelDataSet data)
        {
            setModel(slot, data, GameCore.call()._monPanel.GetComponent<MonPanel>()._monAvatarList[slot]);
            _status = new Status(level, data);
        }
        
        public Status getStatus()
        {
            return _status;
        }
        
        public void HPActive()
        {
            getBatCon().UpdateMonsterHP();
        }

       
        public void HateActive()
        {
            _icon.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
            
        }
        
        public void Dead()
        {
            getAvatarTrans().parent.localScale = getOriginalSize();
            
            _icon.transform.localScale = new Vector3(1, 1, 1);
            _status.hate = 0;
            _icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(46, 46, 46, 150);
            getBatCon()._monster.Remove(this);
            if (getBatCon()._monster.ToList().Count == 0)
            {
                getBatCon()._waitEndTurn = false;
                getBatCon().OnBattleEnd(true);
            }
            else
            {
                getBatCon()._focusMonster++;
            }
            
        }

        public void Attack(Hero target)
        {
            if (getBatCon()._crystalMon <= 0)
            {
                //Debug.Log("end 3");
                getBatCon()._waitEndTurn = true;
                return;
            }
            getAnim().SetTrigger("IsAttack");
            //_anim.SetBool("IsAttack", true);
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            int useCrystal = Random.Range(1, 11);
            //Debug.Log("b crystal " + _battleCon._crystalMon+"use "+ useCrystal);
            _status.setCurrentSkill(_status.attack[0]);
            
            if (_status.hate >= 100)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    _status.hate = 0;
                    useCrystal = _status.attack[1].skill.crystal;
                    _status.setCurrentSkill(_status.attack[1]);
                }
            }
            int crtBefore = getBatCon()._crystalMon;
            getBatCon()._crystalMon -= useCrystal;

            if (GameCore.call()._buffCon._defenseList.ToList().Count >0 && GameCore.call()._buffCon._defenseList[0].crystal == (useCrystal > crtBefore ? crtBefore > 3? crtBefore / 3 : crtBefore : useCrystal) && _status.getCurrentSkill().skill.effect != "")
            {
                getBatCon().RunCounterAttack();
                target.getStatus().hate += (int)(_status.getCurrentSkill().hate*1.5);
            }
            else
            {
                _status.setCurrentDmg(_status.CalDmgSkill(_status.getCurrentSkill().skill.type, _status.getCurrentSkill().skill.bonusDmg, target.getStatus(), 1));
                
                target.getStatus().currentHP -= _status.getCurrentDmg();
                for (int i = 0; i < _status.getCurrentSkill().buff.Count; i++)
                {
                    _status.getCurrentSkill().buff[i].startTime = getBatCon()._turnAround;
                    _status.getCurrentSkill().buff[i].originalSize = getOriginalSize();
                    GameCore.call()._buffCon.AddBuff(_status.getCurrentSkill().buff[i]);
                }
                target.getStatus().hate += _status.getCurrentSkill().hate;
                getBatCon().AttackEffect(_status.getCurrentSkill(), _status.getCurrentDmg(), _Model.PLAYER);
            }

            _status.hate += _status.getCurrentSkill().hate / 3;
            GameCore.call()._buffCon.RemoveDefense(0);
            _attackCount++;
        }
        public void LoadAvatar()
        {
            LoadSprite();
            _status.hate = 0;
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
        }
        
        public void PlayInjury(int dmg)
        {
            
            injur = true;
            oldPos = getAvatarTrans().position;
            endPosition = new Vector3(getAvatarPos().x - 0.8f, getAvatarPos().y, getAvatarPos().z);

            setOriginalMat(getAvatarMat());
            setAvatarMat(getBatCon()._injuryMat);
            
            getAnim().SetTrigger("IsInjury");
            startInjur = true;
            OnPassiveWorking(getStatus().passive.ToString(), _Model.MONSTER);
            getBatCon().ShowDamage(dmg, getAvatarPos());
            if (_status.currentHP <= 0)
            {
                getAnim().SetTrigger("IsDead");
                Dead();
            }
            else
            {

                if (getBatCon()._counterATKSuccess)
                {
                    getBatCon()._counterATKSuccess = false;
                    getBatCon()._focusHero++;
                }
                getBatCon()._focusMonster++;
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
            else if(injur)
            {
                injur = false;
                endPosition = oldPos;
                return true;
            }
            if (startInjur)
            {
                Debug.Log("check injury3");
                setAvatarMat(getOriginalMat());
            }
            startInjur = false;
            return false;
        }
        
    }
}

