using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Linq;
using model;
using system;

namespace controller
{
    public class MapController : MonoBehaviour {

        GameCore _core;
        Calculator _cal;

        public GameObject _mapPanel;
        
        public GameObject _mapBlockSlot,_blackFog;
        public Sprite _blockWarpIcon;
        public Sprite _blockBossIcon;

        string _firstTouchBlock;
        
        public float speed = 0.005F;
        Vector3 delta = Vector3.zero;
        Vector3 lastPos = Vector3.zero;
        public DungeonBlock[] _dunBlock;
        Dungeon _dungeon;

        Vector2 mapSize;
        Vector2 blockSize;
        Vector3 mapPos;
        public int _dungeonFloorLoaded;
        int _dgColumnCount;
        int _dgRowCount;
        int _dunBlockCount;
        int startBlockX;
        int startBlockY;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = new Calculator();
            speed = 0.005F;
            mapSize = _core._mapSpace.GetComponent<SpriteRenderer>().bounds.size;
            mapPos = _core._mapSpace.transform.position;
        }
        

        void OnEnable() {
            Camera.main.orthographicSize = 0.9f;
            _core.getMenuCon().setIconMapBtn("mapOpen");
            LoadData();
            SetPanel(true);
            LoadMap();
        }

        void LoadData()
        {
            Debug.Log("Dungeon Floor "+_core._player.currentDungeonFloor);
            _dungeon = _core._dungeon[_core._player.currentDungeonFloor - 1];
            
        }
        
        void Update () {
            if (_core.IsPaused)
            {
                if (_dunBlock[_core._player.currentStayDunBlock].obj != null)
                {
                    _dunBlock[_core._player.currentStayDunBlock].obj.transform.Find("LightMask").GetComponent<MaskLight>().enabled = false;
                }
            }
            else
            {
                if (_core.IsPauseLock)
                {
                    if (_dunBlock[_core._player.currentStayDunBlock].obj != null)
                        _dunBlock[_core._player.currentStayDunBlock].obj.transform.Find("LightMask").GetComponent<MaskLight>().enabled = true;
                }
                if (_core._gameMode == _GameState.MAP && !_core._subMenuPanel.activeSelf)
                {
                    CameraMove();
                    CheckTouchBlock();
                    //CameraZoom(); //w8 modify
                }
            }
            
                
        }

        void SetPanel(bool set)
        {
            _blackFog.SetActive(set);
            _mapPanel.transform.Find("DunNameText").GetComponent<Text>().text = _dungeon.data.name;
            _mapPanel.SetActive(set);

        }
        
