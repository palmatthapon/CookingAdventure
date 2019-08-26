using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Linq;
using model;

namespace controller
{
    public class MapController : MonoBehaviour {

        GameCore _core;
        Calculate _cal;

        public GameObject _teamIconSlot;
        public GameObject _monsterSlot;
        public GameObject _roomSlot,_blackFog;
        public Sprite[] _roomIcon;
        public GameObject _warpBtn, _bossRoomBtn,_campBtn;

        Vector2 StartPosition;
        Vector2 DragStartPosition;
        Vector2 DragNewPosition;
        Vector2 Finger0Position;
        float DistanceBetweenFingers;
        bool isZooming;
        bool _touchRoom = false;
        GameObject _roomTarget;
        public float speed = 0.005F;
        Vector3 delta = Vector3.zero;
        Vector3 lastPos = Vector3.zero;
        public GameObject[] _roomList;
        public GameObject _roomLoad;
        Dungeon _dungeon;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = _core._cal;
            speed = 0.005F;
            mapSize = _core._mapObj.GetComponent<SpriteRenderer>().bounds.size;
            mapPos = _core._mapObj.transform.position;
        }
        

        void OnEnable() {
            Camera.main.orthographicSize = 0.9f;
            _core._menuCon.setIconMapBtn("mapOpen");
            LoadData();
            SetPanel(true);
            LoadMap();
            //_core.LoadScene(_GameState.FORESTSHOP);
        }

        void LoadData()
        {
            Debug.Log("Dungeon Floor "+_core._player.currentDungeonFloor);
            _dungeon = _core._dungeon[_core._player.currentDungeonFloor - 1];
            
        }
        
        void Update () {
            if (!_core.isPaused)
            {
                if (_core._gameMode == _GameState.MAP && !_core._subMenuPanel.activeSelf)
                {
                    CameraMove();
                    CheckTouchRoom();
                    //CameraZoom(); //w8 modify
                }
            }
            if (_roomLoad != null)
            {
                if (_core.isPaused)
                    _roomLoad.transform.Find("LightMask").GetComponent<MaskLight>().enabled = false;
            }
                
        }

        void SetPanel(bool set)
        {
            _blackFog.SetActive(set);
            _core._mapPanel.transform.Find("DunNameText").GetComponent<Text>().text = _dungeon.dungeon.name;
            _core._mapPanel.SetActive(set);
            _campBtn.SetActive(set);

        }
        
        Vector2 mapSize;
        Vector2 roomSize;
        Vector3 mapPos;
        public int _dungeonLayerIsLoad;
        int sizeX;
        int sizeY;
        int sizeMax;

        int startX;
        int startY;

