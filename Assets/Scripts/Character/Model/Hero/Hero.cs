
using controller;
using model;
using Model;
using skill;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;
using character;

namespace model
{
    [System.Serializable]
    public class Hero
    {
        GameCore _core;
        BattleController _battleCon;
        BuffController _buffCon;
        public AttackController _attackCon;

        int _storeId;
        ModelDataSet data;
        Status _status;
        double _exp;

        public GameObject obj;

        int slotId;
        GameObject _avatar;

        public GameObject _icon;

        Skill _currentSkill;
        int _attackCount;
        Animator _anim;
        Vector3 _originalSize;
        int Damage;
        

        public Hero(int storeId, int slot,double exp, ModelDataSet model)
        {
            _storeId = storeId;
            _core = Camera.main.GetComponent<GameCore>();
            _battleCon = _core._battleCon;
            _buffCon = _core._buffCon;
            _attackCon = _core._attackCon;
            _avatar = _core._heroAvatar;
            _anim = _avatar.GetComponent<Animator>();
            slotId = slot;
            _exp = exp;
            _status = new Status(_exp, model);
            data = model;
        }
        
        public int GetStoreId()
        {
            return _storeId;
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
        
        public double GetExp()
        {
            return _exp;
        }

        public void SetExp(double exp)
        {
            if (exp == 0) return;
            _exp = exp;
        }

        public int _currentDamage
        {
            get
            {
                return this.Damage;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.Damage = value;
            }
        }
        

        public void UpdateCurrentHPMax(int h)
        {
            _status.currentHPMax += h;
            _status.currentHP += h;
        }

        public void HPActive()
        {
            _icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.hpMax);
            
        }

        public void HateActive()
        {
            if (_status.hate == 100)
            {
                _status.hate = 0;
                _attackCon.LoadUltimate(slotId);
            }
            _icon.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_status.hate * 1 / 100);
        }
        
        public void Revive()
        {
            _icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 150);
            _anim.SetTrigger("IsBorn");
            
        }
        
        public void Dead()
        {
            _avatar.transform.parent.localScale = _originalSize;
            _status.hate = 0;
            _battleCon._hero.Remove(this);
            if (_battleCon._hero.ToList().Count == 0)
            {
                _battleCon._waitEndTurn = false;
                _battleCon.OnBattleEnd(false);
            }
            else
            {
                if(_battleCon._monster.ToList().Count == 0)
                {
                    _battleCon._waitEndTurn = false;
                }
                else
                {
                    Debug.Log("runAI 3");
                    _battleCon._focusHero++;
                    _battleCon.RunMonAI();
                }
            }
            _attackCon.UpdateAttackSlot();
        }

        public void AddDefenseList(int crystal)
        {
            _buffCon.AddDefense(crystal);
        }

        public void Attack(bool ultimate = false,bool questionFail=false,int stack=1)
        {
            _anim.SetTrigger("IsAttack");
            if (ultimate)
                _currentSkill = _status.attack[1];
            else
                _currentSkill = _status.attack[0];
            
            Monster target = _battleCon.FocusMonster();
            OnPassiveWorking();

            for (int i = 0; i < _currentSkill.buff.Count; i++)
            {
                _currentSkill.buff[i].startTime = _battleCon._turnAround;
                _currentSkill.buff[i].originalSize = _originalSize;
                _buffCon.AddBuff(_currentSkill.buff[i]);
            }
            
            if (_currentSkill.skill.type != _Attack.BUFF)
            {
                //Damage = Damage + 500;//for tester.

                if (_currentSkill.skill.type == _Attack.PHYSICAL)
                {
                    
                    _currentDamage = (int)(_status.ATK * _currentSkill.skill.bonusDmg * _status._passiveBonusATK * _status._eventBonusDmg * _status._buffBonusATK * stack - target.GetStatus().DEF * _status._buffBonusDEF * _status._passiveBonusDEF);
                }
                else
                {
                    
                    _currentDamage = (int)(_status.ATK * _currentSkill.skill.bonusDmg * _status._passiveBonusMATK * _status._eventBonusDmg * _status._buffBonusMATK * stack - target.GetStatus().MDEF * _status._buffBonusMDEF * _status._passiveBonusMDEF);
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
                int hpOld = target.GetStatus().currentHP;
                if (questionFail)
                    _currentDamage = _currentDamage / 2;
                //Debug.Log("hero damage final " + _damage);
                target.GetStatus().currentHP -= _currentDamage;
                _battleCon._damage_of_each_hero[_battleCon.FocusMonster().GetSlot(), slotId] += hpOld - target.GetStatus().currentHP;
            }
            else
            {
                _currentDamage = _currentDamage * 0;
            }
            
            target.GetStatus().hate += _currentSkill.hate;
            _status.hate += (_currentSkill.hate / 3)*stack;
            
            _battleCon.AttackEffect(_currentSkill, _currentDamage, _Model.MONSTER);
            
        }
        

        public int CalDamageCounterAttack(Monster target)
        {
            int damage = (int)(_status.ATK* _status._eventBonusDmg * _status._passiveBonusATK * _status._buffBonusATK);
            int hpOld = target.GetStatus().currentHP;
            target.GetStatus().currentHP -= damage;
            _battleCon._damage_of_each_hero[_battleCon.FocusMonster().GetSlot(), slotId] += hpOld - target.GetStatus().currentHP;
            return damage;
        }

        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void LoadSprite()
        {
            if (getSpriteSet != data.spriteSet)
            {
                getSpriteSet = data.spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            Debug.Log("test "+ getSpriteSet);
            _avatar.GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == data.spriteName);
            _avatar.SetActive(true);
            //_anim.SetTrigger("IsBorn");
            _originalSize = _avatar.transform.parent.localScale;
            _status.currentHPMax = _status.hpMax;
            _status.hate = 0;
            _status.currentHP = _status.hpMax;
            _icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)_status.currentHP * 1 / _status.hpMax);
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
            endPosition = new Vector3(_avatar.transform.position.x + 0.8f, _avatar.transform.position.y, _avatar.transform.position.z);
            
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
                //Debug.Log("runAI 2");
                if (!_battleCon._counterATKSuccess)
                {
                    _battleCon._focusHero++;
                }
                _battleCon.RunMonAI();
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
            else if (injur)
            {
                injur = false;
                endPosition = oldPos;
                return true;
            }
            if (startInjur)
            {
                Debug.Log("check injury");
                _avatar.GetComponent<SpriteRenderer>().material = _originalMat;
            }
            startInjur = false;
            return false;
        }

        public void OnPassiveWorking()
        {
            OnPassiveFunction(_status.passive.ToString(), _Model.PLAYER);
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

