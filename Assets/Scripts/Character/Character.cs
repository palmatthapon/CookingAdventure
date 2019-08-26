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

        _Model _type;
        
        public Character(Hero hero)
        {
            _type = _Model.PLAYER;
            _hero = hero;
        }
        public Character(Monster monster)
        {
            _type = _Model.MONSTER;
            _monster = monster;
        }

        public Status getStatus()
        {
            if (_type == _Model.PLAYER)
                return _hero.getStatus();
            else
                return _monster.getStatus();
        }

    }
}