        void LoadMap()
        {
            if (_dungeonFloorLoaded != _core._player.currentDungeonFloor || _core._mapSpace.transform.childCount == 0)
            {
                _dungeonFloorLoaded = _core._player.currentDungeonFloor;
                
                foreach (Transform child in _core._mapSpace.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                
                string sizeStr = _dungeon.data.size;
                string[] size = sizeStr.Split(':');
                _dgColumnCount = Calculator.IntParseFast(size[0]);
                _dgRowCount = Calculator.IntParseFast(size[1]);
                _dunBlockCount = _dgColumnCount * _dgRowCount;

                startBlockX = _dungeon.data.warpBlock % _dgColumnCount;
                startBlockY = _dungeon.data.warpBlock / _dgColumnCount + 1;


                LoadAllMonsterInDungeonFloor();

                _dunBlock = new DungeonBlock[_dunBlockCount];
                
                for (int row = 0; row < _dgRowCount; row++)
                {
                    for (int col = 0; col < _dgColumnCount; col++)
                    {
                        GameObject block = Instantiate(_mapBlockSlot, new Vector3(0, 0, 0), Quaternion.identity);
                        int pos = (row * _dgColumnCount) + col;
                        _dunBlock[pos] = new DungeonBlock(pos,0,0);
                        block.name = pos.ToString();
                        block.transform.SetParent(_core._mapSpace.transform);
                        block.transform.localScale = new Vector2(1, 1);
                        blockSize = block.GetComponent<SpriteRenderer>().bounds.size;
                        block.transform.position = new Vector3(blockSize.x / (float)2 + mapPos.x - mapSize.x / (float)2 + ((float)col * (mapSize.x / (float)19) * (float)2), -blockSize.y / (float)2 + mapPos.y + mapSize.y / (float)2 - ((float)row * (mapSize.y / (float)19) * (float)2), mapPos.z);
                        block.GetComponent<SpriteRenderer>().sprite = getBlockIcon(pos,true);
                        block.transform.Find("LightMask").gameObject.SetActive(false);
                        _dunBlock[pos].obj = block;
                        
                    }
                }

                foreach (DungeonBlock block in _dungeon.blockIsPlayed)
                {
                    _dunBlock[block.getNumber()].AddPlayed(block.getPlayed());
                    _dunBlock[block.getNumber()].AddEscaped(block.getEscaped());

                    setBlockIcon(block.getNumber(), false);
                    
                    _dunBlock[block.getNumber()].obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
            }
            Navigate(_core._player.currentStayDunBlock, true);

            _dunBlock[_core._player.currentStayDunBlock].obj.transform.Find("LightMask").gameObject.SetActive(true);
            ChangePlayerIconMap(_core._player.currentStayDunBlock);
            FocusPosition();
        }
        
        List<Monster> _monsInCurrentdgFloor;
        List<Monster> _bossInCurrentdgFloor;

        void LoadAllMonsterInDungeonFloor()
        {
            _monsInCurrentdgFloor = new List<Monster>();
            _bossInCurrentdgFloor = new List<Monster>();

            CrateMonsterFromData("monster",_dungeon.data.monsterIdList.Split(','));
            CrateMonsterFromData("boss",_dungeon.data.bossIdList.Split(','));

        }
        
        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void ChangePlayerIconMap(int pos,int posNew=-1)
        {

            if (getSpriteSet != _core._player._heroIsPlaying.getSpriteSet())
            {
                getSpriteSet = _core._player._heroIsPlaying.getSpriteSet();
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            if (posNew == -1)
            {
                _dunBlock[pos].obj.transform.Find("Player").gameObject.SetActive(true);
                _dunBlock[pos].obj.transform.Find("Player").GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Map_"+ _core._player._heroIsPlaying.getSpriteName());
            }
            else
            {
                if(pos == _dungeon.data.warpBlock)
                {
                    _dunBlock[pos].obj.transform.Find("Player").gameObject.SetActive(true);
                    _dunBlock[pos].obj.transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _blockWarpIcon;
                }
                else if (pos == _dungeon.data.bossBlock)
                {
                    _dunBlock[pos].obj.transform.Find("Player").gameObject.SetActive(true);
                    _dunBlock[pos].obj.transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _blockBossIcon;
                }
                else
                {
                    _dunBlock[pos].obj.transform.Find("Player").gameObject.SetActive(false);
                }
                _dunBlock[pos].obj.transform.Find("LightMask").gameObject.SetActive(false);
                _dunBlock[posNew].obj.transform.Find("Player").gameObject.SetActive(true);
                _dunBlock[posNew].obj.transform.Find("Player").GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Map_" + _core._player._heroIsPlaying.getSpriteName());
                _dunBlock[posNew].obj.transform.Find("LightMask").gameObject.SetActive(true);
            }
            
        }

        public void FocusPosition()
        {
            Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
            this.transform.position = new Vector3(center.x - _dunBlock[_core._player.currentStayDunBlock].obj.transform.position.x+ this.transform.position.x,
                (center.y- _dunBlock[_core._player.currentStayDunBlock].obj.transform.position.y)+ this.transform.position.y, this.transform.position.z);
        }

        Sprite getBlockIcon(int pos, bool isBlack)
        {
            string icon;
            if (pos == 0)
            {
                if(isBlack)
                    icon = "map2_1";
                else
                    icon = "map2_10";
            }
            else if (pos == _dgColumnCount - 1)
            {
                if (isBlack)
                    icon = "map2_3";
                else
                    icon = "map2_12";
            }
            else if (pos == _dunBlockCount - _dgColumnCount)
            {
                if (isBlack)
                    icon = "map2_7";
                else
                    icon = "map2_16";
            }
            else if (pos == _dunBlockCount - 1)
            {
                if (isBlack)
                    icon = "map2_9";
                else
                    icon = "map2_18";
            }
            else if (pos < _dgColumnCount)
            {
                if (isBlack)
                    icon = "map2_2";
                else
                    icon = "map2_11";
            }
            else if (pos % _dgColumnCount == 0)
            {
                if (isBlack)
                    icon = "map2_4";
                else
                    icon = "map2_13";
            }
            else if ((pos+1)% _dgColumnCount == 0)
            {
                if (isBlack)
                    icon = "map2_6";
                else
                    icon = "map2_15";
            }
            else if (pos > _dunBlockCount - _dgColumnCount)
            {
                if (isBlack)
                    icon = "map2_8";
                else
                    icon = "map2_17";
            }
            else
            {
                if (isBlack)
                    icon = "map2_5";
                else
                    icon = "map2_14";
            }
            return _core._mapSprite.Single(s => s.name == icon);
        }
        
        public void CameraMove()
        {

#if UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                this.transform.Translate(touchDeltaPosition.x * speed, touchDeltaPosition.y * speed, 0);
            }
#endif
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                lastPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                delta = Input.mousePosition - lastPos;
                Vector2 touchDeltaPosition = delta;
                this.transform.Translate(touchDeltaPosition.x * speed, touchDeltaPosition.y * speed, 0);
                lastPos = Input.mousePosition;
            }
#endif
            
        }
        public float perspectiveZoomSpeed = 0.005f;        // The rate of change of the field of view in perspective mode.
        public float orthoZoomSpeed = 0.005f;

        void CameraZoom()
        {
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                // If the camera is orthographic...
                if (Camera.main.orthographic)
                {
                    // ... change the orthographic size based on the change in distance between the touches.
                    Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                    // Make sure the orthographic size never drops below zero.
                    Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
                }
                else
                {
                    // Otherwise change the field of view based on the change in distance between the touches.
                    Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                    // Clamp the field of view to make sure it's between 0 and 180.
                    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.1f, 179.9f);
                }
            }
        }
        private float secondsCount;
        float lastTimeClick;

        void CheckTouchBlock()
        {
            //-----touch collider2d room-----------
            
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (TouchBlock())
                {
                    int newPosition = Calculator.IntParseFast(_firstTouchBlock);
                    if (newPosition == _core._player.currentStayDunBlock + 1 || newPosition == _core._player.currentStayDunBlock - 1 || newPosition == _core._player.currentStayDunBlock + _dgColumnCount || newPosition == _core._player.currentStayDunBlock - _dgRowCount)
                    {
                        _dunBlock[_core._player.currentStayDunBlock].obj.transform.Find("LightMask").gameObject.SetActive(false);

                        setBlockIcon(_core._player.currentStayDunBlock, false);
                        setBlockIcon(newPosition, false);

                        ChangePlayerIconMap(_core._player.currentStayDunBlock, newPosition);

                        Navigate(_core._player.currentStayDunBlock, false);
                        _core._player.currentStayDunBlock = newPosition;
                        Navigate(_core._player.currentStayDunBlock, true);

                        if (newPosition == _dungeon.data.warpBlock)
                        {
                            _core.OpenScene(_GameState.LAND);
                        }
                        else if (newPosition == _dungeon.data.bossBlock)
                        {
                            LoadBossInBattle();
                        }
                        else
                        {
                            _dunBlock[_core._player.currentStayDunBlock].AddPlayed(1);
                            int shoprate = 30;

                            float shopRan = Random.Range(0, 100);
                            Debug.Log(shopRan + "<=" + shoprate + "mod " + shopRan % 2);
                            if (newPosition != _dungeon.data.bossBlock && shopRan <= shoprate && shopRan % 2 == 0)
                            {
                                _core.OpenScene(_GameState.SECRETSHOP);
                            }
                            else
                            {
                                LoadMonsterInBattle();
                            }

                        }
                    }

                }
            }
        }
        
        bool TouchBlock()
        {
            //LayerMask layerMask = 1 << 8;
            LayerMask layerMask = 1 << LayerMask.NameToLayer("DGBlock");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, float.PositiveInfinity, layerMask);
            if (hit.collider != null)
            {
                float currentTimeClick = Time.time;
                if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
                {
                    currentTimeClick = 0;
                    if (_firstTouchBlock == hit.collider.gameObject.name)
                    {
                        return true;
                    }
                }
                _firstTouchBlock = hit.collider.gameObject.name;
                lastTimeClick = currentTimeClick;
            }
            
            return false;
        }

        void setBlockIcon(int pos,bool isblack)
        {
            _dunBlock[pos].obj.GetComponent<SpriteRenderer>().sprite = getBlockIcon(pos, isblack);
            if (pos == _dungeon.data.warpBlock)
            {
                _dunBlock[pos].obj.transform.Find("Player").gameObject.SetActive(true);
                _dunBlock[pos].obj.transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _blockWarpIcon;
            }
            else if (pos == _dungeon.data.bossBlock)
            {
                _dunBlock[pos].obj.transform.Find("Player").gameObject.SetActive(true);
                _dunBlock[pos].obj.transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _blockBossIcon;
            }
        }

        void Navigate(int pos,bool open)
        {
            if (pos%_dgColumnCount != _dgColumnCount-1)
            {
                _dunBlock[pos + 1].obj.transform.Find("ParticleRoom").gameObject.SetActive(open);
            }
            if (pos % _dgColumnCount != 0)
                _dunBlock[pos - 1].obj.transform.Find("ParticleRoom").gameObject.SetActive(open);
            if (pos + _dgColumnCount < _dunBlockCount)
                _dunBlock[pos + _dgColumnCount].obj.transform.Find("ParticleRoom").gameObject.SetActive(open);
            if (pos - _dgColumnCount >= 0)
                _dunBlock[pos - _dgColumnCount].obj.transform.Find("ParticleRoom").gameObject.SetActive(open);
        }
        
        void LoadMonsterInBattle()
        {
            int posX = _core._player.currentStayDunBlock % _dgColumnCount;
            int posY = _core._player.currentStayDunBlock / _dgColumnCount + 1;
            
            int x = Math.Abs(posX - startBlockX);
            int y = Math.Abs(posY - startBlockY);
            int maxPos = (x > y ? x : y);
            
            if (_core._player.currentStayDunBlock == _dungeon.data.bossBlock)
            {
                _escapeRate = 0.4f;
                
            }
            else
            {

                _escapeRate = 0.5f;

                _core.getBattCon()._currentMonBatt = new Monster[1 + (Random.Range(0, max: Random.Range(0f, 1f) < 0.5 ? 5 : 5 / maxPos))];

                int MaxLevel = maxPos * _dungeon.data.levelMax / _dgRowCount;
                Debug.Log("Monster have " + _core.getBattCon()._currentMonBatt.Length + " lvl max " + MaxLevel + " dun lvl max " + _dungeon.data.levelMax);

                for(int i=0;i< _core.getBattCon()._currentMonBatt.Length; i++)
                {
                   
                    _core.getBattCon()._currentMonBatt[i] = _monsInCurrentdgFloor[Random.Range(0, _monsInCurrentdgFloor.Count)].Copy();
                }
                
            }
            _core.OpenScene(_GameState.BATTLE);
        }

        void LoadBossInBattle()
        {
                _escapeRate = 0.4f;

                _core.getBattCon()._currentMonBatt = new Monster[_bossInCurrentdgFloor.Count];
                Debug.Log("Boss have " + _core.getBattCon()._currentMonBatt.Length);

                for (int i = 0; i < _bossInCurrentdgFloor.Count; i++)
                {
                    _core.getBattCon()._currentMonBatt[i] = _bossInCurrentdgFloor[i];
                }

            
            _core.OpenScene(_GameState.BATTLE);
        }
        
        void CrateMonsterFromData(string monstype, string[] monsterList)
        {
            for (int a = 0; a < monsterList.Length; a++)
            {
                foreach (MonsterDataList dataList in _core.dataMonsterList)
                {

                    if (dataList.id == Calculator.IntParseFast(monsterList[a]))
                    {
                        string[] monsList = dataList.name.Split(',');
                        string[] type = dataList.type.Split(',');
                        string[] baseSTR = dataList.baseSTR.Split(',');
                        string[] baseAGI = dataList.baseAGI.Split(',');
                        string[] baseINT = dataList.baseINT.Split(',');
                        string[] skillLoad = dataList.skillList.Split(',');
                        string[] patternAtkLoad = dataList.patternAttack.Split(',');

                        for (int i = 0; i < monsList.Length; i++)
                        {
                            ModelDataSet dataNew = new ModelDataSet();
                            dataNew.name = monsList[i];
                            dataNew.spriteSet = dataList.spriteSet;
                            dataNew.spriteName = dataList.spriteSet + "_" + i;

                            dataNew.baseSTR = Calculator.IntParseFast(baseSTR[i]);
                            dataNew.baseAGI = Calculator.IntParseFast(baseAGI[i]);
                            dataNew.baseINT = Calculator.IntParseFast(baseINT[i]);

                            Monster monster = new Monster(_dungeon.data.levelMin, dataNew);

                            string[] skillList = skillLoad[i].Split(':');
                            for (int c = 0; c < skillList.Length; c++)
                            {
                                foreach (SkillDataSet skillData in _core.dataSkillList)
                                {
                                    if (Calculator.IntParseFast(skillList[c]) == skillData.id)
                                    {
                                        monster.getStatus().attack[c] = skillData;
                                        break;
                                    }
                                }
                            }

                            monster.patternAttack = patternAtkLoad;
                            if (monstype == "monster")
                                _monsInCurrentdgFloor.Add(monster);
                            else
                                _bossInCurrentdgFloor.Add(monster);
                        }
                        break;
                    }

                }
            }
        }
        
        public float _escapeRate;
        
        public void OnDisable()
        {
            if (_core._mapSpace.transform.childCount != 0)
            {
                _dunBlock[_core._player.currentStayDunBlock].obj.transform.Find("LightMask").gameObject.SetActive(false);
                _dunBlock[_core._player.currentStayDunBlock].obj.transform.Find("Player").gameObject.SetActive(false);
                Navigate(_core._player.currentStayDunBlock, false);
            }
            _core.getMenuCon()._mapBtn.SetActive(true);
            SetPanel(false);
        }
    }
}