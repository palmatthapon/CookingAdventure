using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace CollectionData
{
    [System.Serializable]
    public class Hero
    {
        MainCore _core;
        BattleController _battleCon;
        BuffController _buffCon;
        HeroPanel _heroCom;
        SelectAttackController _selectATKCon;

        public Hero(int index)
        {
            _core = Camera.main.GetComponent<MainCore>();
            _battleCon = _core._battleCon;
            _heroCom = _core._heroCom;
            _buffCon = _core._buffCon;
            _selectATKCon = _core._selectATKCon;
            _avatar = _heroCom._heroAvatar;
            _anim = _avatar.GetComponent<Animator>();
            slotId = index;
        }
        public int slotId;
        public GameObject _avatar;
        public int _originalHPMax;
        public HeroStore hero;
        public GameObject _icon;
        public Skill skill;
        
        public int _attackCount;
        int _hate;
        public Animator _anim;
        public float _eventBonusDmg=1;
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
                if (value < 0)
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
                return this.hero.hp;
            }
            set
            {
                this.hero.hp = value;
                _icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.hp * 1 / hero.hpMax);
            }
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
                }else if(value > 100)
                {
                    value = 100;
                }
                this._hate = value;
                if(Hate == 100)
                {
                    Hate = 0;
                    _selectATKCon.LoadUltimate(slotId);
                }
                _icon.transform.Find("UltimateSlider").GetComponent<ControlSlider>().AddFill((float)_hate * 1 / 100);
            }
        }

        public void UpdatehpMax(int h)
        {
            hero.hpMax += h;
            hero.hp += h;
        }

        public void Revive()
        {
            _icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 150);
            _anim.SetTrigger("IsBorn");
            
        }
        
        public void Dead()
        {
            _avatar.transform.parent.localScale = _originalSize;
            hero.hpMax = _originalHPMax;
            Hate = 0;
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
                    _battleCon._heroFocus++;
                    _battleCon.RunMonAI();
                }
            }
            _selectATKCon.UpdateAttackSlot();
        }

        public void AddDefenseList(int crystal)
        {
            _buffCon.AddDefense(crystal);
        }

        public void Attack(bool ultimate = false,bool questionFail=false,int stack=1)
        {
            _anim.SetTrigger("IsAttack");
            if (ultimate)
                skill = hero.attack[1];
            else
                skill = hero.attack[0];
            
            Monster target = _battleCon.GetMonFocus();
            OnPassiveWorking();

            for (int i = 0; i < skill.buff.Count; i++)
            {
                skill.buff[i].startTime = _battleCon._turnAround;
                skill.buff[i].originalSize = _originalSize;
                _buffCon.AddBuff(skill.buff[i]);
            }
            
            if (skill.skill.type != _Attack.BUFF)
            {
                //Damage = Damage + 500;//for tester.

                if (skill.skill.type == _Attack.PHYSICAL)
                {
                    //Debug.Log("hero damage " + hero.ATK + " skill bonus " + skill.skill.bonusDmg + " dmg*bonus " + (hero.ATK * skill.skill.bonusDmg) + " - def " + target.DEF);
                    _damage = (int)(hero.ATK * skill.skill.bonusDmg * _passiveBonusATK * _eventBonusDmg * _buffBonusATK * stack - target.DEF * _buffBonusDEF * _passiveBonusDEF);
                }
                else
                {
                    //Debug.Log("hero damage " + hero.ATK + " skill bonus " + skill.skill.bonusDmg + " dmg*bonus " + (hero.ATK * skill.skill.bonusDmg) + " - mdef " + target.MDEF);
                    _damage = (int)(hero.ATK * skill.skill.bonusDmg * _passiveBonusMATK * _eventBonusDmg * _buffBonusMATK * stack - target.MDEF * _buffBonusMDEF * _passiveBonusMDEF);
                }

                if (hero.hero.type - target.type == -1 || hero.hero.type - target.type == 2)
                {
                    _damage = (int)(_damage * 1.25);
                }
                else if (hero.hero.type - target.type == 0)
                {
                    _damage = _damage;
                }
                else
                {
                    _damage = (int)(_damage * 0.75);
                }
                int hpOld = target.hp;
                if (questionFail)
                    _damage = _damage / 2;
                //Debug.Log("hero damage final " + _damage);
                target.hp -= _damage;
                _battleCon._damage_of_each_hero[_battleCon.GetMonFocus().slotId, slotId] += hpOld - target.hp;
            }
            else
            {
                _damage = _damage * 0;
            }
            
            target.Hate += skill.hate;
            Hate += (skill.hate / 3)*stack;
            
            _battleCon.AttackEffect(skill, _damage, _Model.MONSTER);
            
        }
        

        public int CalDamageCounterAttack(Monster target)
        {
            int damage = (int)(hero.ATK* _eventBonusDmg * _passiveBonusATK * _buffBonusATK);
            int hpOld = target.hp;
            target.hp -= damage;
            _battleCon._damage_of_each_hero[_battleCon.GetMonFocus().slotId, slotId] += hpOld - target.hp;
            return damage;
        }

        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void LoadSprite()
        {
            if (getSpriteSet != hero.hero.spriteSet)
            {
                getSpriteSet = hero.hero.spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            _avatar.GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == hero.hero.spriteName);
            _avatar.SetActive(true);
            //_anim.SetTrigger("IsBorn");
            _originalSize = _avatar.transform.parent.localScale;
            _originalHPMax = hero.hpMax;
            Hate = 0;
            hp = hero.hpMax;
            _icon.transform.Find("HPSlider").GetComponent<ControlSlider>().AddFill((float)hero.hp * 1 / hero.hpMax);
            OnPassiveWorking();
            _battleCon._battleState = _BattleState.Finish;
        }
        Sprite[] loadSpriteHeroInjurious = null;
        string getHeroSpriteSet = "";
        Vector3 endPosition;
        Vector3 oldPos;
        bool startInjur;
        bool injur;
        float speed = 4f;

        public void PlayInjurious(int dmg)
        {
            if (getHeroSpriteSet != hero.hero.spriteSet)
            {
                getHeroSpriteSet = hero.hero.spriteSet + "_Injurious";
                loadSpriteHeroInjurious = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getHeroSpriteSet);
            }
            injur = true;
            oldPos = _avatar.transform.position;
            endPosition = new Vector3(_avatar.transform.position.x + 0.8f, _avatar.transform.position.y, _avatar.transform.position.z);

            _avatar.GetComponent<SpriteRenderer>().sprite = loadSpriteHeroInjurious.Single(s => s.name == hero.hero.spriteName + "_Injurious");

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
                //Debug.Log("runAI 2");
                if (!_battleCon._counterATKSuccess)
                {
                    _battleCon._heroFocus++;
                }
                _battleCon.RunMonAI();
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
            else if (injur)
            {
                injur = false;
                endPosition = oldPos;
                return true;
            }
            startInjur = false;
            _avatar.GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == hero.hero.spriteName);
            return false;
        }

        public void OnPassiveWorking()
        {
            OnPassiveFunction(hero.passive.ToString(), _Model.PLAYER);
        }
        PassiveAbility PassiveFunction;

        private void OnPassiveFunction(string methodName, params object[] parameter)
        {
            if (PassiveFunction == null)
                PassiveFunction = new PassiveAbility();

            var method = PassiveFunction.GetType().GetMethod(methodName);
            if (method != null)
                method.Invoke(PassiveFunction, parameter);
        }
    }
}

