
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using model;
using controller;
using skill;
using character;

namespace model
{
    [System.Serializable]
    public class Monster
    {
        GameCore _core;
        BattleController _battleCon;
        BuffController _buffCon;
        MonPanel _monCom;

        ModelDataSet data;
        Status _status;
        double _expDrop;

        GameObject _avatar;
        public GameObject _icon;
        int slotId;

        int _originalHPMax;
        public string[] attackPattern;

        int _attackCount = 0;
        Skill _currentSkill;
        Animator _anim;
        Vector3 _originalSize;
        float CriRate = 0.2f;
        int Damage;

        public Monster(int slot, int level, ModelDataSet model)
        {
            _core = Camera.main.GetComponent<GameCore>();
            _monCom = _core._monCom;
            _buffCon = _core._buffCon;
            _avatar = _monCom._monAvatarList[slot];
            _anim = _avatar.GetComponent<Animator>();
            _battleCon = _core._battleCon;
            slotId = slot;
            _status = new Status(level, model);
            data = model;
        }

        public int GetSlot()
        {
            return slotId;
        }

        public Status GetStatus()
        {
            return _status;
        }

        public ModelDataSet GetData()
        {
            return data;
        }

        public Animator GetAnim()
        {
            return _anim;
        }

        public GameObject GetAvatar()
        {
            return _avatar;
        }

        public double GetExpDrop()
        {
            return 50.0 + (_status.STR - _status.AGI);
        }

        public int _currentDamage
        {
            get
            {
                return this.Damage;
            }
            set
            {
                if(value < 0)
                {
                    value = 0;
                }
                this.Damage = value;
            }
        }

        public void HPActive()
        {
            _battleCon.UpdateMonsterHP();
        }

        public void UpdateCurrentHPMax(int h)
        {
            _status.currentHPMax += h;
            _status.currentHP += h;
        }

        public void HateActive()
        {
            _icon.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
            
        }
        
        public void Dead()
        {
            _avatar.transform.parent.localScale = _originalSize;
            
            _status.hpMax = _originalHPMax;
            _icon.transform.localScale = new Vector3(1, 1, 1);
            _status.hate = 0;
            _icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(46, 46, 46, 150);
            _battleCon._monster.Remove(this);
            if (_battleCon._monster.ToList().Count == 0)
            {
                _battleCon._waitEndTurn = false;
                _battleCon.OnBattleEnd(true);
            }
            else
            {
                _battleCon._focusMonster++;
            }
            
        }
        int selectPat;
        string pattern;
        int patternRun;

        public void Attack(Hero target)
        {
            if (_battleCon._crystalMon <= 0)
            {
                //Debug.Log("end 3");
                _battleCon._waitEndTurn = true;
                return;
            }
            _anim.SetTrigger("IsAttack");
            //_anim.SetBool("IsAttack", true);
            OnPassiveWorking();
            int useCrystal = Random.Range(1, 11);
            //Debug.Log("b crystal " + _battleCon._crystalMon+"use "+ useCrystal);
            _currentSkill = _status.attack[0];
            
            if (_status.hate >= 100)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    _status.hate = 0;
                    useCrystal = _status.attack[1].skill.crystal;
                    _currentSkill = _status.attack[1];
                }
            }
            int oldCrystal = _battleCon._crystalMon;
            _battleCon._crystalMon -= useCrystal;
            //Debug.Log("f crystal " + _battleCon._crystalMon);
            if (_buffCon._defenseList.ToList().Count >0 && _buffCon._defenseList[0].crystal == (useCrystal > oldCrystal? oldCrystal>3? oldCrystal/3 : oldCrystal : useCrystal) && _currentSkill.skill.effect != "")
            {
                //Debug.Log("defense crystal " + _buffCon._defenseList[0].crystal);
                _battleCon.RunCounterAttack();
                target.GetStatus().hate += (int)(_currentSkill.hate*1.5);
            }
            else
            {
                if (_currentSkill.skill.type == _Attack.PHYSICAL)
                {
                    //Debug.Log("mon damage " + ATK + " skill bonus " + skill.skill.bonusDmg + " dmg*bonus " + (ATK * skill.skill.bonusDmg)+" - def "+ target._status.DEF);
                    _currentDamage = (int)(_status.ATK * _currentSkill.skill.bonusDmg * _status._passiveBonusATK * _status._eventBonusDmg * _status._buffBonusATK - target.GetStatus().DEF * _status._buffBonusDEF * _status._passiveBonusDEF);
                }
                else
                {
                    //Debug.Log("mon damage " + ATK + " skill bonus " + skill.skill.bonusDmg + " dmg*bonus " + (ATK * skill.skill.bonusDmg) + " - mdef " + target._status.MDEF);
                    _currentDamage = (int)(_status.ATK * _currentSkill.skill.bonusDmg * _status._passiveBonusMATK * _status._eventBonusDmg * _status._buffBonusMATK - target.GetStatus().MDEF * _status._buffBonusMDEF * _status._passiveBonusMDEF);
                }
                
                if (data.type - target.GetData().type == -1 || data.type - target.GetData().type == 2)
                {
                    _currentDamage = (int)(_currentDamage * 1.25);
                }
                else if (data.type - target.GetData().type == 0)
                {
                    _currentDamage = _currentDamage;
                }
                else
                {
                    _currentDamage = (int)(_currentDamage * 0.75);
                }
                //Debug.Log("mon damage final " + _damage);
                target.GetStatus().currentHP -= _currentDamage;
                for (int i = 0; i < _currentSkill.buff.Count; i++)
                {
                    _currentSkill.buff[i].startTime = _battleCon._turnAround;
                    _currentSkill.buff[i].originalSize = _originalSize;
                    _buffCon.AddBuff(_currentSkill.buff[i]);
                }
                target.GetStatus().hate += _currentSkill.hate;
                _battleCon.AttackEffect(_currentSkill, _currentDamage, _Model.PLAYER);
            }

