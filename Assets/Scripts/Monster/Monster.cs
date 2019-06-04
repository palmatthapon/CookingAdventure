
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using player;
using model;
using controller;
using skill;

namespace monster
{
    [System.Serializable]
    public class Monster
    {
        public Monster(int index)
        {
            _core = Camera.main.GetComponent<GameCore>();
            _monCom = _core._monCom;
            _buffCon = _core._buffCon;
            _avatar = _monCom._monAvatarList[index];
            _anim = _avatar.GetComponent<Animator>();
            _battleCon = _core._battleCon;
            slotId = index;
        }

        public GameObject _avatar;
        public GameObject _icon;
        public int slotId;
        public string name;
        public string spriteSet;
        public string spriteName;
        public _Character type;
        public int level;
        public double expDrop;
        public double STR;
        public double AGI;
        public double INT;
        public int hpMax;
        private int HP;
        public int ATK;
        public int MATK;
        public int DEF;
        public int MDEF;
        public Skill[] attack = new Skill[2];
        public _Passive passive;
        public int _originalHPMax;
        public string[] attackPattern;
        
        GameCore _core;
        BattleController _battleCon;
        BuffController _buffCon;
        MonPanel _monCom;
        public int attackCount =0;
        int _hate;
        public Skill skill;
        public Animator _anim;
        public float _eventBonusDmg = 1;
        public float _passiveBonusATK = 1;
        public float _passiveBonusMATK = 1;
        public float _passiveBonusDEF = 1;
        public float _passiveBonusMDEF = 1;
        public float _buffBonusATK = 1;
        public float _buffBonusMATK = 1;
        public float _buffBonusDEF = 1;
        public float _buffBonusMDEF = 1;
        public float _buffBonusHP = 1;
        public Vector3 _originalSize;
        public float CriRate = 0.2f;
        int damage;

        public int _damage
        {
            get
            {
                return this.damage;
            }
            set
            {
                if(value < 0)
                {
                    value = 0;
                }
                this.damage = value;
            }
        }

        public int hp
        {
            get
            {
                return this.HP;
            }
            set
            {
                if (value < 0)
                    this.HP = 0;
                else if (value > hpMax)
                    this.HP = hpMax;
                else
                    this.HP = value;
                _battleCon.UpdateMonsterHP();
            }
        }

        public void UpdatehpMax(int h)
        {
            hpMax += h;
            hp += h;
        }
        
