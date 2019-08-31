
namespace system
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
    
    public enum _Attack
    {
        PHYSICAL = 1,
        MAGICAL
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

    public enum _Passive
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

    public enum _Item
    {
        SmallPosion = 1,
        MediumPosion,
        LargePosion,
        ReCrystal,
        Antidote
    }

    public enum _GameState
    {
        GAMEMENU,
        START,
        BATTLE,
        MAP,
        CAMP,
        LAND,
        SECRETSHOP
    }
    public enum _ActionState
    {
        Attack,
        Defense,
        Item,
        Rawmaterial,
        Shop,
        Team,
        Map,
        Cooking,
        Farming
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

    public enum _Event
    {
        Wind,
        Rain,
        Sunshine
    }
}
