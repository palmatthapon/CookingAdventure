using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Json;
using System.Linq;
using Random = UnityEngine.Random;
using monster;
using model;
using warp;
using controller;

public class GameCore : MonoBehaviour
    {

        public BattleController _battleCon;
        public MapController _mapCon;
        public BuffController _buffCon;
        public Calculate _cal;
        public MonPanel _monCom;
        public SelectAttackController _selectATKCon;


        JsonReadWrite _json;

        public GoSheets googleSheet;
        ///------your google sheet tsv link------------
        //string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTGnUodHOAsOpl-Bb8HYzq3gHA0PYNHj5Cr3sRUrZKzhc75XGCfSAoPiJu5CAY8mtTqI_V267aBu0v4/pub?gid=0&single=true&output=tsv";
        //string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTGnUodHOAsOpl-Bb8HYzq3gHA0PYNHj5Cr3sRUrZKzhc75XGCfSAoPiJu5CAY8mtTqI_V267aBu0v4/pub?gid=130495216&single=true&output=tsv";


        ///------------All DataSet-----------------
        public DungeonDataSet[] dataDungeonList;
        public MonsterDataSet[] dataMonsterList;
        public HeroDataSet[] dataHeroList;
        public SkillDataSet[] dataSkillList;
        public PlayerLog[] dataPlayerLog;
        public ItemDataSet[] dataItemList;
        public Setting[] dataSetting;
        ///-----end dataset zone--------

        /// <summary>
        /// Import Object By Scene...
        /// </summary>
        public GameObject _mapObj;
        public GameObject _battleObj;
        public GameObject _campObj;
        public GameObject _landObj;
        public GameObject _forestShopObj;

        ///---Panel Zone-----
        public GameObject _gameMenu, _miniGameMenu;
        public GameObject _settingPanel;
        public GameObject _mainMenu;
        public GameObject _attackPanel, _questionPanel;
        public Text _questionPanelTxt;
        public GameObject _monPanel;
        public GameObject _eventPanel;
        public GameObject _mapPanel;
        public GameObject _CharacterPanel;
        public GameObject _itemPanel;
        public GameObject _changeTeamPanel;
        public GameObject _manageHeroPanel;
        public GameObject _subMenuPanel;
        public GameObject _playerLifePanel;
        public GameObject _rewardPanel;
        public GameObject _shopPanel;
        public GameObject _talkPanel;
        public GameObject _infoPanel;
        public GameObject _monTalkPanel;
        public GameObject _gatePanel;
        public GameObject _buffPanel;
        public GameObject _confirmNotify;
        public GameObject _cutscenePanel;
        public GameObject _tutorialPanel;
        public GameObject _heroAvatar;
        public GameObject _cookMenu;
        public GameObject _farmMenu;
        public GameObject _forestShopPanel;

    /// -----End Panel Zone

    ///-----general----
    public GameObject _loadingScreen;
        public GameObject _loadingNotify;
        public GameObject[] _campHeroSprite;
        public Sprite[] _typeSprite;
        public GameObject _playerSoulBar;
        ///------ End import object by scene-----------

        public Dungeon[] _dungeon;
        public List<ItemStore> _itemStore;
        public List<HeroStore> _heroStore;
        public List<HeroInTeam> _teamSetup;
        public List<ItemShop> _landShopList;

        public string _questionUrl;
        public string[,] _questionList;
        public int _currentTeamIsSelect;
        public int _currentRoomPosition;
        public int _currentDungeonLayer;
        public Monster[] _currentMonsterBattle;
        int money;
        int playerHP;

        Image _loadingScreenImg;

        public _GameStatus _gameMode;
        Vector3 _cameraMainPosition;
        
        Material[] _mats;
        public string[] _passiveDatail;
        public Sprite[] _bgList;
        public bool _adsFinish = false;
        public bool _loadNewGame = false;
        public GameObject[] _environment;

        public int _playerHP
        {
            get
            {
                return this.playerHP;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }else if (value > 100)
                {
                    value = 100;
                }
                this.playerHP = value;
                _playerSoulBar.GetComponent<PlayerSoul>().AddFill(playerHP);
                _playerSoulBar.transform.Find("SoulText").GetComponent<Text>().text = playerHP + "/100";
            }
        }
        
        public int _currentMoney
        {
            get
            {
                return this.money;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.money = value;
            }
        }

        private void Awake()
        {
            Caching.ClearCache();
            _passiveDatail = new string[]{"หนุมาน:หากตายในเทิร์นที่มีลมพัดผ่าน จะฟื้นคืนเลือด 50",
                "นักมวย:หากเลือดต่ำกว่า 50% พลังโจมตีจะเพิ่มขึ้น 25%",
                "อภัยมณี:หากเลือดมากกว่า 50% อภัยมณีจะได้รับดาเมจเวทย์ลดลง",
                "ทศกัณฐ์:หากตายในตอนที่หลอดความโกรธมากกว่า 75% จะฟื้นคืนชีพด้วยเลือด 25%",
                "ราม:เมื่อใช้สกิลไม้ตายตอนศัตรูเลือดต่ำกว่า 25% พลังโจมตีจะเพิ่มขึ้น200%",
                "สีดา:เมื่อสิ้นชีพ ฮีโร่ในทีมจะได้รับการฮิวเลือด 50% ของเลือดสูงสุดตน",
                "สุครีพ:เมื่อสิ้นชีพจะส่งต่อค่าความโกรธของตัวเองให้ฮีโร่ในทีม",
                "หมอผี:เมื่อเลือดต่ำกว่า 25% จะได้รับความเสียหายกายภาพลดลง",
                "เงาะป่า:หากเลือดน้อยกว่า 1 ใน 3 แล้วป้องกันสำเร็จจะได้เลือดคืนครึ่งหนึ่งของดาเมจที่ทำได้" };
            SetComponent();
            _cal = new Calculate();
            _json = new JsonReadWrite();
            dataSetting = _json.ReadSetting(dataSetting);
            AudioListener.volume = dataSetting[0].soundValue;
            _uiCanvas = GameObject.Find("UICanvas");
            _loadingScreenImg = _loadingScreen.GetComponent<Image>();
            _mats = Resources.LoadAll("Sprites/Character/Hero/", typeof(Material)).Cast<Material>().ToArray();
            googleSheet = this.gameObject.AddComponent<GoSheets>();
            SetingBeforeStart();
            OpenGameMenu();
            
        }
        
        void Start()
        {
        }

        void SetComponent()
        {
            _battleCon = _battleObj.GetComponent<BattleController>();
            _mapCon = _mapObj.GetComponent<MapController>();
            _buffCon = _buffPanel.GetComponent<BuffController>();
            _monCom = _monPanel.GetComponent<MonPanel>();
            _selectATKCon = _attackPanel.GetComponent<SelectAttackController>();
        }
        
        public bool isPaused = false;
        bool pauseLock = false;

        public GameObject _campfireObj;

        void OnGUI()
        {
            if (isPaused)
            {
                if (pauseLock) return;
                pauseLock = true;
                //Debug.Log("Pause");
                GUI.Label(new Rect(0, 0, 250, 100), "Game paused");
                _uiCanvas.SetActive(false);
                if (_campObj.activeSelf)
                {
                    _campfireObj.transform.Find("Point Light").gameObject.SetActive(false);
                    foreach (Transform child in _campfireObj.transform.Find("Fire"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                if(_battleObj.activeSelf)
                {
                    _battleCon.GetHeroFocus()._anim.enabled = false;
                    _battleCon.GetMonFocus()._anim.enabled = false;
                }
            }
            else
            {
                if (!pauseLock) return;
                pauseLock = false;
                //Debug.Log("Unpause");
                _uiCanvas.SetActive(true);
                if (_campObj.activeSelf)
                {
                    _campfireObj.transform.Find("Point Light").gameObject.SetActive(true);
                    foreach (Transform child in _campfireObj.transform.Find("Fire"))
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                if (_battleObj.activeSelf)
                {
                    _battleCon.GetHeroFocus()._anim.enabled = true;
                    _battleCon.GetMonFocus()._anim.enabled = true;
                }
                if (_mapObj.activeSelf)
                {
                    if (_mapCon._roomLoad != null)
                        _mapCon._roomLoad.transform.Find("LightMask").GetComponent<MaskLight>().enabled = true;
                }
                    
            }
                

        }

        void OnApplicationFocus(bool hasFocus)
        {
#if (!UNITY_EDITOR)
            isPaused = !hasFocus;
#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
#if (!UNITY_EDITOR)
            isPaused = pauseStatus;
#endif
        }

        void Update()
        {
            if (loadScene)
            {
                //Debug.Log("loading...");
                Color curColor = _loadingScreenImg.color;
                float alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
                if (alphaDiff > 0.0001f)
                {
                    curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
                    _loadingScreenImg.color = curColor;
                }else
                {
                    loadScene = false;
                    if(targetAlpha == 0)
                        _loadingScreen.SetActive(false);
                }
            }
            if (_gameMode == _GameStatus.GAMEMENU)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                if (!moveLeft)
                {
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney);
                    if (transform.position.x >= endMarker.x && moveLeft == false)
                    {
                        moveLeft = true;
                        //Debug.Log("a " + transform.position.x);
                        startTime = Time.time;
                        journeyLength = Vector3.Distance(startMarker, endMarker);
                    }
                }
                else
                {
                    transform.position = Vector3.Lerp(endMarker, startMarker, fracJourney);
                    if (transform.position.x <= startMarker.x && moveLeft == true)
                    {
                        moveLeft = false;
                        //Debug.Log("b " + transform.position.x);
                        startTime = Time.time;
                        journeyLength = Vector3.Distance(endMarker, startMarker);
                    }
                }
            }
        }

        Vector3 startMarker;
        Vector3 endMarker;
        float speed = 0.01f;
        private float startTime;
        private float journeyLength;
        bool moveLeft = false;

        void OpenGameMenu()
        {
            journeyLength = Vector3.Distance(startMarker, endMarker);
            _gameMode = _GameStatus.GAMEMENU;
            _gameMenu.SetActive(true);
            OpenObjInScene(_campObj);
            _settingPanel.SetActive(false);
            _miniGameMenu.SetActive(false);
        _mainMenu.SetActive(false);
            _tutorialPanel.SetActive(false);
            _playerSoulBar.SetActive(false);
            _cookMenu.SetActive(false);
            OpenActionPanel();
        }
        GameObject _uiCanvas;

        void SetingBeforeStart()
        {
            if (_loadingScreenImg == null)
            {
                Debug.LogError("Error: No image on " + _loadingScreen.name);
            }
            targetAlpha = _loadingScreenImg.color.a;
            Camera.main.orthographicSize = 0.5f;
            startMarker = new Vector3(-0.35f, 0.2f, transform.position.z);
            endMarker = new Vector3(startMarker.x + 0.65f, startMarker.y, startMarker.z);
        }

        void LoadStartScene()
        {
            _gameMode = _GameStatus.START;
            Camera.main.orthographicSize = 0.8f;
            Camera.main.transform.position = new Vector3(0, 0f, Camera.main.transform.position.z);
            _cameraMainPosition = transform.position;
            _questionUrl = dataSetting[0].questionLink;
            LoadQuestion();
            ReadDataAll();
            dataPlayerLog = _json.ReadDataPlayerLog(dataPlayerLog);
            CompilePlayerLog();
            LoadCampAvatar();
            _gameMenu.SetActive(false);
            _miniGameMenu.SetActive(true);
            _playerSoulBar.SetActive(true);
        _mainMenu.SetActive(true);
            if (dataPlayerLog[0].landScene)
            {
                StartCoroutine(LoadingScene(_GameStatus.LAND, false));
            }
            else
            {
                StartCoroutine(LoadingScene(_GameStatus.CAMP, false));
            }
            
        }
        
        public void OpenActionPanel(GameObject obj=null)
        {
            _manageHeroPanel.SetActive(obj == _manageHeroPanel ? true : false);
            _itemPanel.SetActive(obj == _itemPanel ? true : false);
            _CharacterPanel.SetActive(obj == _CharacterPanel ? true : false);
            _attackPanel.SetActive(obj == _attackPanel ? true : false);
            //_defensePanel.SetActive(obj == _defensePanel ? true : false);
            //if (obj == _teamPanel || obj == _itemPanel)
                //_storyPanel.SetActive(true);
            //else
               // _storyPanel.SetActive(obj == _storyPanel ? true : false);
            _questionPanel.SetActive(obj == _questionPanel ? true : false);
            //_skillPanel.SetActive(obj == _skillPanel ? true : false);
        }

        void ReadDataAll()
        {
            TextAsset loadedDungeon = Resources.Load<TextAsset>("JsonDatabase/DungeonList");
            dataDungeonList = JsonHelper.FromJson<DungeonDataSet>(loadedDungeon.text);
            print("load " + dataDungeonList.Length + " dungeons.");

            TextAsset loadedMonster = Resources.Load<TextAsset>("JsonDatabase/MonsterList");
            dataMonsterList = JsonHelper.FromJson<MonsterDataSet>(loadedMonster.text);
            print("load " + dataMonsterList.Length + " monsters.");

            TextAsset loadedHero = Resources.Load<TextAsset>("JsonDatabase/HeroList");
            dataHeroList = JsonHelper.FromJson<HeroDataSet>(loadedHero.text);
            print("load " + dataHeroList.Length + " heroes.");

            TextAsset loadedItem = Resources.Load<TextAsset>("JsonDatabase/ItemList");
            dataItemList = JsonHelper.FromJson<ItemDataSet>(loadedItem.text);
            print("load " + dataItemList.Length + " items.");

            TextAsset loadedUlti = Resources.Load<TextAsset>("JsonDatabase/SkillList");
            dataSkillList = JsonHelper.FromJson<SkillDataSet>(loadedUlti.text);
            print("load " + dataSkillList.Length + " skills.");
            
        }
        
        void CompilePlayerLog(int SaveNum = 0)
        {
            _playerHP = dataPlayerLog[SaveNum].hp;
            _currentDungeonLayer = dataPlayerLog[SaveNum].dungeonLayer;
            _currentRoomPosition = dataPlayerLog[SaveNum].roomPosition;
            _currentMoney = dataPlayerLog[SaveNum].money;
            ///------load itemstore------
            if(dataPlayerLog[0].itemStore != "")
            {
                string[] itemStore = dataPlayerLog[0].itemStore.Split(',');
                _itemStore = new List<ItemStore>();
                int rowReal = 0;
                for (int i = 0; i < itemStore.Length; i++)
                {
                    string[] itemData = itemStore[i].Split(':');

                    foreach (ItemDataSet data in dataItemList)
                    {
                        if (_cal.IntParseFast(itemData[1]) == data.id)
                        {
                            //ItemStore item = new ItemStore();
                            //item.id = _cal.IntParseFast(itemData[0]);
                            //item.itemId = data.id;
                            //item.amount = _cal.IntParseFast(itemData[2]);
                            //item.item = data;
                            //_itemStore.Add(item);
                            break;
                        }
                        rowReal++;
                    }
                }
                //Debug.Log("foreach row " + rowReal);
                int dataCount = 0;
                int row = 0;
                int itemCount = 0;
                do
                {
                    if (dataCount >= dataItemList.Length) dataCount = 0;
                    else if (dataCount < 0) dataCount = dataItemList.Length - 1;
                    string[] itemData = itemStore[itemCount].Split(':');
                    //Debug.Log(dataItemList[dataCount].id + " == " + _cal.IntParseFast(itemData[1]));
                    if (dataItemList[dataCount].id == _cal.IntParseFast(itemData[1]))
                    {
                        //Debug.Log("add item" + row);
                        ItemStore item = new ItemStore();
                        item.id = _cal.IntParseFast(itemData[0]);
                        item.itemId = dataItemList[dataCount].id;
                        item.amount = _cal.IntParseFast(itemData[2]);
                        item.item = dataItemList[dataCount];
                        _itemStore.Add(item);
                        itemCount++;
                    }
                    else
                    {
                        if (dataItemList[dataCount].id >= _cal.IntParseFast(itemData[1]))
                            dataCount--;
                        else
                            dataCount++;
                    }
                    row++;
                } while (itemCount < itemStore.Length);
            }
            
            //Debug.Log("do while row" + row);
            ///-------end load itemstore---------------

            ///-------load herostore ------------
            _heroStore = new List<HeroStore>();
            string[] heroStore = dataPlayerLog[0].heroStore.Split(',');
            for (int p = 0; p < heroStore.Length; p++)
            {
                string[] teamData = heroStore[p].Split(':');
                HeroStore hero = new HeroStore();
                hero.id = _cal.IntParseFast(teamData[0]);
                hero.heroId = _cal.IntParseFast(teamData[1]);
                hero.exp = double.Parse(teamData[2]);
                foreach (HeroDataSet data in dataHeroList)
                {
                    if (hero.heroId == data.id)
                    {
                        hero.hero = data;
                        hero.level = _cal.CalculateLevel(hero.exp);
                        //hero.STR = _cal.CalculateSTR(hero);
                        //hero.AGI = _cal.CalculateAGI(hero);
                        //hero.INT = _cal.CalculateINT(hero);
                        //hero.hpMax = _cal.CalculateHpMax(hero);
                        hero.hp = _cal.IntParseFast(teamData[3]);
                        hero.ATK = _cal.CalculateATK(hero);
                        hero.MATK = _cal.CalculateMATK(hero);
                        hero.DEF = _cal.CalculateDEF(hero);
                        hero.MDEF = _cal.CalculateMDEF(hero);
                        //hero.hp = hero.hpMax;//for test 'set hp full max'
                        //Debug.Log("before " + data.baseSTR + " " + data.baseAGI + " " + data.baseINT + " after" + hero.STR + " " + hero.AGI + " " + hero.INT);
                        string[] skillList = data.skillList.Split(':');
                        for (int a = 0; a < skillList.Length; a++)
                        {
                            foreach(SkillDataSet skill in dataSkillList)
                            {
                                if(_cal.IntParseFast(skillList[a])== skill.id)
                                {
                                    Skill attack = new Skill();
                                    string[] buffList = skill.buffList.Split(',');
                                    for (int s = 0; s < buffList.Length; s++)
                                    {
                                        string[] buff = buffList[s].Split(':');
                                        Buff dataBuff = new Buff();
                                        dataBuff.id = _cal.IntParseFast(buff[0]);
                                        dataBuff.icon = _cal.IntParseFast(buff[1]);
                                        dataBuff.timeCount = _cal.IntParseFast(buff[2]);
                                        dataBuff.whoUse = _Model.PLAYER;
                                        dataBuff.forMe = (_cal.IntParseFast(buff[3])==0)? true: false;
                                        attack.buff.Add(dataBuff);
                                    }
                                    attack.hate = (int)skill.bonusDmg * 20;
                                    attack.skill = skill;
                                    hero.attack[a] = attack;
                                    break;
                                }
                            }
                        }
                        hero.passive = (_Passive)data.passiveId;
                        _heroStore.Add(hero);
                        break;
                    }
                }

            }

            ///-----end load herostore-----
            _currentTeamIsSelect = dataPlayerLog[SaveNum].teamSelected;
            ///-----load teamsetup-----
            _teamSetup = new List<HeroInTeam>();
            string[] teamSetup = dataPlayerLog[SaveNum].teamSetup.Split(',');
            for (int l = 0; l < teamSetup.Length; l++)
            {
                string[] heroPos = teamSetup[l].Split(':');
                HeroInTeam team = new HeroInTeam();
                for (int m = 0; m < heroPos.Length; m++)
                {
                    if (Int32.Parse(heroPos[m]) == -1)
                    {
                        HeroStore hero = new HeroStore();
                        hero.id = -1;
                        team.position.Add(hero);
                    }
                    else
                    {
                        foreach (HeroStore hero in _heroStore)
                        {
                            if (hero.id == _cal.IntParseFast(heroPos[m]))
                            {
                                //HeroStore newHero = new HeroStore();
                                //newHero = hero;
                                team.position.Add(hero);
                                break;
                            }
                        }
                    }

                }
                _teamSetup.Add(team);

            }
            /*
            foreach(HeroInTeam hero in _teamSetup)
            {
                foreach (HeroStore heroS in hero.position)
                {
                    Debug.Log("print hero "+heroS.id);
                }
            }*/
            ///-----end load teamsetup-----
            ///-----load dungeonClear-----
            _dungeon = new Dungeon[dataDungeonList.Length];
            for (int i = 0; i < dataDungeonList.Length; i++)
            {
                Dungeon dun = new Dungeon();
                dun.dungeon = dataDungeonList[i];
                _dungeon[i] = dun;
            }
            string[] dunLevel = dataPlayerLog[SaveNum].dungeonIsPass.Split(',');
            for (int i = 0; i < dunLevel.Length; i++)
            {
                string[] roomData = dunLevel[i].Split('_');
                string[] roomClear = roomData[1].Split(':');
                for (int a = 0; a < roomClear.Length; a++)
                {
                    Room room = new Room();
                    string[] r = roomClear[a].Split('-');
                    room.id = _cal.IntParseFast(r[0]);
                    room.passCount = _cal.IntParseFast(r[1]);
                    room.escapeCount = _cal.IntParseFast(r[2]);
                    _dungeon[_cal.IntParseFast(roomData[0]) - 1].roomIsPass.Add(room);
                }

            }
            ///-----End load dungeonClear-----
            string[] shopList = dataPlayerLog[SaveNum].shopList.Split(',');
            _landShopList = new List<ItemShop>();
            for(int i =0;i< shopList.Length; i++)
            {
                string[] shopCut = shopList[i].Split(':');
                ItemShop newShop = new ItemShop();
                newShop.id = _cal.IntParseFast(shopCut[0]);
                newShop.buyCount = _cal.IntParseFast(shopCut[1]);
                foreach (ItemDataSet data in dataItemList)
                {
                    if (newShop.id == data.id)
                    {
                        newShop.item = data;
                        break;
                    }
                }
                _landShopList.Add(newShop);
            }
        }
        
        public void LoadCampAvatar(GameObject obj = null)
        {
            Sprite[] loadSprite = null;
            string getSpriteSet = "";

            for (int i = 0; i < 1; i++)
            {
                if (_teamSetup[_currentTeamIsSelect - 1].position[i].id == -1)
                {
                    if (getSpriteSet != "hero")
                    {
                        getSpriteSet = "hero";
                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                    }
                    _campHeroSprite[i].GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Shadow");
                    _campHeroSprite[i].GetComponent<SpriteRenderer>().material = _mats.Single(s => s.name == getSpriteSet);//Resources.Load<Material>(getSpriteSet);
                }
                else
                {
                    if (getSpriteSet != _teamSetup[_currentTeamIsSelect - 1].position[i].hero.spriteSet)
                    {
                        getSpriteSet = _teamSetup[_currentTeamIsSelect - 1].position[i].hero.spriteSet;
                        loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                    }
                    _campHeroSprite[i].GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == _teamSetup[_currentTeamIsSelect - 1].position[i].hero.spriteName);
                    _campHeroSprite[i].GetComponent<SpriteRenderer>().material = _mats.Single(s => s.name == getSpriteSet);

                }

            }
        }
        
        public void LoadQuestion()
        {
            _questionLoadComplete = 0;
            googleSheet.GetCell(_questionUrl, 9, 1, GetRowMax);
        }
        public int _columnQuestionMax =0;
        int _rowQuestionMax = 0;

        void GetRowMax(string link, int c, int r, string data)
        {
            if (data != "")
            {
                _rowQuestionMax = Int32.Parse(data)+1;
                googleSheet.GetCell(_questionUrl, 11, 1, GetColumnMax);
            }
        }

        void GetColumnMax(string link, int c, int r, string data)
        {
            if (data != "")
            {
                _columnQuestionMax = Int32.Parse(data)+2;
                
                //Debug.Log("r " + _rowQuestionMax + " c " + _columnQuestionMax);
                _questionList = new string[_rowQuestionMax, _columnQuestionMax];
                StartCoroutine(LoadQuestionList());
            }
        }
        
        IEnumerator LoadQuestionList()
        {
            for (int ro = 2; ro <= _rowQuestionMax; ro++)
            {
                for (int co = 1; co <= _columnQuestionMax; co++)
                {
                    googleSheet.GetCell(_questionUrl, co, ro, AddQuestionToArray);
                    
                }
                yield return new WaitForSeconds(1);
            }
            
        }
        public int _questionLoadComplete = 0;
        int _questionLoadError = 0;

        void AddQuestionToArray(string link, int c, int r, string data)
        {
            if (data != "")
            {
                //Debug.Log(c + " " + r +" = "+data);
                //Debug.Log("complete position " + r + "," + c);
                _questionList[r - 2, c-1] = data;
                if (c == _columnQuestionMax)
                    _questionLoadComplete++;
                if (c == _columnQuestionMax && r == _rowQuestionMax)
                {
                    print("question load complete " + _questionLoadComplete + " ,error "+ _questionLoadError);
                }
            }
            else
            {
                _questionLoadError++;
                Debug.Log("error! position " + r + "," + c);
            }
        }

        public float FadeRate;
        float targetAlpha;
        bool loadScene = false;

        public void FadeOut()
        {
            //Debug.Log("FadeOut!");
            _loadingScreenImg.color = new Color(_loadingScreenImg.color.r, _loadingScreenImg.color.g, _loadingScreenImg.color.b, 1);
            _loadingScreen.SetActive(true);
            loadScene = true;
            targetAlpha = 0.0f;
        }

        public void FadeIn()
        {
            //Debug.Log("FadeIn!");
            _loadingScreen.SetActive(true);
            loadScene = true;
            targetAlpha = 1.0f;
        }

        IEnumerator SavePlayerData(bool ExitGame=false)
        {
            _loadingNotify.transform.Find("BG").GetComponentInChildren<Text>().text = "กำลังเซฟข้อมูลเกม..."; 
            _loadingNotify.SetActive(true);
            _json.WriteDataPlayerLog(dataPlayerLog);
            yield return new WaitForSeconds(3);
            _loadingNotify.SetActive(false);
            if (ExitGame)
            {
                Application.Quit();
            }
        }

        IEnumerator LoadingScene(_GameStatus mode,bool save=true)
        {
            _gameMode = mode;
            FadeIn();
            yield return new WaitForSeconds(3);
            ParticleSystem ps = _environment[0].GetComponent<ParticleSystem>();
            var coll = ps.collision;
            coll.enabled = false;
            switch (mode)
            {
                case _GameStatus.BATTLE:
                    OpenBattleScene();
                    coll.enabled = true;
                    break;
                case _GameStatus.CAMP:
                    OpenCampScene();
                    break;
                case _GameStatus.MAP:
                    OpenMapScene();
                    break;
                case _GameStatus.LAND:
                    OpenLandScene();
                    break;
                case _GameStatus.FORESTSHOP:
                    OpenForestShopScene();
                    break;
            }
            //yield return new WaitForSeconds(1);
            if (save)
                _json.WriteDataPlayerLog(dataPlayerLog);
            FadeOut();
            if(_unlockHeroList!=null)
                OpenUnlockNotify();

            if (_loadNewGame)
            {
                _cutscene = Instantiate(_cutscenePanel);
                _cutscene.transform.SetParent(_gameMenu.transform.parent.gameObject.transform);
                _cutscene.transform.localPosition = new Vector3(0, 0, 0);
                _cutscene.transform.localScale = new Vector3(1, 1, 1);
                _loadNewGame = false;
            }
        }
        public GameObject _cutscene;

        public void CallSubMenu(_SubMenu mode)
        {
            CallSubMenu(mode, "");
        }
        _SubMenu _subMenuMode;
        public _ActionStatus _ActionMode;
        
        public void CallSubMenu(_SubMenu mode, string topic)
        {
            _subMenuMode = mode;
            Transform trans = _subMenuPanel.transform.Find("GridView").transform;
            _subMenuPanel.transform.Find("CloseButton").gameObject.SetActive(false);
            trans.Find("Post").gameObject.SetActive(false);
            trans.Find("ConfirmButton").gameObject.SetActive(false);
            trans.Find("CancelButton").gameObject.SetActive(false);
            trans.Find("UseButton").gameObject.SetActive(false);
            trans.Find("SellButton").gameObject.SetActive(false);
            trans.Find("BuyButton").gameObject.SetActive(false);
            trans.Find("SaveButton").gameObject.SetActive(false);
            trans.Find("NewGameButton").gameObject.SetActive(false);
            trans.Find("SettingButton").gameObject.SetActive(false);
            trans.Find("ExitGameButton").gameObject.SetActive(false);
            trans.Find("BackTownButton").gameObject.SetActive(false);

            if (_subMenuMode == _SubMenu.Item)
            {
                trans.Find("CancelButton").gameObject.SetActive(true);
                trans.Find("SellButton").gameObject.SetActive(true);
            }
            else if (_subMenuMode == _SubMenu.Alert)
            {
                trans.Find("Post").gameObject.SetActive(true);
                trans.Find("Post").gameObject.GetComponentInChildren<Text>().text = topic;
                trans.Find("ConfirmButton").gameObject.SetActive(true);
            }
            else if (_subMenuMode == _SubMenu.Shop)
            {
                trans.Find("CancelButton").gameObject.SetActive(true);
                trans.Find("BuyButton").gameObject.SetActive(true);
            }
            else if (_subMenuMode == _SubMenu.Warp)
            {
                trans.Find("Post").gameObject.SetActive(true);
                trans.Find("Post").gameObject.GetComponentInChildren<Text>().text = topic;
                trans.Find("ConfirmButton").gameObject.SetActive(true);
                trans.Find("CancelButton").gameObject.SetActive(true);
            }
            else if (_subMenuMode == _SubMenu.GameMenu)
            {
                _subMenuPanel.transform.Find("CloseButton").gameObject.SetActive(true);
                trans.Find("SaveButton").gameObject.SetActive(true);
                trans.Find("NewGameButton").gameObject.SetActive(true);
                trans.Find("SettingButton").gameObject.SetActive(true);
                trans.Find("ExitGameButton").gameObject.SetActive(true);
            }else if (_subMenuMode == _SubMenu.BattleEnd)
            {
                trans.Find("Post").gameObject.SetActive(true);
                trans.Find("Post").gameObject.GetComponentInChildren<Text>().text = topic;
                trans.Find("BackTownButton").gameObject.SetActive(true);
            }else if (_subMenuMode == _SubMenu.ManageHero)
            {
                trans.Find("CancelButton").gameObject.SetActive(true);
            }
            else if (_subMenuMode == _SubMenu.LoadBattleRevive)
            {
                trans.Find("Post").gameObject.SetActive(true);
                trans.Find("Post").gameObject.GetComponentInChildren<Text>().text = topic;
                trans.Find("HeroReviveButton").gameObject.SetActive(true);
            }
            else if (_subMenuMode == _SubMenu.GameOver)
            {
                trans.Find("Post").gameObject.SetActive(true);
                trans.Find("Post").gameObject.GetComponentInChildren<Text>().text = topic;
                trans.Find("ConfirmButton").gameObject.SetActive(true);
            }
            _subMenuPanel.SetActive(true);
        }

        /// <summary>
        /// Event Button Zone....
        /// </summary>
        /// 
        public void SubMenuCloseBtn()
        {
            _subMenuPanel.SetActive(false);
        }
        
        public void SubMenuBackTownBtn()
        {
            //_battleCon.IsRevive = true;
            CalEscapeRoom();
            _subMenuPanel.SetActive(false);
            LoadScene(_GameStatus.LAND);
        }

        public void StartBtn()
        {
            LoadStartScene();
        }

        public void SettingBtn()
        {
            _settingPanel.SetActive(true);
            _gameMenu.SetActive(false);
        }

        public void SubMenuSaveBtn()
        {
            StartCoroutine(SavePlayerData());
        }

        public void SubMenuNewGameBtn()
        {
            OpenConfirmNotify("เจ้าแน่ใจว่าต้องการเริ่มเกมใหม่?", _ConfirmNotify.NewGame);
        }

        public void SubMenuExitGameBtn()
        {
            OpenConfirmNotify("เจ้าแน่ใจว่าต้องการออกจากเกม?", _ConfirmNotify.ExitGame);
        }

        public void ExitGameBtn()
        {
            Application.Quit();
        }

        public void MiniGameMenuBtn()
        {
            if (_subMenuMode== _SubMenu.GameMenu && _subMenuPanel.activeSelf)
            {
                if(!_settingPanel.activeSelf)
                    _subMenuPanel.SetActive(false);
            }
            else
            {
                CallSubMenu(_SubMenu.GameMenu);
            }
        }
        
        public void AttackBtn()
        {
            OpenActionPanel(_attackPanel);
        }

        public void DefenseBtn()
        {
            if (_cutscene != null)
            {
                //_cutscene.GetComponent<Cutscene>().TutorialPlay(_defensePanel.transform.Find("Defense2"));
            }
            //OpenActionPanel(_defensePanel);
        }

        public GameObject _itemBtn;
        public Sprite[] _BagIcon;

        public void ItemBtn()
        {
            _ActionMode = _ActionStatus.Item;
            if (_itemPanel.activeSelf)
            {
                _itemBtn.GetComponent<Image>().sprite = _BagIcon[0];
                _itemPanel.SetActive(false);
                if(_gameMode == _GameStatus.BATTLE)
                    _attackPanel.SetActive(true);
            }
            else
            {
                _itemBtn.GetComponent<Image>().sprite = _BagIcon[1];
                OpenActionPanel(_itemPanel);
            }
        }

        public void TeamBtn()
        {
            _ActionMode = _ActionStatus.Team;
            if (_gameMode == _GameStatus.BATTLE)
            {
                //_menuPanel.transform.parent.gameObject.SetActive(false);
                OpenActionPanel(_manageHeroPanel);
            }
            else
            {
                if (_CharacterPanel.activeSelf)
                {
                    _CharacterPanel.SetActive(false);
                }
                else
                    OpenActionPanel(_CharacterPanel);
            }
        }
        
        public void SubMenuConfirmBtn()
        {
            _subMenuPanel.SetActive(false);
            if (_subMenuMode == _SubMenu.Alert)
            {

            }else if (_subMenuMode == _SubMenu.Warp)
            {
                _currentDungeonLayer = _gatePanel.GetComponent<GatePanel>()._dungeonLayerIsSelect;
                _currentRoomPosition = _dungeon[_currentDungeonLayer - 1].dungeon.startRoom;
                LoadScene(_GameStatus.MAP);
                _gatePanel.SetActive(false);
                _talkPanel.SetActive(false);
            }else if (_subMenuMode == _SubMenu.BattleEnd)
            {
                LoadScene(_GameStatus.MAP);
            }else if (_subMenuMode == _SubMenu.GameOver)
            {
                DeleteFile();
                foreach (Transform child in _mapObj.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                _mapObj.transform.DetachChildren();
                //_subMenuPanel.SetActive(false);
                //_confirmNotify.SetActive(false);
                SetingBeforeStart();
                OpenGameMenu();
            }
        }

        public void SubMenuCancelBtn()
        {
            _subMenuPanel.SetActive(false);
            if (_subMenuMode == _SubMenu.Alert)
            {

            }
            else if (_subMenuMode == _SubMenu.ManageTeam)
            {
                //_menuPanel.transform.parent.gameObject.SetActive(true);
            }
        }

        

        public void MapBtn()
        {
            _ActionMode = _ActionStatus.Map;
            if (_gameMode == _GameStatus.BATTLE)
            {
                if (!_battleCon._isEscape)
                {
                    _battleCon._isEscape = true;
                    if (UseCrystal(2))
                    {
                        if (Random.Range(0f, 1f) <= _mapCon._escapeRate)
                        {
                            LoadScene(_GameStatus.MAP);
                            CalEscapeRoom();
                        }
                        else
                        {
                            _battleCon.ShowDamage("หลบหนีล้มเหลว", _battleCon.GetHeroFocus()._avatar.transform.position);
                            _battleCon.EndTurnSpeed();
                            _mapCon._escapeRate += 0.05f;
                        }
                   
                    }
                    else
                    {
                        //CallSubMenu(_SubMenu.Alert, "คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย 2");
                        OpenErrorNotify("คริสตัลของคุณไม่เพียงพอ จำเป็นต้องมีอย่างน้อย 2");
                    }
                }

            }
            else
            {
                _CharacterPanel.SetActive(false);
                _itemPanel.SetActive(false);
                LoadScene(_GameStatus.MAP);
            }
            
            
        }
        
        public void ConfirmNotifyOkayBtn()
        {
            if(_confirmNotifyMode == _ConfirmNotify.NewGame)
            {
                DeleteFile();
                foreach (Transform child in _mapObj.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                _mapObj.transform.DetachChildren();
                LoadStartScene();
                _subMenuPanel.SetActive(false);
                _confirmNotify.SetActive(false);
            }
            else if(_confirmNotifyMode == _ConfirmNotify.ExitGame)
            {
                OpenConfirmNotify("เจ้าต้องการเซฟเกมหรือไม่?", _ConfirmNotify.SaveAndExit);
                
            }else if (_confirmNotifyMode == _ConfirmNotify.SaveAndExit)
            {
                StartCoroutine(SavePlayerData(true));
                _confirmNotify.SetActive(false);
            }
        }

        public void ConfirmNotifyCancelBtn()
        {
            if (_confirmNotifyMode == _ConfirmNotify.NewGame)
            {
                _confirmNotify.SetActive(false);
            }
            else if (_confirmNotifyMode == _ConfirmNotify.ExitGame)
            {
                _confirmNotify.SetActive(false);
            }
            else if (_confirmNotifyMode == _ConfirmNotify.SaveAndExit)
            {
                _confirmNotify.SetActive(false);
                Application.Quit();
            }
        }

        /// End Event button zone.
        /// 
        _ConfirmNotify _confirmNotifyMode;
        void OpenConfirmNotify(string txt, _ConfirmNotify mode)
        {
            _confirmNotifyMode = mode;
            _confirmNotify.SetActive(true);
            _confirmNotify.transform.Find("Text").GetComponent<Text>().text = txt;
        }

        void DeleteFile(string fileName = "PlayerLog.json")
        {
            string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/dataFile/";
            string filePath = folderPath + fileName;
            File.Delete(filePath);
            RefreshEditorProjectWindow();
        }

        void RefreshEditorProjectWindow()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public void LoadScene(_GameStatus mode)
        {
            StartCoroutine(LoadingScene(mode));
        }
        
        void OpenMapScene()
        {
            dataPlayerLog[0].landScene = false;
            OpenObjInScene(_mapObj);
            //OpenActionPanel(_storyPanel);
        }

        void OpenBattleScene()
        {
            OpenObjInScene(_battleObj);
            OpenActionPanel(_attackPanel);
            transform.position = _cameraMainPosition;
            //SetMenuPanel(_gameMode);
        }

        void OpenCampScene()
        {
            OpenObjInScene(_campObj);
            //OpenActionPanel(_storyPanel);
            transform.position = _cameraMainPosition;
            //SetMenuPanel(_gameMode);
        }

        public void OpenLandScene()
        {
            dataPlayerLog[0].landScene = true;
            OpenObjInScene(_landObj);
            transform.position = _cameraMainPosition;
        }

        void OpenForestShopScene()
        {
            OpenObjInScene(_forestShopObj);
            _forestShopPanel.SetActive(true);
        }
        
        void OpenObjInScene(GameObject obj)
        {
            _forestShopObj.SetActive(obj == _forestShopObj ? true : false);
            _landObj.SetActive(obj == _landObj ? true : false);
            if (_landObj.activeSelf)
            {
                _campObj.SetActive(true);
            }
            else
            {
                _campObj.SetActive(obj == _campObj ? true : false);
            }
            _battleObj.SetActive(obj == _battleObj ? true : false);
            _mapObj.SetActive(obj == _mapObj ? true : false);
            
        }
        
        
        public void SetSpriteType(Image img, _Character type)
        {

            if(type == _Character.HAMMER)
            {
                img.sprite = _typeSprite[0];
                img.color = new Color32(233, 91, 79, 255);
            }
            else if(type == _Character.SCISSORS)
            {
                img.sprite = _typeSprite[1];
                img.color = new Color32(255, 183, 67, 255);
            }
            else
            {
                img.sprite = _typeSprite[2];
                img.color = new Color32(67, 188, 255, 255);
            }
            
        }
        Vector2 padding = new Vector2(20, 20);

        public void SetTalk(string txt)
        {
            Text talk = _talkPanel.GetComponentInChildren<Text>();
            talk.text = txt;
            RectTransform parentRect = _talkPanel.GetComponent<RectTransform>();

            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y-200));
            newPos.z = _talkPanel.transform.position.z;
            _talkPanel.transform.position = newPos;

            if (_CharacterPanel.activeSelf)
            {
                parentRect.sizeDelta = new Vector2(Screen.width, 260);
            }else
                parentRect.sizeDelta = new Vector2(talk.preferredWidth, talk.preferredHeight) + padding;
            _talkPanel.SetActive(true);
        }

        public void SetInfo(string txt)
        {
            Text talk = _infoPanel.GetComponentInChildren<Text>();
            talk.text = txt;
            RectTransform parentRect = _infoPanel.GetComponent<RectTransform>();
            parentRect.sizeDelta = new Vector2(talk.preferredWidth, talk.preferredHeight) + padding;
            _infoPanel.SetActive(true);
        }

        public void SetMonTalk(string txt)
        {
            Text talk = _monTalkPanel.GetComponentInChildren<Text>();
            talk.text = txt;
            RectTransform parentRect = _monTalkPanel.GetComponent<RectTransform>();
            parentRect.sizeDelta = new Vector2(talk.preferredWidth, talk.preferredHeight) + padding;
            _battleCon.timeLeft = 3;
            _monTalkPanel.SetActive(true);
        }

        public HeroStore GetLeader()
        {
            return _teamSetup[0].position[0];
        }

        public bool CheckCrystal(int point)
        {
            if (_gameMode != _GameStatus.BATTLE) return true;
            if (_battleCon.Crystal - point < 0)
            {
                return false;
            }
            return true;
        }

        public bool UseCrystal(int point)
        {
            if (_gameMode != _GameStatus.BATTLE) return true;
            if (_battleCon.Crystal - point < 0)
            {
                return false;
            }
            _battleCon.Crystal = _battleCon.Crystal - point;
            if (_battleCon.Crystal == 0)
            {
                _battleCon.EndTurnSpeed();
            }
            return true;
        }

        public void CalEscapeRoom()
        {
            foreach (Room room in _dungeon[_currentDungeonLayer - 1].roomIsPass)
            {
                if (room.id == _currentRoomPosition)
                {
                    room.escapeCount++;
                    break;
                }
            }
        }
        public List<string> _unlockHeroList;
        public PopupText _unlockNotifyPopup;

        public void OpenUnlockNotify()
        {
            StartCoroutine(ShowUnlockNotify());
        }

        IEnumerator ShowUnlockNotify()
        {
            yield return new WaitForSeconds(1.5f);
            Sprite[] loadSprite = null;
            string getSpriteSet = "";
            for (int i=0;i< _unlockHeroList.Count; i++)
            {
                PopupText unlock = Instantiate(_unlockNotifyPopup);
                unlock.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);
                unlock.transform.localScale = new Vector3(1, 1, 1);
                foreach(HeroDataSet data in dataHeroList)
                {
                    if(_unlockHeroList[i]== data.spriteName)
                    {
                        if (getSpriteSet != data.spriteSet)
                        {
                            getSpriteSet = data.spriteSet;
                            loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
                        }
                        unlock.transform.Find("Panel").Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Icon_" + data.spriteName);
                        unlock.SetPopupText("ปลดล็อค" + data.name + "สำเร็จ");
                        HeroStore hero = new HeroStore();
                        int idStore = 1;
                        foreach(HeroStore h in _heroStore)
                        {
                            if (h.id > idStore)
                            {
                                idStore = h.id;
                            }
                        }
                        hero.id = idStore+1;
                        hero.heroId = data.id;
                        hero.exp = 0;
                        hero.hero = data;
                        hero.level = _cal.CalculateLevel(hero.exp);
                        //hero.STR = _cal.CalculateSTR(hero);
                        //hero.AGI = _cal.CalculateAGI(hero);
                        //hero.INT = _cal.CalculateINT(hero);
                        //hero.hpMax = _cal.CalculateHpMax(hero);
                        //hero.hp = hero.hpMax;
                        hero.ATK = _cal.CalculateATK(hero);
                        hero.MATK = _cal.CalculateMATK(hero);
                        hero.DEF = _cal.CalculateDEF(hero);
                        hero.MDEF = _cal.CalculateMDEF(hero);

                        string[] skillList = data.skillList.Split(':');
                        for (int a = 0; a < skillList.Length; a++)
                        {
                            foreach (SkillDataSet skill in dataSkillList)
                            {
                                if (_cal.IntParseFast(skillList[a]) == skill.id)
                                {
                                    Skill attack = new Skill();
                                    string[] buffList = skill.buffList.Split(',');
                                    for (int s = 0; s < buffList.Length; s++)
                                    {
                                        string[] buff = buffList[s].Split(':');
                                        Buff dataBuff = new Buff();
                                        dataBuff.id = _cal.IntParseFast(buff[0]);
                                        dataBuff.icon = _cal.IntParseFast(buff[1]);
                                        dataBuff.timeCount = _cal.IntParseFast(buff[2]);
                                        dataBuff.whoUse = _Model.PLAYER;
                                        dataBuff.forMe = (_cal.IntParseFast(buff[3]) == 0) ? true : false;
                                        attack.buff.Add(dataBuff);
                                    }
                                    attack.hate = (int)skill.bonusDmg * 20;
                                    attack.skill = skill;
                                    hero.attack[a] = attack;
                                    break;
                                }
                            }
                        }
                        hero.passive = (_Passive)data.passiveId;
                        _heroStore.Add(hero);
                        break;
                    }
                }
                
                yield return new WaitForSeconds(1.5f);
            }
            _unlockHeroList = null;
            //yield return new WaitForSeconds(1.5f);
        }

        public void OpenErrorNotify(string txt)
        {
            StartCoroutine(ShowNotify(txt,false));
        }

        public void OpenTrueNotify(string txt)
        {
            StartCoroutine(ShowNotify(txt, true));
        }

        IEnumerator ShowNotify(string txt,bool icon)
        {
            yield return new WaitForSeconds(0.1f);
            PopupText unlock = Instantiate(_unlockNotifyPopup);
            unlock.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);
            unlock.transform.localScale = new Vector3(1, 1, 1);
            Sprite[] sprite = Resources.LoadAll<Sprite>("Sprites/UI/ui2");
            if (icon)
            {
                unlock.transform.Find("Panel").Find("Icon").GetComponent<Image>().sprite = sprite.Single(s => s.name == "ui2_5");
            }
            else
            {
                unlock.transform.Find("Panel").Find("Icon").GetComponent<Image>().sprite = sprite.Single(s => s.name == "ui2_4");
            }
            unlock.SetPopupText(txt);
            //yield return new WaitForSeconds(1.5f);
        }
        public GameObject _healEffect;

        public GameObject CreateEffect(GameObject effectInput, Transform target)
        {
            GameObject effect = Instantiate(effectInput);
            effect.transform.SetParent(target);
            effect.transform.localScale = new Vector3(1, 1, 1);
            effect.transform.localPosition = Vector3.zero;
            effect.AddComponent<ParticleDestroy>();
            return effect;
        }

        public void SetColliderCamp(bool set)
        {
            foreach (GameObject obj in _campHeroSprite)
            {
                obj.GetComponent<CapsuleCollider2D>().enabled = set;
            }
        }
    }



