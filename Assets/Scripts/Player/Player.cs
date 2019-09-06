using UnityEngine;
using System.Collections;
using model;
using UnityEngine.UI;

namespace player
{
    [System.Serializable]
    public class Player
    {

        string Name;
        int Money;
        int Soul;

        public int currentDungeonFloor;
        public int currentStayDunBlock;
        public Hero _heroIsPlaying;

        public string name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        public int currentSoul
        {
            get
            {
                return this.Soul;
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
                this.Soul = value;
                getCore()._playerSoulBar.GetComponent<PlayerSoul>().AddFill(this.Soul);
                getCore()._playerSoulBar.transform.Find("SoulText").GetComponent<Text>().text = this.Soul + "/100";
            }
        }

        GameCore getCore()
        {
            return Camera.main.GetComponent<GameCore>();
        }

        public int currentMoney
        {
            get
            {
                return this.Money;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.Money = value;
            }
        }
    }
}