        public int Hate
        {
            get
            {
                return this._hate;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 100)
                {
                    value = 100;
                }
                this._hate = value;
                _icon.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_hate * 1 / 100);
            }
        }
        
        public void Dead()
        {
            _avatar.transform.parent.localScale = _originalSize;
            hpMax = _originalHPMax;
            _icon.transform.localScale = new Vector3(1, 1, 1);
            Hate = 0;
            _icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(46, 46, 46, 150);
            _battleCon._monster.Remove(this);
            if (_battleCon._monster.ToList().Count == 0)
            {
                _battleCon._waitEndTurn = false;
                _battleCon.OnBattleEnd(true);
            }
            else
            {
                _battleCon._monFocus++;
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
            skill = attack[0];
            
            if (_hate >= 100)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    Hate = 0;
                    useCrystal = attack[1].skill.crystal;
                    skill = attack[1];
                }
            }
            int oldCrystal = _battleCon._crystalMon;
            _battleCon._crystalMon -= useCrystal;
            //Debug.Log("f crystal " + _battleCon._crystalMon);
            if (_buffCon._defenseList.ToList().Count >0 && _buffCon._defenseList[0].crystal == (useCrystal > oldCrystal? oldCrystal>3? oldCrystal/3 : oldCrystal : useCrystal) && skill.skill.effect != "")
            {
                //Debug.Log("defense crystal " + _buffCon._defenseList[0].crystal);
                _battleCon.RunCounterAttack();
                target.Hate += (int)(skill.hate*1.5);
            }
            else
            {
                if (skill.skill.type == _Attack.PHYSICAL)
                {
                    //Debug.Log("mon damage " + ATK + " skill bonus " + skill.skill.bonusDmg + " dmg*bonus " + (ATK * skill.skill.bonusDmg)+" - def "+ target.hero.DEF);
                    _damage = (int)(ATK * skill.skill.bonusDmg * _passiveBonusATK * _eventBonusDmg * _buffBonusATK - target.hero.DEF * _buffBonusDEF * _passiveBonusDEF);
                }
                else
                {
                    //Debug.Log("mon damage " + ATK + " skill bonus " + skill.skill.bonusDmg + " dmg*bonus " + (ATK * skill.skill.bonusDmg) + " - mdef " + target.hero.MDEF);
                    _damage = (int)(ATK * skill.skill.bonusDmg * _passiveBonusMATK * _eventBonusDmg * _buffBonusMATK - target.hero.MDEF * _buffBonusMDEF * _passiveBonusMDEF);
                }
                
                if (type - target.hero.hero.type == -1 || type - target.hero.hero.type == 2)
                {
                    _damage = (int)(_damage * 1.25);
                }
                else if (type - target.hero.hero.type == 0)
                {
                    _damage = _damage;
                }
                else
                {
                    _damage = (int)(_damage * 0.75);
                }
                //Debug.Log("mon damage final " + _damage);
                target.hp -= _damage;
                for (int i = 0; i < skill.buff.Count; i++)
                {
                    skill.buff[i].startTime = _battleCon._turnAround;
                    skill.buff[i].originalSize = _originalSize;
                    _buffCon.AddBuff(skill.buff[i]);
                }
                target.Hate += skill.hate;
                _battleCon.AttackEffect(skill, _damage, _Model.PLAYER);
            }
            
            Hate += skill.hate / 3;
            _buffCon.RemoveDefense(0);
            attackCount++;
        }
        Sprite[] loadSprite;
        string monsSpriteSet = "";

        public void LoadSprite()
        {
            if (monsSpriteSet != spriteSet)
            {
                monsSpriteSet = spriteSet;
                if (spriteSet.Contains("monster"))
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Monster/" + monsSpriteSet);
                }
                else
                {
                    loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + monsSpriteSet);
                }
            }

            _avatar.GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == spriteName);
            _avatar.SetActive(true);
            _originalSize = _avatar.transform.parent.localScale;
            _originalHPMax = hpMax;
            Hate = 0;
            OnPassiveWorking();
            _battleCon._battleState = _BattleState.Finish;
        }
        Sprite[] loadSpriteMonsterInjurious = null;
        string getMonsterSpriteSet = "";
        Vector3 endPosition;
        Vector3 oldPos;
        bool startInjur;
        bool injur;
        float speed = 4f;

        public void PlayInjurious(int dmg)
        {
            if (getMonsterSpriteSet != spriteSet)
            {
                getMonsterSpriteSet = spriteSet + "_Injurious";
                if (spriteSet.Contains("monster"))
                {
                    loadSpriteMonsterInjurious = Resources.LoadAll<Sprite>("Sprites/Character/Monster/" + getMonsterSpriteSet);
                }
                else
                {
                    loadSpriteMonsterInjurious = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getMonsterSpriteSet);
                }
            }
            injur = true;
            oldPos = _avatar.transform.position;
            endPosition = new Vector3(_avatar.transform.position.x - 0.8f, _avatar.transform.position.y, _avatar.transform.position.z);

            _avatar.GetComponent<SpriteRenderer>().sprite = loadSpriteMonsterInjurious.Single(s => s.name == spriteName + "_Injurious");
            
            _anim.SetTrigger("IsInjurious");
            startInjur = true;
            OnPassiveWorking();
            _battleCon.ShowDamage(dmg, _avatar.transform.position);
            if (hp <= 0)
            {
                _anim.SetTrigger("IsDead");
                Dead();
            }
            else
            {

                if (_battleCon._counterATKSuccess)
                {
                    _battleCon._counterATKSuccess = false;
                    _battleCon._heroFocus++;
                }
                _battleCon._monFocus++;
            }
            
        }
        
        public bool CheckInjurious()
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
            startInjur = false;
            _avatar.GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == spriteName);
            return false;
        }

        public void OnPassiveWorking()
        {
            OnPassiveFunction(passive.ToString(), _Model.MONSTER);
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