            _status.hate += _currentSkill.hate / 3;
            _buffCon.RemoveDefense(0);
            _attackCount++;
        }
        Sprite[] loadSprite;
        string monsSpriteSet = "";

        public void LoadSprite()
        {
            if (monsSpriteSet != data.spriteSet)
            {
                monsSpriteSet = data.spriteSet;
                if (data.spriteSet.Contains("monster"))
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Monster/" + monsSpriteSet);
                }
                else
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + monsSpriteSet);
                }
            }

            _avatar.GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == data.spriteName);
            _avatar.SetActive(true);
            _originalSize = _avatar.transform.parent.localScale;
            _originalHPMax = _status.hpMax;
            _status.hate = 0;
            OnPassiveWorking();
            _battleCon._battleState = _BattleState.Finish;
        }

        Vector3 endPosition;
        Vector3 oldPos;
        bool startInjur;
        bool injur;
        float speed = 4f;
        Material _originalMat;

        public void PlayInjury(int dmg)
        {
            
            injur = true;
            oldPos = _avatar.transform.position;
            endPosition = new Vector3(_avatar.transform.position.x - 0.8f, _avatar.transform.position.y, _avatar.transform.position.z);

            _originalMat = _avatar.GetComponent<SpriteRenderer>().material;
            _avatar.GetComponent<SpriteRenderer>().material = _battleCon._injuryMat;
            
            _anim.SetTrigger("IsInjury");
            startInjur = true;
            OnPassiveWorking();
            _battleCon.ShowDamage(dmg, _avatar.transform.position);
            if (_status.currentHP <= 0)
            {
                _anim.SetTrigger("IsDead");
                Dead();
            }
            else
            {

                if (_battleCon._counterATKSuccess)
                {
                    _battleCon._counterATKSuccess = false;
                    _battleCon._focusHero++;
                }
                _battleCon._focusMonster++;
            }
            
        }
        
        public bool CheckInjury()
        {
            if (_anim == null) return false;
            if (startInjur && _avatar.transform.position != endPosition)
            {
                _avatar.transform.position = Vector3.MoveTowards(_avatar.transform.position, endPosition, speed * Time.deltaTime);
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
                _avatar.GetComponent<SpriteRenderer>().material = _originalMat;
            }
            startInjur = false;
            return false;
        }

        public void OnPassiveWorking()
        {
            OnPassiveFunction(_status.passive.ToString(), _Model.MONSTER);
        }
        PassiveActive PassiveFunction;

        private void OnPassiveFunction(string methodName, params object[] parameter)
        {
            if (PassiveFunction == null)
                PassiveFunction = new PassiveActive();

            var method = PassiveFunction.GetType().GetMethod(methodName);
            if (method != null)
                method.Invoke(PassiveFunction, parameter);
        }
    }
}

