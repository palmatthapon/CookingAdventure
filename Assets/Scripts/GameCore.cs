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
using model;
using warp;
using controller;
using player;

public class GameCore : MonoBehaviour
{

    public BattleController _battleCon;
    public MapController _mapCon;
    public BuffController _buffCon;
    public Calculate _cal;
    public AttackController _attackCon;
    public MenuController _menuCon;


    JsonReadWrite _json;
    

    ///------------All DataSet-----------------
    public DungeonDataSet[] dataDungeonList;
    public MonsterDataList[] dataMonsterList;
    public ModelDataSet[] dataHeroList;
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
    public GameObject _menuPanel;
    public GameObject _attackPanel, _questionPanel;
    public Text _questionPanelTxt;
    public GameObject _monPanel;
    public GameObject _eventPanel;
    public GameObject _mapPanel;
    public GameObject _CharacterPanel;
    public GameObject _itemPanel;
    public GameObject _changeTeamPanel;
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
    public GameObject _PlayerInfoPanel;

    /// -----End Panel Zone

    ///-----general----
    public GameObject _loadingScreen;
    public GameObject _loadingNotify;
    public GameObject[] _campAvatar;
    public GameObject _playerSoulBar;
    ///------ End import object by scene-----------

    public Dungeon[] _dungeon;
    public List<ItemStore> _itemStore;
    public List<Hero> _heroStore;
    public List<ItemShop> _landShopList;

    public Hero _heroIsPlaying;
    
    public Monster[] _currentMonsterBattle;
    

    Image _loadingScreenImg;

    public _GameState _gameMode;
    Vector3 _cameraMainPosition;

    
    public string[] _passiveDatail;
    public Sprite[] _bgList;
    public bool _loadNewGame = false;
    public GameObject[] _environment;


    public Sprite[] _uiSprite1;
    public Sprite[] _uiSprite2;
    public Sprite[] _mapSprite;

    public GameObject _cutscene;
    public _SubMenu _subMenuMode;
    public _ActionState _actionMode;


    private void Awake()
    {
        Caching.ClearCache();
        _uiSprite1 = Resources.LoadAll<Sprite>("Sprites/UI/ui");
        _uiSprite2 = Resources.LoadAll<Sprite>("Sprites/UI/ui2");
        _mapSprite = Resources.LoadAll<Sprite>("Sprites/UI/map2");

        _passiveDatail = new string[]{"passive detail" };

        _player = new Player();
        SetComponent();
        _cal = new Calculate();
        _json = new JsonReadWrite();
        dataSetting = _json.ReadSetting(dataSetting);
        AudioListener.volume = dataSetting[0].soundValue;
        _uiCanvas = GameObject.Find("UICanvas");
        _loadingScreenImg = _loadingScreen.GetComponent<Image>();

        SettingBeforeStart();
        OpenGameMenu();

    }

    void Start()
    {
    }

    public static GameCore call() {
        return Camera.main.GetComponent<GameCore>();
    }

    void SetComponent()
    {
        _battleCon = _battleObj.GetComponent<BattleController>();
        _mapCon = _mapObj.GetComponent<MapController>();
        _buffCon = _buffPanel.GetComponent<BuffController>();
        _attackCon = _attackPanel.GetComponent<AttackController>();
        _menuCon = _menuPanel.GetComponent<MenuController>();
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
            if (_battleObj.activeSelf)
            {
                _battleCon.FocusHero().Anim(false);
                _battleCon.FocusMonster().getAnim().enabled = false;
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
                _battleCon.FocusHero().getAnim().enabled = true;
                _battleCon.FocusMonster().getAnim().enabled = true;
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
            }
            else
            {
                loadScene = false;
                if (targetAlpha == 0)
                    _loadingScreen.SetActive(false);
            }
        }
        if (_gameMode == _GameState.GAMEMENU)
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

    public void OpenGameMenu()
    {
        journeyLength = Vector3.Distance(startMarker, endMarker);
        _gameMode = _GameState.GAMEMENU;
        _gameMenu.SetActive(true);
        OpenObjInScene(_campObj);
        _settingPanel.SetActive(false);
        _miniGameMenu.SetActive(false);
        _menuPanel.SetActive(false);
        _tutorialPanel.SetActive(false);
        _playerSoulBar.SetActive(false);
        _cookMenu.SetActive(false);
        OpenActionPanel();
    }
    GameObject _uiCanvas;

    public void SettingBeforeStart()
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

    public Player _player;

