using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Json;
using System.Linq;
using model;
using controller;
using player;
using system;

public class GameCore : MonoBehaviour
{
    JsonReadWrite _json;

    public _GameState _gameMode;
    public _ActionState _actionMode;

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
    public GameObject _mapSpace;
    public GameObject _battleSpace;
    public GameObject _campSpace;
    public GameObject _landSpace;
    public GameObject _SecretShopSpace;

    ///---Panel Zone-----
    public GameObject _startMenu;
    public GameObject _settingPanel;
    public GameObject _menuPanel;
    public GameObject _subMenuPanel;
    public GameObject _playerLifePanel;
    
    public GameObject _talkPanel;
    public GameObject _infoPanel;
    public GameObject _monTalkPanel;
    
    public GameObject _confirmNotify;

    /// -----End Panel Zone

    ///-----general----
    public GameObject _loadingScreen;
    public GameObject _loadingNotify;
    
    public GameObject _playerSoulBar;
    ///------ End import object by scene-----------

    public Dungeon[] _dungeon;
    public List<ItemStore> _itemStore;
    public List<Hero> _heroStore;
    public List<ItemShop> _landShopList;
    
    Image _loadingScreenImg;

    
    Vector3 _cameraMainPosition;
    Vector3 startMarker;
    Vector3 endMarker;
    float speed = 0.01f;
    private float startTime;
    private float journeyLength;
    bool moveLeft = false;


    public string[] _passiveDatail;
    public bool _loadNewGame = false;
    public GameObject[] _environment;
    public bool IsPaused = false;
    public bool IsPauseLock = false;


    public Sprite[] _uiSprite1;
    public Sprite[] _uiSprite2;
    public Sprite[] _mapSprite;

    GameObject _UI;

    public Player _player;


    private void Awake()
    {
        Caching.ClearCache();
        _uiSprite1 = Resources.LoadAll<Sprite>("Sprites/UI/ui");
        _uiSprite2 = Resources.LoadAll<Sprite>("Sprites/UI/ui2");
        _mapSprite = Resources.LoadAll<Sprite>("Sprites/UI/map2");
        _UI = GameObject.Find("UICanvas");
        _passiveDatail = new string[]{"passive detail" };

        _player = new Player();
        _json = new JsonReadWrite();
        dataSetting = _json.ReadSetting(dataSetting);
        AudioListener.volume = dataSetting[0].soundValue;
        _loadingScreenImg = _loadingScreen.GetComponent<Image>();

        SettingBeforeStart();
        OpenStartMenu();

    }

    void Start()
    {
    }
    
    public BattleController getBattCon()
    {
        return _battleSpace.GetComponent<BattleController>();
    }
    public MapController getMapCon()
    {
        return _mapSpace.GetComponent<MapController>();
    }
    public AttackController getATKCon()
    {
        return getMenuCon()._attackMenu.GetComponent<AttackController>();
    }
    public MenuController getMenuCon()
    {
        return _menuPanel.GetComponent<MenuController>();
    }

    public ItemController getItemCon()
    {
        return getMenuCon().gameObject.GetComponent<ItemController>();
    }

    public ShopController getShopCon()
    {
        return getMenuCon()._shopMenu.GetComponent<ShopController>();
    }

    public CampController getCampCon()
    {
        return _campSpace.GetComponent<CampController>();
    }

    public CookingController getCookCon()
    {
        return getMenuCon()._cookMenu.GetComponent<CookingController>();
    }

    public FarmingController getFarmCon()
    {
        return getMenuCon()._farmMenu.GetComponent<FarmingController>();
    }

    public SubMenuPanel getSubMenuCore()
    {
        return _subMenuPanel.GetComponent<SubMenuPanel>();
    }

    public LandController getLandCon()
    {
        return _landSpace.GetComponent<LandController>();
    }
    
    void OnGUI()
    {
        if (IsPaused)
        {
            if (IsPauseLock) return;
            IsPauseLock = true;
            GUI.Label(new Rect(0, 0, 250, 100), "Game paused");
            _UI.SetActive(false);
        }
        else
        {
            if (!IsPauseLock) return;
            IsPauseLock = false;
            _UI.SetActive(true);
        }


    }

