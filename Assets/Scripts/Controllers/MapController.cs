using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Linq;

using UI;
using monster;
using model;

namespace controller
{
    public class MapController : MonoBehaviour {

        GameCore _core;
        Calculate _cal;
        MonPanel _monCom;

        public GameObject _teamIconSlot;
        public GameObject _monsterSlot;
        public GameObject _roomSlot,_blackFog;
        public Sprite[] _roomIcon;
        public GameObject _warpBtn, _bossRoomBtn,_campBtn;
        public Sprite[] _dunBlack,_dunWhite;


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
        public List<HeroStore> _teamList;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = _core._cal;
            _monCom = _core._monCom;
            speed = 0.005F;
            mapSize = _core._mapObj.GetComponent<SpriteRenderer>().bounds.size;
            mapPos = _core._mapObj.transform.position;
        }
        

        void OnEnable() {
            Camera.main.orthographicSize = 0.9f;
            SelectTeamAndLoadData();
            SetPanel(true);
            LoadMap();
            //_core.LoadScene(_GameStatus.FORESTSHOP);
        }

        void LoadData()
        {
            TeamIsSelect = _core._currentTeamIsSelect;
            _dungeon = _core._dungeon[_core._currentDungeonLayer - 1];
            _teamList = _core._teamSetup[_core._currentTeamIsSelect - 1].position;
            
        }
        
