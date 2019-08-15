
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace model
{
    public class Calculate
    {
        int _damageBase = 8;
        /// <summary>
        /// For Hero calaulate
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public int CalculateSTR(int baseSTR,int baseAGI,int baseINT, int level)
        {
            if (baseSTR > baseAGI)
                if (baseSTR > baseINT)
                    return (int)(baseSTR + 1.5 * level);

            return baseSTR + level;
        }
        public int CalculateAGI(int baseSTR, int baseAGI, int baseINT, int level)
        {
            if (baseAGI > baseSTR)
                if (baseAGI > baseINT)
                    return (int)(baseAGI + 1.5 * level);
            return baseAGI + level;
        }
        public int CalculateINT(int baseSTR, int baseAGI, int baseINT, int level)
        {
            if (baseINT > baseSTR)
                if (baseINT > baseAGI)
                    return (int)(baseINT + 1.5 * level);
            return baseINT + level;
        }
        public int CalculateHpMax(int STR, int AGI, int INT)
        {
            return (int)(STR * 5 + AGI * 2.5 + INT * 1.5 + 50);
        }
        public int CalculateLevel(double exp)
        {
            return (int)((Math.Sqrt(100 * ((2 * exp) + 25)) + 50) / 100);
        }
        public double CalculateExp(int level)
        {
            return (Math.Pow(level, 2) + level) / 2 * 100 - (level * 100);
        }
        public int CalculateATK(int STR,int AGI,int INT)
        {
            return (int)(STR * 1.5 + AGI / 5 + INT / 10 + _damageBase);
        }
        public int CalculateMATK(int STR, int AGI, int INT)
        {
            return (int)(STR / 10 + AGI / 5 + INT * 1.5 + _damageBase/2);
        }
        public int CalculateDEF(int STR, int AGI, int INT)
        {
            return (int)(STR / 5 + AGI * 0.5 + INT / 5);
        }
        public int CalculateMDEF(int STR, int AGI, int INT)
        {
            return (int)(STR / 10 + AGI / 5 + INT * 0.5);
        }

        /// <summary>
        /// For Monster calculate
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public int[] CalculateAllStatus(int baseSTR, int baseAGI, int baseINT, int level)
        {
            int[] status = new int[3];
            status[0] = baseSTR + level;
            if (baseSTR > baseAGI)
                if (baseSTR > baseINT)
                    status[0]=(int)(baseSTR + 1.5 * level);
            status[1] = baseAGI + level;
            if (baseAGI > baseSTR)
                if (baseAGI > baseINT)
                    status[1]=(int)(baseAGI + 1.5 * level);
            status[2] = baseINT + level;
            if (baseINT > baseSTR)
                if (baseINT > baseAGI)
                    status[2] = (int)(baseINT + 1.5 * level);
            return status;
        }
        
        /// <summary>
        /// For all project
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IntParseFast(string value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char letter = value[i];
                result = 10 * result + (letter - 48);
            }
            return result;
        }

    }
}
