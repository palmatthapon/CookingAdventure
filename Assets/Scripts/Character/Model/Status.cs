﻿using system;

namespace model
{
    [System.Serializable]
    public class Status
    {
        Calculator _cal;

        int level;
        int STR;
        int AGI;
        int INT;
        int hpMax;
        int ATK;
        int MATK;
        int DEF;
        int MDEF;
        public Skill[] attack = new Skill[2];
        public _Passive passive;

        Skill _currentSkill;

        public int currentHPMax;
        int HP;

        double Exp;

        int Hate;
        int _damageReceived;

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
        
        public Status(double exp, ModelDataSet model)
        {
            Exp = exp;

            _cal = new Calculator();

            level = _cal.CalculateLevel(exp);
            STR = _cal.CalculateSTR(model.baseSTR, model.baseAGI, model.baseINT, level);
            AGI = _cal.CalculateAGI(model.baseSTR, model.baseAGI, model.baseINT, level);
            INT = _cal.CalculateINT(model.baseSTR, model.baseAGI, model.baseINT, level);

            ATK = _cal.CalculateATK(STR,AGI,INT);
            MATK = _cal.CalculateMATK(STR, AGI, INT);
            DEF = _cal.CalculateDEF(STR, AGI, INT);
            MDEF = _cal.CalculateMDEF(STR, AGI, INT);

            hpMax = _cal.CalculateHpMax(STR, AGI, INT);
            currentHPMax = hpMax;
            currentHP = hpMax;
        }

        public Status(int level, ModelDataSet model)
        {
            _cal=new Calculator();

            STR = _cal.CalculateSTR(model.baseSTR, model.baseAGI, model.baseINT, level);
            AGI = _cal.CalculateAGI(model.baseSTR, model.baseAGI, model.baseINT, level);
            INT = _cal.CalculateINT(model.baseSTR, model.baseAGI, model.baseINT, level);

            ATK = _cal.CalculateATK(STR, AGI, INT);
            MATK = _cal.CalculateMATK(STR, AGI, INT);
            DEF = _cal.CalculateDEF(STR, AGI, INT);
            MDEF = _cal.CalculateMDEF(STR, AGI, INT);

            hpMax = _cal.CalculateHpMax(STR, AGI, INT);
            currentHPMax = hpMax;
            currentHP = hpMax;
        }

        public int getATK()
        {
            return ATK;
        }

        public int getMATK()
        {
            return MATK;
        }

        public int getLvl()
        {
            return level;
        }

        public void setLvl(double exp)
        {
            level = _cal.CalculateLevel(exp);
        }

        public int currentHP
        {
            get
            {
                return this.HP;
            }
            set
            {
                if (value < 0)
                    this.HP = 0;
                else if (value > currentHPMax)
                    this.HP = currentHPMax;
                else
                    this.HP = value;
            }
        }

        public int hate
        {
            get
            {
                return this.Hate;
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
                this.Hate = value;

            }
        }

        public void SetDefaultPassive()
        {
            _passiveBonusATK = 1;
            _passiveBonusMATK = 1;
            _passiveBonusDEF = 1;
            _passiveBonusMDEF = 1;
        }

        public void SetDefaultBuff()
        {
            _buffBonusATK = 1;
            _buffBonusMATK = 1;
            _buffBonusDEF = 1;
            _buffBonusMDEF = 1;
            _buffBonusHP = 1;
        }

        public double getExp()
        {
            return Exp;
        }

        public void setExp(double exp)
        {
            if (exp == 0) return;
            
            Exp = Exp + exp;
            setLvl(Exp);
        }

        public int getDEF()
        {
            return DEF;
        }

        public int getMDEF()
        {
            return MDEF;
        }

        public int CalDmgSkill(_Attack type,double skillBonus,Status target,int skillStack)
        {
            if (type == _Attack.PHYSICAL)
            {

                return((int)(ATK * skillBonus * _passiveBonusATK * _eventBonusDmg *_buffBonusATK * skillStack - target.getDEF() * _buffBonusDEF * _passiveBonusDEF));
            }
            else
            {

                return ((int)(ATK * skillBonus * _passiveBonusMATK * _eventBonusDmg * _buffBonusMATK * skillStack - target.getMDEF() * _buffBonusMDEF * _passiveBonusMDEF));
            }
        }

        public double getExpDrop()
        {
            return 50.0 + (STR - AGI);
        }

        public int CalDmgCounterAttack(int slot,Status target,int targetSlot)
        {
            int damage = (int)(ATK * _eventBonusDmg * _passiveBonusATK * _buffBonusATK);
            int hpBefore = target.currentHP;
            target.currentHP -= damage;
            return damage;
        }

        public void ResetHP()
        {
            currentHPMax = hpMax;
            currentHP = hpMax;
        }

        public Skill getCurrentSkill()
        {
            return _currentSkill;
        }

        public void setCurrentSkill(Skill skill)
        {
            _currentSkill = skill;
        }

        public int getDamageReceived()
        {
            return _damageReceived;
        }

        public void setDamageReceived(int dmg)
        {
            if (dmg < 0)
            {
                dmg = 0;
            }
            _damageReceived = dmg;
        }
    }
}