        void LoadMap()
        {
            if (_dungeonLayerIsLoad != _core._player.currentDungeonFloor || _core._mapObj.transform.childCount == 0)
            {
                _dungeonLayerIsLoad = _core._player.currentDungeonFloor;
                
                foreach (Transform child in _core._mapObj.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                
                string sizeStr = _dungeon.dungeon.size;
                string[] size = sizeStr.Split(':');
                sizeX = _cal.IntParseFast(size[0]);
                sizeY = _cal.IntParseFast(size[1]);
                sizeMax = sizeX * sizeY;

                startX = _dungeon.dungeon.startRoom % sizeX;
                startY = _dungeon.dungeon.startRoom / sizeX + 1;


                LoadMonsterInDungeonLayer();

                _roomList = new GameObject[sizeMax];

                for (int row = 0; row < sizeY; row++)
                {
                    for (int col = 0; col < sizeX; col++)
                    {
                        GameObject room = Instantiate(_roomSlot, new Vector3(0, 0, 0), Quaternion.identity);
                        int pos = (row * sizeX) + col;
                        room.name = pos.ToString();
                        room.transform.SetParent(_core._mapObj.transform);
                        room.transform.localScale = new Vector2(1, 1);
                        roomSize = room.GetComponent<SpriteRenderer>().bounds.size;
                        room.transform.position = new Vector3(roomSize.x / (float)2 + mapPos.x - mapSize.x / (float)2 + ((float)col * (mapSize.x / (float)19) * (float)2), -roomSize.y / (float)2 + mapPos.y + mapSize.y / (float)2 - ((float)row * (mapSize.y / (float)19) * (float)2), mapPos.z);
                        room.GetComponent<SpriteRenderer>().sprite = SetRoomIconBlack(pos);
                        room.transform.Find("LightMask").gameObject.SetActive(false);
                        _roomList[pos] = room;
                        
                    }
                }
                foreach (Room room in _dungeon.roomIsPass)
                {
                    if (room.id == _dungeon.dungeon.startRoom)
                    {
                        SetRoomIconWhite(room.id, _roomList[room.id], 's');
                    }
                    else if (room.id == _dungeon.dungeon.bossRoom)
                    {
                        SetRoomIconWhite(room.id, _roomList[room.id], 'b');
                    }
                    else
                    {
                        SetRoomIconWhite(room.id, _roomList[room.id], 'w');
                    }

                }
            }
            Navigate(_core._player.currentRoomPosition, true);

            _roomList[_core._player.currentRoomPosition].transform.Find("LightMask").gameObject.SetActive(true);
            _roomLoad = _roomList[_core._player.currentRoomPosition];
            _warpBtn.SetActive(_core._player.currentRoomPosition == _dungeon.dungeon.startRoom ? true : false);
            _bossRoomBtn.SetActive(_core._player.currentRoomPosition == _dungeon.dungeon.bossRoom ? true : false);
            ChangePlayerIconMap(_core._player.currentRoomPosition);
            FocusPosition();
        }

        List<Monster> _bossInCurrentDungeonLayer;
        List<Monster> _monsterInCurrentDungeonLayer;

        void LoadMonsterInDungeonLayer()
        {
            _bossInCurrentDungeonLayer = new List<Monster>();
            _monsterInCurrentDungeonLayer = new List<Monster>();

            foreach (MonsterDataList dataList in _core.dataMonsterList)
            {
                if (dataList.id == _dungeon.dungeon.monsterSetId)
                {
                    string[] monsList = dataList.name.Split(',');
                    string[] type = dataList.type.Split(',');
                    string[] baseSTR = dataList.baseSTR.Split(',');
                    string[] baseAGI = dataList.baseAGI.Split(',');
                    string[] baseINT = dataList.baseINT.Split(',');
                    string[] skillLoad = dataList.skillList.Split(',');
                    string[] patternAtkLoad = dataList.patternAttack.Split(',');


                    for (int id = 0; id < monsList.Length; id++)
                    {
                        ModelDataSet dataNew = new ModelDataSet();
                        dataNew.name = monsList[id];
                        dataNew.spriteSet = dataList.spriteSet;
                        dataNew.spriteName = dataList.spriteSet + "_" + id;
                        
                        dataNew.baseSTR = _cal.IntParseFast(baseSTR[id]);
                        dataNew.baseAGI = _cal.IntParseFast(baseAGI[id]);
                        dataNew.baseINT = _cal.IntParseFast(baseINT[id]);

                        Monster monster = new Monster(id, _dungeon.dungeon.levelMin, dataNew);

                        string[] skillList = skillLoad[id].Split(':');
                        for (int c = 0; c < skillList.Length; c++)
                        {
                            foreach (SkillDataSet skill in _core.dataSkillList)
                            {
                                if (_cal.IntParseFast(skillList[c]) == skill.id)
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
                                        dataBuff.whoUse = _Model.MONSTER;
                                        dataBuff.forMe = (_cal.IntParseFast(buff[3]) == 0) ? true : false;
                                        attack.buff.Add(dataBuff);
                                    }
                                    attack.hate = (int)skill.bonusDmg * 20;
                                    attack.skill = skill;
                                    monster.getStatus().attack[c] = attack;
                                    break;
                                }
                            }
                        }
                        
                        monster.patternAttack = patternAtkLoad;
                        _monsterInCurrentDungeonLayer.Add(monster);
                    }
                    break;
                }

            }

            float bonusBoss = 1;

