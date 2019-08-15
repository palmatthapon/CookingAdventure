using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using skill;
using model;

namespace character
{
    [System.Serializable]
    public class Status
    {
        Calculate _cal;

        public GameObject _avatar;
        public string name;
        public int level;
        public int STR;
        public int AGI;
        public int INT;
        public int hpMax;
        public int ATK;
        public int MATK;
        public int DEF;
        public int MDEF;
        public Skill[] attack = new Skill[2];
        public _Passive passive;

        public int currentHPMax;
        int CurrentHP;

        int Hate;
        
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
            _cal = new Calculate();
            level = _cal.CalculateLevel(exp);

            STR = _cal.CalculateSTR(model.baseSTR, model.baseAGI, model.baseINT, level);
            AGI = _cal.CalculateAGI(model.baseSTR, model.baseAGI, model.baseINT, level);
            INT = _cal.CalculateINT(model.baseSTR, model.baseAGI, model.baseINT, level);

            ATK = _cal.CalculateATK(STR,AGI,INT);
            MATK = _cal.CalculateMATK(STR, AGI, INT);
            DEF = _cal.CalculateDEF(STR, AGI, INT);
            MDEF = _cal.CalculateMDEF(STR, AGI, INT);

            hpMax = _cal.CalculateHpMax(STR, AGI, INT);
            currentHP = hpMax;
        }

        public Status(int level, ModelDataSet model)
        {
            _cal = new Calculate();

            STR = _cal.CalculateSTR(model.baseSTR, model.baseAGI, model.baseINT, level);
            AGI = _cal.CalculateAGI(model.baseSTR, model.baseAGI, model.baseINT, level);
            INT = _cal.CalculateINT(model.baseSTR, model.baseAGI, model.baseINT, level);

            ATK = _cal.CalculateATK(STR, AGI, INT);
            MATK = _cal.CalculateMATK(STR, AGI, INT);
            DEF = _cal.CalculateDEF(STR, AGI, INT);
            MDEF = _cal.CalculateMDEF(STR, AGI, INT);

            hpMax = _cal.CalculateHpMax(STR, AGI, INT);
            currentHP = hpMax;
        }

        public int currentHP
        {
            get
            {
                return this.CurrentHP;
            }
            set
            {
                if (value < 0)
                    this.CurrentHP = 0;
                else if (value > currentHPMax)
                    this.CurrentHP = currentHPMax;
                else
                    this.CurrentHP = value;
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

    }
}
