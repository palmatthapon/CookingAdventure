﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectionData
{
    static class Constants
    {
        public const int _crystalAttack = 3, _crystalDefense = 3, _crystalItem = 2, _crystalTeam = 4, _crystalBase = 4;
    }

    public enum _Model
    {
        PLAYER,
        MONSTER,
        NPC
    }
    
    public enum _Character
    {
        HAMMER = 1,
        SCISSORS,
        PAPER
    }

    public enum _Attack
    {
        PHYSICAL = 1,
        MAGICAL,
        BUFF
    }
    
    public enum _Buff
    {
        Buff1 = 1,
        Buff2,
        Buff3,
        Buff4,
        Buff5,
        Buff6,
        Buff7,
        Buff8
    }

    public enum _passive
    {
        Passive1 = 1,
        Passive2,
        Passive3,
        Passive4,
        Passive5,
        Passive6,
        Passive7,
        Passive8,
        Passive9
    }

    public enum _item
    {
        SmallPosion = 1,
        MediumPosion,
        LargePosion,
        ReCrystal,
        Antidote,
        Revive
    }

    public enum _GameStatus
    {
        GAMEMENU,
        START,
        BATTLE,
        MAP,
        CAMP,
        LAND,
        FORESTSHOP
    }
    public enum _ActionStatus
    {
        Attack,
        Defense,
        Item,
        Team,
        Map
    }

    public enum _SubMenu
    {
        ManageTeam,
        Item,
        Alert,
        Shop,
        Warp,
        GameMenu,
        Defense,
        BattleEnd,
        ManageHero,
        LoadBattleRevive,
        GameOver
    }

    public enum _ConfirmNotify
    {
        NewGame,
        ExitGame,
        SaveAndExit
    }

    public enum _RoundBattle
    {
        PLAYER,
        ENEMY

    }

    public enum _BattleState
    {
        Start,
        Wait,
        Finish
    }
    
    public enum Sorting
    {
        None,
        Level,
        HP,
        Type
    }

    public enum _event
    {
        Wind,
        Rain,
        Sunshine
    }
}