    void OnApplicationFocus(bool hasFocus)
    {
#if (!UNITY_EDITOR)
            IsPaused = !hasFocus;
#endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
#if (!UNITY_EDITOR)
            IsPaused = pauseStatus;
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
    
    public void OpenStartMenu()
    {
        journeyLength = Vector3.Distance(startMarker, endMarker);
        _gameMode = _GameState.GAMEMENU;
        _startMenu.SetActive(true);
        OpenObjInScene(_campSpace);
        _settingPanel.SetActive(false);
        _menuPanel.SetActive(false);
        _playerSoulBar.SetActive(false);
    }

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
    
    public void OpenStartScene()
    {
        _gameMode = _GameState.START;
        Camera.main.orthographicSize = 0.8f;
        Camera.main.transform.position = new Vector3(0, 0f, Camera.main.transform.position.z);
        _cameraMainPosition = transform.position;
        ReadDataAll();
        dataPlayerLog = _json.ReadDataPlayerLog(dataPlayerLog);
        CompilePlayerLog();
        _startMenu.SetActive(false);
        _playerSoulBar.SetActive(true);
        _menuPanel.SetActive(true);
        if (dataPlayerLog[0].landScene)
        {
            OpenScene(_GameState.LAND, false);
        }
        else
        {
            OpenScene(_GameState.CAMP, false);
        }
        
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
        _player.currentStayDunBlock = dataPlayerLog[SaveNum].stayDungeonBlock;
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
                    if (Calculator.IntParseFast(itemData[1]) == data.id)
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
                if (dataItemList[dataCount].id == Calculator.IntParseFast(itemData[1]))
                {
                    //Debug.Log("add item" + row);
                    ItemStore item = new ItemStore();
                    item.id = Calculator.IntParseFast(itemData[0]);
                    item.itemId = dataItemList[dataCount].id;
                    item.amount = Calculator.IntParseFast(itemData[2]);
                    item.data = dataItemList[dataCount];
                    _itemStore.Add(item);
                    itemCount++;
                }
                else
                {
                    if (dataItemList[dataCount].id >= Calculator.IntParseFast(itemData[1]))
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
        CreateHeroFromData(heroStore);

        ///-----end load herostore-----
        ///-----load playerAvatar-----
        foreach (Hero hero in _heroStore)
        {
            if (hero.getStoreId() == dataPlayerLog[SaveNum].heroIsPlaying)
            {
                _player._heroIsPlaying = hero;
                getCampCon().LoadCampAvatar();
                Debug.Log("current hp "+_player._heroIsPlaying.getStatus().currentHP);
                break;
            }
        }

        ///-----end load playerAvatar-----
        ///-----load dungeonClear-----
        _dungeon = new Dungeon[dataDungeonList.Length];
        for (int i = 0; i < dataDungeonList.Length; i++)
        {
            Dungeon dun = new Dungeon();
            dun.data = dataDungeonList[i];
            _dungeon[i] = dun;
        }

        string[] floorData = dataPlayerLog[SaveNum].floorIsPlayed.Split(',');
        for (int i = 0; i < floorData.Length; i++)
        {
            string[] floor = floorData[i].Split('_');
            string[] block = floor[1].Split(':');
            for (int a = 0; a < block.Length; a++)
            {
                string[] blockData = block[a].Split('-');
                DungeonBlock newBlock = new DungeonBlock(Calculator.IntParseFast(blockData[0]), Calculator.IntParseFast(blockData[1]), Calculator.IntParseFast(blockData[2]));
                _dungeon[Calculator.IntParseFast(floor[0]) - 1].blockIsPlayed.Add(newBlock);
            }

        }
        ///-----End load dungeonClear-----
        string[] shopList = dataPlayerLog[SaveNum].shopList.Split(',');
        _landShopList = new List<ItemShop>();
        for (int i = 0; i < shopList.Length; i++)
        {
            string[] shopCut = shopList[i].Split(':');
            ItemShop newShop = new ItemShop();
            newShop.id = Calculator.IntParseFast(shopCut[0]);
            newShop.buyCount = Calculator.IntParseFast(shopCut[1]);
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

    void CreateHeroFromData(string[] heroStore)
    {
        for (int slot = 0; slot < heroStore.Length; slot++)
        {
            string[] teamData = heroStore[slot].Split(':');

            int heroId = Calculator.IntParseFast(teamData[1]);
            foreach (ModelDataSet data in dataHeroList)
            {
                if (heroId == data.id)
                {
                    Hero hero = new Hero(Calculator.IntParseFast(teamData[0]), slot, double.Parse(teamData[2]), data);
                    string[] skillList = data.skillList.Split(':');
                    for (int a = 0; a < skillList.Length; a++)
                    {
                        foreach (SkillDataSet skillData in dataSkillList)
                        {
                            if (Calculator.IntParseFast(skillList[a]) == skillData.id)
                            {
                                Skill attack = new Skill();
                                attack.hate = (int)skillData.bonusDmg * 20;
                                attack.data = skillData;
                                hero.getStatus().attack[a] = attack;
                                break;
                            }
                        }
                    }
                    hero.getStatus().passive = (_Passive)data.passiveId;
                    Debug.Log("atk " + hero.getStatus().getATK());
                    Debug.Log("current hp " + hero.getStatus().currentHP);
                    _heroStore.Add(hero);
                    break;
                }
            }

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

    IEnumerator LoadingScene(_GameState mode, bool savegame)
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
            case _GameState.SECRETSHOP:
                OpenSecretShopScene();
                break;
        }
        //yield return new WaitForSeconds(1);
        if (savegame)
            _json.WriteDataPlayerLog(dataPlayerLog);
        FadeOut();
        
        if (_loadNewGame)
        {
            _loadNewGame = false;
        }
    }
    
    /// <summary>
    /// Event Button Zone....
    /// </summary>
    /// 
    
    public void StartBtn()
    {
        OpenStartScene();
    }

    public void SettingBtn()
    {
        _settingPanel.SetActive(true);
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

    public void OpenScene(_GameState mode,bool savegame=true)
    {
        StartCoroutine(LoadingScene(mode, savegame));
    }

    void OpenMapScene()
    {
        dataPlayerLog[0].landScene = false;
        OpenObjInScene(_mapSpace);
    }

    void OpenBattleScene()
    {
        OpenObjInScene(_battleSpace);
        getMenuCon().OpenBattleMenu();
        transform.position = _cameraMainPosition;
    }

    void OpenCampScene()
    {
        OpenObjInScene(_campSpace);
        transform.position = _cameraMainPosition;
        
    }

    public void OpenLandScene()
    {
        dataPlayerLog[0].landScene = true;
        OpenObjInScene(_landSpace);
        transform.position = _cameraMainPosition;
    }

    void OpenSecretShopScene()
    {
        OpenObjInScene(_SecretShopSpace);
        getShopCon().MoveCameraToSecretShop();
        getMenuCon().gridViewTrans.Find("ShopButton").gameObject.SetActive(true);
    }

    void OpenObjInScene(GameObject obj)
    {
        _SecretShopSpace.SetActive(obj == _SecretShopSpace ? true : false);
        _landSpace.SetActive(obj == _landSpace ? true : false);
        if (_landSpace.activeSelf)
        {
            _campSpace.SetActive(true);
        }
        else
        {
            _campSpace.SetActive(obj == _campSpace ? true : false);
        }
        _battleSpace.SetActive(obj == _battleSpace ? true : false);
        _mapSpace.SetActive(obj == _mapSpace ? true : false);

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

        if (getMenuCon()._playerInfoPanel.activeSelf)
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
        getBattCon().timeLeft = 3;
        _monTalkPanel.SetActive(true);
    }
    
    public GameObject _unlockNotifyPopup;
    
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
        GameObject unlock = Instantiate(_unlockNotifyPopup);
        unlock.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);
        unlock.transform.localScale = new Vector3(1, 1, 1);
        Sprite img;
        if (icon)
        {
            img = _uiSprite2.Single(s => s.name == "confirm");
        }
        else
        {
            img = _uiSprite2.Single(s => s.name == "cancel");
        }
        unlock.transform.GetChild(0).Find("Icon").GetComponent<Image>().sprite = img;
        unlock.GetComponent<PopupText>().SetPopupText(txt);
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
    
}



