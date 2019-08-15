using controller;
using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace character
{
    [System.Serializable]
    public class Character
    {
        Hero _hero = null;
        Monster _monster = null;
        
        public Character(Hero hero)
        {
            _hero = hero;
        }
        public Character(Monster monster)
        {
            _monster = monster;
        }

        public Status GetStatus()
        {
            if (_hero != null)
                return _hero.GetStatus();
            else
                return _monster.GetStatus();
        }

    }
}