        void Update () {
            if (!_core.isPaused)
            {
                if (_core._gameMode == _GameStatus.MAP && !_core._subMenuPanel.activeSelf)
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

        void SelectTeamAndLoadData()
        {
            TeamIsSelect = _core._currentTeamIsSelect;
            bool haveLeader = false;
            int countError = 0;
            while (!haveLeader)
            {
                if (countError > _core._teamSetup.Count) break;
                for (int m = 0; m < 5; m++)
                {
                    if (_core._teamSetup[TeamIsSelect - 1].position[m].id != -1)
                    {
                        haveLeader = true;
                        break;
                    }
                    
                }
                if(!haveLeader)
                    TeamIsSelect++;
                countError++;
            }
            _core._currentTeamIsSelect = TeamIsSelect;
            LoadData();
        }

        Vector2 mapSize;
        Vector2 roomSize;
        Vector3 mapPos;
        public int _dungeonLayerIsLoad;
        int sizeX;
        int sizeY;
        int sizeMax;

        void LoadMap()
        {
            if (_dungeonLayerIsLoad != _core._currentDungeonLayer || _core._mapObj.transform.childCount == 0)
            {
                _dungeonLayerIsLoad = _core._currentDungeonLayer;
                foreach (Transform child in _core._mapObj.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                string sizeStr = _dungeon.dungeon.size;
                string[] size = sizeStr.Split(':');
                sizeX = _cal.IntParseFast(size[0]);
                sizeY = _cal.IntParseFast(size[1]);
                sizeMax = sizeX * sizeY;
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
            Navigate(_core._currentRoomPosition, true);

            _roomList[_core._currentRoomPosition].transform.Find("LightMask").gameObject.SetActive(true);
            _roomLoad = _roomList[_core._currentRoomPosition];
            _warpBtn.SetActive(_core._currentRoomPosition == _dungeon.dungeon.startRoom ? true : false);
            _bossRoomBtn.SetActive(_core._currentRoomPosition == _dungeon.dungeon.bossRoom ? true : false);
            ChangePlayerIconMap(_core._currentRoomPosition);
            FocusPosition();
        }
        Sprite[] loadSprite = null;
        string getSpriteSet = "";
        HeroDataSet heroLeader;

        public void ChangePlayerIconMap(int pos,int posNew=-1)
        {
            heroLeader = _core.GetLeader().hero;

            if (getSpriteSet != heroLeader.spriteSet)
            {
                getSpriteSet = heroLeader.spriteSet;
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            if (posNew == -1)
            {
                _roomList[pos].transform.Find("Player").gameObject.SetActive(true);
                _roomList[pos].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Map_"+ heroLeader.spriteName);
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
                _roomList[posNew].transform.Find("Player").GetComponent<SpriteRenderer>().sprite = loadSprite.Single(s => s.name == "Map_" + heroLeader.spriteName);
                _roomList[posNew].transform.Find("LightMask").gameObject.SetActive(true);
            }
            
        }

        public void FocusPosition()
        {
            Camera.main.transform.position = new Vector3(_roomList[_core._currentRoomPosition].transform.position.x,
                    _roomList[_core._currentRoomPosition].transform.position.y,
                    Camera.main.transform.position.z);
        }

        Sprite SetRoomIconBlack(int pos)
        {
            if (pos == 0)
            {
                return _dunBlack[0];
            }else if (pos == sizeX-1)
            {
                return _dunBlack[2];
            }
            else if (pos == sizeMax - sizeX)
            {
                return _dunBlack[6];
            }
            else if (pos == sizeMax - 1)
            {
                return _dunBlack[8];
            }
            else if (pos < sizeX)
            {
                return _dunBlack[1];
            }
            else if (pos % sizeX ==0)
            {
                return _dunBlack[3];
            }
            else if ((pos+1)%sizeX == 0)
            {
                return _dunBlack[5];
            }
            else if (pos > sizeMax - sizeX)
            {
                return _dunBlack[7];
            }
            return _dunBlack[4];
        }

        void SetRoomIconWhite(int pos,GameObject room,char name)
        {
            Sprite roomIcon;
            if (pos == 0)
            {
                roomIcon = _dunWhite[0];
            }
            else if (pos == sizeX - 1)
            {
                roomIcon = _dunWhite[2];
            }
            else if (pos == sizeMax - sizeX)
            {
                roomIcon = _dunWhite[6];
            }
            else if (pos == sizeMax - 1)
            {
                roomIcon = _dunWhite[8];
            }
            else if (pos < sizeX)
            {
                roomIcon = _dunWhite[1];
            }
            else if (pos % sizeX == 0)
            {
                roomIcon = _dunWhite[3];
            }
            else if ((pos + 1) % sizeX == 0)
            {
                roomIcon = _dunWhite[5];
            }
            else if (pos > sizeMax - sizeX)
            {
                roomIcon = _dunWhite[7];
            }
            else
            {
                roomIcon = _dunWhite[4];
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
                        if (newPosition == _core._currentRoomPosition + 1 || newPosition == _core._currentRoomPosition - 1 || newPosition == _core._currentRoomPosition + sizeX || newPosition == _core._currentRoomPosition - sizeY)
                        {
                            _roomList[_core._currentRoomPosition].transform.Find("LightMask").gameObject.SetActive(false);
                            if (_core._currentRoomPosition == _dungeon.dungeon.startRoom)
                            {
                                SetRoomIconWhite(_core._currentRoomPosition, _roomList[_core._currentRoomPosition], 's');
                            }
                            else if (_core._currentRoomPosition == _dungeon.dungeon.bossRoom)
                            {
                                SetRoomIconWhite(_core._currentRoomPosition, _roomList[_core._currentRoomPosition], 'b');
                            }
                            else
                            {
                                SetRoomIconWhite(_core._currentRoomPosition, _roomList[_core._currentRoomPosition], 'w');
                            }
                            SetRoomIconWhite(newPosition, _roomList[newPosition], 'w');
                            ChangePlayerIconMap(_core._currentRoomPosition, newPosition);

                            Navigate(_core._currentRoomPosition, false);
                            _core._currentRoomPosition = newPosition;
                            Navigate(_core._currentRoomPosition, true);

                            _roomLoad = _roomList[_core._currentRoomPosition];


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
                                    newRoom.id = _core._currentRoomPosition;
                                    newRoom.passCount = 0;
                                    newRoom.escapeCount = 0;
                                    _dungeon.roomIsPass.Add(newRoom);
                                }
                                _warpBtn.SetActive(false);
                                _bossRoomBtn.SetActive(_core._currentRoomPosition == _dungeon.dungeon.bossRoom ? true : false);
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
                                        _core.LoadScene(_GameStatus.FORESTSHOP);
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
            if (pos + 1 < sizeMax)
            {
                _roomList[pos + 1].transform.Find("ParticleRoom").gameObject.SetActive(open);
            }
            if (pos - 1 >= 0)
                _roomList[pos - 1].transform.Find("ParticleRoom").gameObject.SetActive(open);
            if (pos + sizeX < sizeMax)
                _roomList[pos + sizeX].transform.Find("ParticleRoom").gameObject.SetActive(open);
            if (pos - sizeX >= 0)
                _roomList[pos - sizeX].transform.Find("ParticleRoom").gameObject.SetActive(open);
        }

        public Monster[] monsterList;

        void LoadMonsterInRoom()
        {
            if (_core._cutscene != null)
            {
               //_core._cutscene.GetComponent<Cutscene>().TutorialPlay(_loadBattlePanel.transform.Find("BG").Find("JoinBattleButton"));
            }
            string[] getMonster;
            int[] bossLvl=null;
            int[] monsterListId=null;
            float bonusBoss = 1;
            
            int posX = _core._currentRoomPosition % sizeX;
            int posY = _core._currentRoomPosition / sizeX + 1;
            int startX = _dungeon.dungeon.startRoom % sizeX;
            int startY = _dungeon.dungeon.startRoom / sizeX + 1;
            int x = Math.Abs(posX - startX);
            int y = Math.Abs(posY - startY);
            int maxPos = (x > y ? x : y);
            
            if (_core._currentRoomPosition == _dungeon.dungeon.bossRoom)
            {
                _escapeRate = 0.4f;
                getMonster = _dungeon.dungeon.bossListId.Split(',');
                monsterListId = new int[getMonster.Length];
                bossLvl = new int[getMonster.Length];
                for (int i =0;i< getMonster.Length;i++)
                {
                    string[] cut = getMonster[i].Split(':');
                    monsterListId[i] = _cal.IntParseFast(cut[0]);
                    bossLvl[i] = _cal.IntParseFast(cut[1]);
                }
                bonusBoss = 2f;
                
                LoadBoss(monsterListId, bossLvl, bonusBoss);
            }
            else
            {
                
                    _escapeRate = 0.5f;
                    
                    _core._currentMonsterBattle = new Monster[1 + (Random.Range(0, max: Random.Range(0f,1f) < 0.5 ? 5: 5 / maxPos))];
                    
                    int MaxLevel = maxPos * _dungeon.dungeon.levelMax / sizeY;
                    Debug.Log("Monster have " + _core._currentMonsterBattle.Length+" lvl max "+ MaxLevel+" dun lvl max "+ _dungeon.dungeon.levelMax);
                    foreach (MonsterDataSet data in _core.dataMonsterList)
                    {
                        if (data.id == _dungeon.dungeon.monsterSetId)
                        {
                            string[] monsList = data.name.Split(',');
                            for (int i = 0; i < _core._currentMonsterBattle.Length; i++)
                            {
                                Monster monster = new Monster(i);
                                int id = Random.Range(0, monsList.Length);
                                monster.name = monsList[id];
                                monster.spriteSet = data.spriteSet;
                                monster.spriteName = data.spriteSet + "_" + id;
                                string[] type = data.type.Split(',');
                                monster.type = (_Character)_cal.IntParseFast(type[id]);
                                
                                monster.level = Random.Range(0, MaxLevel)+_dungeon.dungeon.levelMin;
                                
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
                                string[] baseSTR = data.baseSTR.Split(',');
                                string[] baseAGI = data.baseAGI.Split(',');
                                string[] baseINT = data.baseINT.Split(',');
                                int[] statusAll = _cal.CalculateAllStatus(_cal.IntParseFast(baseSTR[id]), _cal.IntParseFast(baseAGI[id]), _cal.IntParseFast(baseINT[id]), monster.level);
                                monster.STR = (statusAll[0] + statusBonus[0]) * bonusBoss;
                                monster.AGI = (statusAll[1] + statusBonus[1]) * bonusBoss;
                                monster.INT = (statusAll[2] + statusBonus[2]) * bonusBoss;
                                monster.hpMax = _cal.CalculateHpMax(monster);
                                monster.hp = monster.hpMax;
                                monster.ATK = _cal.CalculateATK(monster);
                                monster.MATK = _cal.CalculateMATK(monster);
                                monster.DEF = _cal.CalculateDEF(monster);
                                monster.MDEF = _cal.CalculateMDEF(monster);
                                monster.expDrop = 50 + (monster.STR - monster.AGI);
                                string[] sk = data.skillList.Split(',');
                                string[] skillList = sk[id].Split(':');
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
                                            monster.attack[c] = attack;
                                            break;
                                        }
                                    }
                                }
                                string[] attackp = data.attackPattern.Split(',');
                                monster.attackPattern = attackp;
                                _core._currentMonsterBattle[i] = monster;
                                monsterList = _core._currentMonsterBattle;
                                _monCom._monAvatarList[i].transform.parent.localScale = new Vector3(1, 1, 1);
                            }
                            break;
                        }
                        
                    }
                
                 
            }
            _core.LoadScene(_GameStatus.BATTLE);
        }

        void LoadBoss(int[] monsterListId,int[] bossLvl,float bonusBoss)
        {
            _core._currentMonsterBattle = new Monster[monsterListId.Length];
            for (int i = 0; i < monsterListId.Length; i++)
            {
                foreach (HeroDataSet boss in _core.dataHeroList)
                {
                    if (boss.id != monsterListId[i]) continue;
                    Monster monster = new Monster(i);
                    monster.name = boss.name;
                    monster.spriteSet = boss.spriteSet;
                    monster.spriteName = boss.spriteName;
                    monster.type = boss.type;
                    monster.level = bossLvl[i];

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
                    int[] statusAll = _cal.CalculateAllStatus(boss.baseSTR, boss.baseAGI, boss.baseINT, monster.level);
                    monster.STR = (statusAll[0] + statusBonus[0]) * bonusBoss;
                    monster.AGI = (statusAll[1] + statusBonus[1]) * bonusBoss;
                    monster.INT = (statusAll[2] + statusBonus[2]) * bonusBoss;
                    monster.hpMax = _cal.CalculateHpMax(monster);
                    monster.hp = monster.hpMax;
                    monster.ATK = _cal.CalculateATK(monster);
                    monster.MATK = _cal.CalculateMATK(monster);
                    monster.DEF = _cal.CalculateDEF(monster);
                    monster.MDEF = _cal.CalculateMDEF(monster);
                    monster.expDrop = 400 + (monster.STR - monster.AGI);
                    string[] skillList = boss.skillList.Split(':');
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
                                monster.attack[c] = attack;
                                break;
                            }
                        }
                    }
                    string[] attackp = boss.attackPattern.Split(',');
                    monster.attackPattern = attackp;
                    _core._currentMonsterBattle[i] = monster;
                    monsterList = _core._currentMonsterBattle;
                    break;
                }
                _monCom._monAvatarList[i].transform.parent.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
        }
        
        int teamIsSelect;

        public int TeamIsSelect
        {
            get
            {
                return this.teamIsSelect;
            }
            set
            {
                if (value < 1) {
                    this.teamIsSelect = _core._teamSetup.Count;
                }
                else if(value > _core._teamSetup.Count)
                    this.teamIsSelect = 1;
                else
                    this.teamIsSelect = value;

            }
        }
        
        
        public void BossRoomBtn()
        {
            LoadMonsterInRoom();
        }
        
        public void CampBtn()
        {
            _core.LoadScene(_GameStatus.CAMP);
        }

        public void WarpLandBtn()
        {
            _core.LoadScene(_GameStatus.LAND);
        }
        
        public void ReviveHero()
        {
            for (int a = 0; a < 5; a++)
            {
                if (_teamList[a].id == -1)
                {

                }
                else
                {
                    if (_teamList[a].hp == 0)
                    {
                        _teamList[a].hp = 1;
                        return;
                    }
                }
            }
        }
        
        public float _escapeRate;
        
        public void OnDisable()
        {
            if (_core._mapObj.transform.childCount != 0)
            {
                _roomList[_core._currentRoomPosition].transform.Find("LightMask").gameObject.SetActive(false);
                _roomList[_core._currentRoomPosition].transform.Find("Player").gameObject.SetActive(false);
                Navigate(_core._currentRoomPosition, false);
            }
            SetPanel(false);
        }
    }
}