    public void LoadStartScene()
    {
        _gameMode = _GameState.START;
        Camera.main.orthographicSize = 0.8f;
        Camera.main.transform.position = new Vector3(0, 0f, Camera.main.transform.position.z);
        _cameraMainPosition = transform.position;
        ReadDataAll();
        dataPlayerLog = _json.ReadDataPlayerLog(dataPlayerLog);
        CompilePlayerLog();
        _gameMenu.SetActive(false);
        _miniGameMenu.SetActive(true);
        _playerSoulBar.SetActive(true);
        _menuPanel.SetActive(true);
        if (dataPlayerLog[0].landScene)
        {
            StartCoroutine(LoadingScene(_GameState.LAND, false));
        }
        else
        {
            StartCoroutine(LoadingScene(_GameState.CAMP, false));
        }
        
    }

    public void OpenActionPanel(GameObject obj = null)
    {
        _itemPanel.SetActive(obj == _itemPanel ? true : false);
        _CharacterPanel.SetActive(obj == _CharacterPanel ? true : false);
        _attackPanel.SetActive(obj == _attackPanel ? true : false);
        _questionPanel.SetActive(obj == _questionPanel ? true : false);
    }

    void ReadDataAll()
    {
        TextAsset loadedDungeon = Resources.Load<TextAsset>("JsonDatabase/DungeonList");
        dataDungeonList = JsonHelper.FromJson<DungeonDataSet>(loadedDungeon.text);
        print("load " + dataDungeonList.Length + " dungeons.");

        TextAsset loadedMonster = Resources.Load<TextAsset>("JsonDatabase/MonsterList");
        dataMonsterList = JsonHelper.FromJson<MonsterDataList>(loadedMonster.text);
        print("load " + dataMonsterList.Length + " monsters.");

        TextAsset loadedHero = Resources.Load<TextAsset>("JsonDatabase/HeroList");
        dataHeroList = JsonHelper.FromJson<ModelDataSet>(loadedHero.text);
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
        _player.name = dataPlayerLog[SaveNum].playerName;
        _player.currentSoul = dataPlayerLog[SaveNum].soul;
        Debug.Log("complie dungeonFloor " + dataPlayerLog[SaveNum].dungeonFloor);
        _player.currentDungeonFloor = dataPlayerLog[SaveNum].dungeonFloor;
        _player.currentRoomPosition = dataPlayerLog[SaveNum].roomPosition;
        _player.currentMoney = dataPlayerLog[SaveNum].money;
        ///------load itemstore------
        if (dataPlayerLog[0].itemStore != "")
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
            Debug.Log("foreach row " + rowReal);
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
            Debug.Log("do while row" + row);
        }
        
        ///-------end load itemstore---------------

