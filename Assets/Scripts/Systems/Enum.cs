
namespace system
{

    public enum MODEL
    {
        PLAYER,
        MONSTER,
        NPC
    }
    
    public enum ATTACK
    {
        PHYSICAL = 1,
        MAGICAL
    }
    
    public enum PASSIVE
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

    public enum ITEMS
    {
        SmallPosion = 1,
        MediumPosion,
        LargePosion,
        ReCrystal,
        Antidote
    }

    public enum GAMESTATE
    {
        GAMEMENU,
        START,
        BATTLE,
        MAP,
        CAMP,
        LAND,
        SECRETSHOP
    }
    public enum ACTIONSTATE
    {
        Attack,
        Item,
        Rawmaterial,
        Shop,
        Team,
        Map,
        Cooking,
        Farming
    }
    
    public enum CONFIRMNOTIFY
    {
        NewGame,
        ExitGame,
        SaveAndExit
    }

    public enum BATTLEROUND
    {
        PLAYER,
        ENEMY

    }

    public enum BATTLESTATE
    {
        Start,
        Wait,
        Finish
    }
    
    public enum EVENT
    {
        Wind,
        Rain,
        Sunshine
    }
}