            string[] getBoss = _dungeon.dungeon.bossListId.Split(',');

            for (int i = 0; i < getBoss.Length; i++)
            {
                string[] bossSet = getBoss[i].Split(':');

                foreach (ModelDataSet boss in _core.dataHeroList)
                {
                    if (boss.id != _cal.IntParseFast(bossSet[0])) continue;
                    Monster bossNew = new Monster(i, _cal.IntParseFast(bossSet[1]), boss);

                    float[] statusBonus = new float[3];
                    for (int p = 1; p <= _roomPassCount; p++)
                    {
                        int ranStatus = Random.Range(0, 3);
                        statusBonus[ranStatus] = statusBonus[ranStatus] + 0.5f;
                    }
                    for (int p = 1; p <= _roomEscapeCount; p++)
                    {
                        int ranStatus = Random.Range(0, 3);
                        statusBonus[ranStatus] = statusBonus[ranStatus] + 2.5f;
                    }

                    string[] attackp = boss.patternAttack.Split(',');
                    bossNew.patternAttack = attackp;
                    _bossInCurrentDungeonLayer.Add(bossNew);
                    break;
                }
                
            }
            bonusBoss = 2f;
        }

        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void ChangePlayerIconMap(int pos,int posNew=-1)
        {

            if (getSpriteSet != _core._heroIsPlaying.getSpriteSet())
            {
                getSpriteSet = _core._heroIsPlaying.getSpriteSet();
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            if (posNew == -1)
            {
                _roomList[pos].transform.Find("Player").gameObject.SetActive(true);
                _roomList[pos].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Map_"+ _core._heroIsPlaying.getSpriteName());
            }
            else
            {
                if(pos == _dungeon.dungeon.startRoom)
                {
                    _roomList[pos].transform.Find("Player").gameObject.SetActive(true);
                    _roomList[pos].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _roomIcon[0];
                }
                else if (pos == _dungeon.dungeon.bossRoom)
                {
                    _roomList[pos].transform.Find("Player").gameObject.SetActive(true);
                    _roomList[pos].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _roomIcon[1];
                }
                else
                {
                    _roomList[pos].transform.Find("Player").gameObject.SetActive(false);
                }
                _roomList[pos].transform.Find("LightMask").gameObject.SetActive(false);
                _roomList[posNew].transform.Find("Player").gameObject.SetActive(true);
                _roomList[posNew].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Map_" + _core._heroIsPlaying.getSpriteName());
                _roomList[posNew].transform.Find("LightMask").gameObject.SetActive(true);
            }
            
        }

        public void FocusPosition()
        {
            Camera.main.transform.position = new Vector3(_roomList[_core._player.currentRoomPosition].transform.position.x,
                    _roomList[_core._player.currentRoomPosition].transform.position.y,
                    Camera.main.transform.position.z);
        }

        Sprite SetRoomIconBlack(int pos)
        {
            if (pos == 0)
            {
                return _core._mapSprite.Single(s => s.name == "map2_1");
            }
            else if (pos == sizeX-1)
            {
                return _core._mapSprite.Single(s => s.name == "map2_3");
            }
            else if (pos == sizeMax - sizeX)
            {
                return _core._mapSprite.Single(s => s.name == "map2_7");
            }
            else if (pos == sizeMax - 1)
            {
                return _core._mapSprite.Single(s => s.name == "map2_9");
            }
            else if (pos < sizeX)
            {
                return _core._mapSprite.Single(s => s.name == "map2_2");
            }
            else if (pos % sizeX ==0)
            {
                return _core._mapSprite.Single(s => s.name == "map2_4");
            }
            else if ((pos+1)%sizeX == 0)
            {
                return _core._mapSprite.Single(s => s.name == "map2_6");
            }
            else if (pos > sizeMax - sizeX)
            {
                return _core._mapSprite.Single(s => s.name == "map2_8");
            }
            return _core._mapSprite.Single(s => s.name == "map2_5");
        }

        void SetRoomIconWhite(int pos,GameObject room,char name)
        {
            Sprite roomIcon;
            if (pos == 0)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_10");
            }
            else if (pos == sizeX - 1)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_12");
            }
            else if (pos == sizeMax - sizeX)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_16");
            }
            else if (pos == sizeMax - 1)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_18");
            }
            else if (pos < sizeX)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_11");
            }
            else if (pos % sizeX == 0)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_13");
            }
            else if ((pos + 1) % sizeX == 0)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_15");
            }
            else if (pos > sizeMax - sizeX)
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_17");
            }
            else
            {
                roomIcon = _core._mapSprite.Single(s => s.name == "map2_14");
            }
            room.GetComponent<SpriteRenderer>().sprite = roomIcon;
            room.GetComponent<SpriteRenderer>().sortingOrder = 1;

            if (name == 's')
            {
                _roomList[pos].transform.Find("Player").gameObject.SetActive(true);
                _roomList[pos].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _roomIcon[0];
            }
            else if (name == 'b')
            {
                _roomList[pos].transform.Find("Player").gameObject.SetActive(true);
                _roomList[pos].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = _roomIcon[1];
            }
            
        }


        public void CameraMove()
        {

#if UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                Camera.main.transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
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
                Camera.main.transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
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

        void CheckTouchRoom()
        {
            //-----touch collider2d room-----------
            secondsCount += Time.deltaTime;

            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (!_touchRoom)
                {
                    FindRoom();
                    lastTimeClick = secondsCount;
                }
            }
            else
            {
                _touchRoom = false;
            }
        }
        int _roomPassCount;
        int _roomEscapeCount;
        string _firstTouchRoom;


        void FindRoom()
        {
            //LayerMask layerMask = 1 << 8;
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Room");
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
                _touchRoom = true;
                
                if (_firstTouchRoom == hit.collider.gameObject.name)
                {
                    if (Mathf.Abs(secondsCount - lastTimeClick) < 0.75f)
                    {
                        int newPosition = _cal.IntParseFast(hit.collider.gameObject.name);
                        if (newPosition == _core._player.currentRoomPosition + 1 || newPosition == _core._player.currentRoomPosition - 1 || newPosition == _core._player.currentRoomPosition + sizeX || newPosition == _core._player.currentRoomPosition - sizeY)
                        {
                            _roomList[_core._player.currentRoomPosition].transform.Find("LightMask").gameObject.SetActive(false);
                            if (_core._player.currentRoomPosition == _dungeon.dungeon.startRoom)
                            {
                                SetRoomIconWhite(_core._player.currentRoomPosition, _roomList[_core._player.currentRoomPosition], 's');
                            }
                            else if (_core._player.currentRoomPosition == _dungeon.dungeon.bossRoom)
                            {
                                SetRoomIconWhite(_core._player.currentRoomPosition, _roomList[_core._player.currentRoomPosition], 'b');
                            }
                            else
                            {
                                SetRoomIconWhite(_core._player.currentRoomPosition, _roomList[_core._player.currentRoomPosition], 'w');
                            }
                            SetRoomIconWhite(newPosition, _roomList[newPosition], 'w');
                            ChangePlayerIconMap(_core._player.currentRoomPosition, newPosition);

                            Navigate(_core._player.currentRoomPosition, false);
                            _core._player.currentRoomPosition = newPosition;
                            Navigate(_core._player.currentRoomPosition, true);

                            _roomLoad = _roomList[_core._player.currentRoomPosition];


                            if (newPosition == _dungeon.dungeon.startRoom)
                            {
                                _warpBtn.SetActive(true);
                            }
                            else
                            {
                                bool roomPass = false;
                                foreach (Room room in _dungeon.roomIsPass)
                                {
                                    if (room.id == newPosition)
                                    {
                                        _roomPassCount = room.passCount;
                                        _roomEscapeCount = room.escapeCount;
                                        roomPass = true;
                                        break;
                                    }
                                }

                                if (roomPass == false)
                                {
                                    Room newRoom = new Room();
                                    newRoom.id = _core._player.currentRoomPosition;
                                    newRoom.passCount = 0;
                                    newRoom.escapeCount = 0;
                                    _dungeon.roomIsPass.Add(newRoom);
                                }
                                _warpBtn.SetActive(false);
                                _bossRoomBtn.SetActive(_core._player.currentRoomPosition == _dungeon.dungeon.bossRoom ? true : false);
                                int shoprate = 30;
                                if (_core._cutscene != null)
                                {
                                    LoadMonsterInRoom();

                                }
                                else
                                {
                                    float shopRan = Random.Range(0, 100);
                                    Debug.Log(shopRan + "<=" + shoprate + "mod " + shopRan % 2);
                                    if (newPosition != _dungeon.dungeon.bossRoom && shopRan <= shoprate && shopRan%2 ==0)
                                    {
                                        _core.LoadScene(_GameState.FORESTSHOP);
                                    }
                                    else
                                    {
                                        LoadMonsterInRoom();
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        _firstTouchRoom = "";
                    }
                }
                else
                {
                    _firstTouchRoom = hit.collider.gameObject.name;
                }
            }
        }

        void Navigate(int pos,bool open)
        {
            if (pos + 1 < sizeMax && pos % sizeX != 2)
            {
                _roomList[pos + 1].transform.Find("ParticleRoom").gameObject.SetActive(open);
            }
            if (pos - 1 >= 0 && pos % sizeX != 0)
                _roomList[pos - 1].transform.Find("ParticleRoom").gameObject.SetActive(open);
            if (pos + sizeX < sizeMax)
                _roomList[pos + sizeX].transform.Find("ParticleRoom").gameObject.SetActive(open);
            if (pos - sizeX >= 0)
                _roomList[pos - sizeX].transform.Find("ParticleRoom").gameObject.SetActive(open);
        }
        
        void LoadMonsterInRoom()
        {

            
            
            int posX = _core._player.currentRoomPosition % sizeX;
            int posY = _core._player.currentRoomPosition / sizeX + 1;
            
            int x = Math.Abs(posX - startX);
            int y = Math.Abs(posY - startY);
            int maxPos = (x > y ? x : y);
            
            if (_core._player.currentRoomPosition == _dungeon.dungeon.bossRoom)
            {
                _escapeRate = 0.4f;
                
            }
            else
            {

                _escapeRate = 0.5f;

                _core._currentMonsterBattle = new Monster[1 + (Random.Range(0, max: Random.Range(0f, 1f) < 0.5 ? 5 : 5 / maxPos))];

                int MaxLevel = maxPos * _dungeon.dungeon.levelMax / sizeY;
                Debug.Log("Monster have " + _core._currentMonsterBattle.Length + " lvl max " + MaxLevel + " dun lvl max " + _dungeon.dungeon.levelMax);

                for(int i=0;i< _core._currentMonsterBattle.Length; i++)
                {
                    _core._currentMonsterBattle[i] = _monsterInCurrentDungeonLayer[Random.Range(0, _monsterInCurrentDungeonLayer.Count)];
                    _core._monPanel.GetComponent<MonPanel>()._monAvatarList[i].transform.parent.localScale = new Vector3(1f, 1f, 1f);
                }
                
            }
            _core.LoadScene(_GameState.BATTLE);
        }
        
        
        int teamIsSelect;
        
        public void BossRoomBtn()
        {
            LoadMonsterInRoom();
        }
        
        public void CampBtn()
        {
            _core.LoadScene(_GameState.CAMP);
        }

        public void WarpLandBtn()
        {
            _core.LoadScene(_GameState.LAND);
        }
        
        public float _escapeRate;
        
        public void OnDisable()
        {
            if (_core._mapObj.transform.childCount != 0)
            {
                _roomList[_core._player.currentRoomPosition].transform.Find("LightMask").gameObject.SetActive(false);
                _roomList[_core._player.currentRoomPosition].transform.Find("Player").gameObject.SetActive(false);
                Navigate(_core._player.currentRoomPosition, false);
            }
            _core._menuCon._mapBtn.SetActive(true);
            SetPanel(false);
        }
    }
}