        ///-------load herostore ------------
        _heroStore = new List<Hero>();
        string[] heroStore = dataPlayerLog[0].heroStore.Split(',');
        for (int slot = 0; slot < heroStore.Length; slot++)
        {
            string[] teamData = heroStore[slot].Split(':');
            
            int heroId = _cal.IntParseFast(teamData[1]);
            foreach (ModelDataSet data in dataHeroList)
            {
                if (heroId == data.id)
                {
                    Hero hero = new Hero(_cal.IntParseFast(teamData[0]),slot, double.Parse(teamData[2]),data);
                    //_status.hp = _status.hpMax;//for test 'set hp full max'
                    //Debug.Log("before " + data.baseSTR + " " + data.baseAGI + " " + data.baseINT + " after" + _status.STR + " " + _status.AGI + " " + _status.INT);
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
                                hero.getStatus().attack[a] = attack;
                                break;
                            }
                        }
                    }
                    hero.getStatus().passive = (_Passive)data.passiveId;
                    _heroStore.Add(hero);
                    break;
                }
            }

        }

        ///-----end load herostore-----
        ///-----load playerAvatar-----
        foreach (Hero hero in _heroStore)
        {
            if (hero.getStoreId() == dataPlayerLog[SaveNum].heroIsPlay)
            {
                _heroIsPlaying = hero;
                break;
            }
        }

        ///-----end load playerAvatar-----
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
        for (int i = 0; i < shopList.Length; i++)
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

    public IEnumerator SavePlayerData(bool ExitGame = false)
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

    IEnumerator LoadingScene(_GameState mode, bool save = true)
    {
        _gameMode = mode;
        FadeIn();
        yield return new WaitForSeconds(3);
        ParticleSystem ps = _environment[0].GetComponent<ParticleSystem>();
        var coll = ps.collision;
        coll.enabled = false;
        switch (mode)
        {
            case _GameState.BATTLE:
                OpenBattleScene();
                coll.enabled = true;
                break;
            case _GameState.CAMP:
                OpenCampScene();
                break;
            case _GameState.MAP:
                OpenMapScene();
                break;
            case _GameState.LAND:
                OpenLandScene();
                break;
            case _GameState.FORESTSHOP:
                OpenForestShopScene();
                break;
        }
        //yield return new WaitForSeconds(1);
        if (save)
            _json.WriteDataPlayerLog(dataPlayerLog);
        FadeOut();
        
        if (_loadNewGame)
        {
            /*
            _cutscene = Instantiate(_cutscenePanel);
            _cutscene.transform.SetParent(_gameMenu.transform.parent.gameObject.transform);
            _cutscene.transform.localPosition = new Vector3(0, 0, 0);
            _cutscene.transform.localScale = new Vector3(1, 1, 1);
            */
            _loadNewGame = false;
        }
    }
    
    public void OpenSubMenu(_SubMenu mode)
    {
        OpenSubMenu(mode, "");
    }
    
    public void OpenSubMenu(_SubMenu mode, string topic)
    {
        _subMenuMode = mode;
        _subMenuPanel.SetActive(true);
        _subMenuPanel.GetComponent<SubMenuPanel>().setTopic(topic);
    }

    /// <summary>
    /// Event Button Zone....
    /// </summary>
    /// 
    
    public void StartBtn()
    {
        LoadStartScene();
    }

    
    public void ExitGameBtn()
    {
        Application.Quit();
    }
    

    /// End Event button zone.
    /// 
    public _ConfirmNotify _confirmNotifyMode;

    public void OpenConfirmNotify(string txt, _ConfirmNotify mode)
    {
        _confirmNotifyMode = mode;
        _confirmNotify.SetActive(true);
        _confirmNotify.transform.Find("Text").GetComponent<Text>().text = txt;
    }

    public void DeleteSave(string fileName = "PlayerLog.json")
    {
        string folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/FileSave/";
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

    public void LoadScene(_GameState mode)
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
        transform.position = _cameraMainPosition;
        
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
        //Camera.main.transform.position = new Vector3(-10, 0f, Camera.main.transform.position.z);
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

    Vector2 padding = new Vector2(20, 20);

    public void SetTalk(string txt)
    {
        Text talk = _talkPanel.GetComponentInChildren<Text>();
        talk.text = txt;
        RectTransform parentRect = _talkPanel.GetComponent<RectTransform>();

        Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y - 200));
        newPos.z = _talkPanel.transform.position.z;
        _talkPanel.transform.position = newPos;

        if (_CharacterPanel.activeSelf)
        {
            parentRect.sizeDelta = new Vector2(Screen.width, 260);
        }
        else
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


    public bool CheckCrystal(int point)
    {
        if (_gameMode != _GameState.BATTLE) return true;
        if (_battleCon.Crystal - point < 0)
        {
            return false;
        }
        return true;
    }

    public bool UseCrystal(int point)
    {
        if (_gameMode != _GameState.BATTLE) return true;
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
        foreach (Room room in _dungeon[_player.currentDungeonFloor - 1].roomIsPass)
        {
            if (room.id == _player.currentRoomPosition)
            {
                room.escapeCount++;
                break;
            }
        }
    }
    
    public PopupText _unlockNotifyPopup;

    

    public void OpenErrorNotify(string txt)
    {
        StartCoroutine(ShowNotify(txt, false));
    }

    public void OpenTrueNotify(string txt)
    {
        StartCoroutine(ShowNotify(txt, true));
    }

    IEnumerator ShowNotify(string txt, bool icon)
    {
        yield return new WaitForSeconds(0.1f);
        PopupText unlock = Instantiate(_unlockNotifyPopup);
        unlock.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);
        unlock.transform.localScale = new Vector3(1, 1, 1);
        if (icon)
        {
            unlock.transform.Find("Panel").Find("Icon").GetComponent<Image>().sprite = _uiSprite2.Single(s => s.name == "confirm");
        }
        else
        {
            unlock.transform.Find("Panel").Find("Icon").GetComponent<Image>().sprite = _uiSprite2.Single(s => s.name == "cencel");
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
        foreach (GameObject obj in _campAvatar)
        {
            obj.GetComponent<CapsuleCollider2D>().enabled = set;
        }
    }

    public bool CheckObjFromTag(string tag)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        }
#endif
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
        if (hit.transform != null && hit.transform.tag == tag)
        {
            Debug.Log("hit " + hit.transform.gameObject.name);
            return true;
        }
        return false;
    }
}



