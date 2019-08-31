using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using battle;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UI;
using character;
using model;
using system;

namespace controller
{
    public class BattleController : MonoBehaviour
    {
        GameCore _core;
        Calculator _cal;
        
        public GameObject _eventPanel;
        public GameObject _rewardPanel;

        public PopupText _showDamage;
        public PopupText _showAction;
        public PopupTurnBattleNotify _showTurnBattle;

        public GameObject[] _heroAvatar;
        public GameObject[] _monAvatar;

        _RoundBattle _roundBattle;

        public int _turnAround;

        public List<Monster> _monster;
        public List<Hero> _hero;
        public Monster[] _monCache;
        public Hero[] _heroCache;

        int _crystalTotal = 4;
        public _BattleState _battleMode;
        
        public GameObject _monHpBar;
        int IsFocusMonster, IsFocusHero;
        public GameObject _hitEffect;
        public Sprite[] _eventIcon;
        public GameObject _playerLifePanel;
        public GameObject _monsterInfoPanel;

        public Monster[] _currentMonsterBattle;

        public Material _injuryMat;
        float lastTimeClick;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = new Calculator();
        }
        
        void OnEnable()
        {
            Camera.main.orthographicSize = 1f;
            
            for(int i=0;i<_monAvatar.Length;i++)
            {
                _monAvatar[i].SetActive(false);
            }
            _monster = new List<Monster>();
            _monCache = new Monster[_currentMonsterBattle.Length];
            for (int i = 0; i < _currentMonsterBattle.Length; i++)
            {
                _currentMonsterBattle[i].LoadAvatar(i);
                _monster.Add(_currentMonsterBattle[i]);
                _monCache[i] = _currentMonsterBattle[i];
            }
            _hero = new List<Hero>();
            _hero.Add(_core._player._heroIsPlaying);
            _heroCache = new Hero[1];
            for (int i = 0; i < _hero.Count; i++)
            {
                _hero[i].LoadAvatar(i);
                _heroCache[i] = _hero[i];
            }

            _focusHero = 0;
            _focusMonster = 0;
            
            _core._playerLifePanel.transform.Find("Crystal").gameObject.SetActive(true);
            UpdateMonsterHP();
            _battleMode = _BattleState.Start;
            _isEscape = false;
            _roundBattle = _RoundBattle.PLAYER;
            _turnAround = 0;
            _eventAround = 0;
            _core.getATKCon()._blockCount = 0;
            Crystal = 1;
            _crystalMon = 1;
            SetPanel(true);
            _core.getATKCon().UpdateAttackSlot();
            
            LoadEvent();
            CreateFocusEffect(FocusMonster().getAvatarTrans());
            ShowTurnBattleNotify();
            _core.getMenuCon().setIconMapBtn("escape");

        }
        public float timeLeft = 3;

        void Update()
        {
            if (_core.IsPaused)
            {
                if (!_core.IsPauseLock)
                {
                    FocusHero().getAnim().enabled = false;
                    FocusMonster().getAnim().enabled = false;
                }
            }
            else
            {
                if (_core.IsPauseLock)
                {
                    FocusHero().getAnim().enabled = true;
                    FocusMonster().getAnim().enabled = true;
                }

                WaitEndTurn();

                foreach(Monster mon in _monster)
                {
                    mon.CheckInjury();
                }
                foreach (Hero hero in _hero)
                {
                    hero.CheckInjury();
                }

                if (_core._monTalkPanel.activeSelf)
                {
                    timeLeft -= Time.deltaTime;
                    if(timeLeft < 0)
                    {
                        _core._monTalkPanel.SetActive(false);
                    }
                }

                OnTouchMonster();
            }
        }
        
        private void LateUpdate()
        {
            
        }

        void OnTouchMonster()
        {
            //-----touch collider2d room-----------
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                if (OnTouchFindTag("Monster"))
                {
                    _monsterInfoPanel.SetActive(true);
                }else
                    _monsterInfoPanel.SetActive(false);
            }
        }

        public bool OnTouchFindTag(string tag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
#endif
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
            if (hit.transform != null && hit.transform.tag == tag)
            {
                float currentTimeClick = Time.time;
                if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
                {
                    currentTimeClick = 0;
                    return true;
                }
                lastTimeClick = currentTimeClick;
            }
            return false;
        }

        public int _focusMonster
        {
            get
            {
                return this.IsFocusMonster;
            }
            set
            {
                
                if (value >= _monster.ToList().Count)
                    this.IsFocusMonster = 0;
                else
                    this.IsFocusMonster = value;
                CreateFocusEffect(FocusMonster().getAvatarTrans());
            }
        }

        public int _focusHero
        {
            get
            {
                return this.IsFocusHero;
            }
            set
            {
                if (value >= _hero.ToList().Count)
                    this.IsFocusHero = 0;
                else
                    this.IsFocusHero = value;
            }
        }

        public void UpdateMonsterHP()
        {
            int hp = 0;
            int hpMax = 0;
            for (int i =0;i< _monCache.Length; i++)
            {
                hp += _monCache[i].getStatus().currentHP;
                hpMax+= _monCache[i].getStatus().currentHPMax;
            }
            _monHpBar.GetComponent<ControlSlider>().AddFill((float)hp * 1 / hpMax);
        }

        public void RunMonAI(int delay = 1)
        {
            StartCoroutine(RunMonsterAI(delay));
        }

        IEnumerator RunMonsterAI(int delay=1)
        {
            if (_crystalMon > 0)
            {
                _battleMode = _BattleState.Wait;
            }
            yield return new WaitForSeconds(delay);
            //Debug.Log("hero count " + _hero.Count+" focus "+_heroFocus);
            if(_monster.ToList().Count > 0 && _hero.ToList().Count > 0)
            {
                _focusMonster = Random.Range(0, _monster.ToList().Count);
                FocusMonster().Attack(FocusHero());
            }
            
        }

        void SetPanel(bool set)
        {
            _eventPanel.SetActive(set);
            _core._menuPanel.transform.Find("MenuMask").Find("GridView").Find("EndTurnButton").gameObject.SetActive(true);
        }
        
        int _eventStart=0;
        int _eventAround=0;
        public _Event _currentEvent;
        int eventRan;
        public float _evenAttack;
        string[] _eventNamePlay;

        void LoadEvent()
        {
            //Debug.Log(_turnBattleCount + " - " + _eventStart + " == " + _eventAround);
            if (_turnAround - _eventStart == _eventAround)
            {
                _eventStart = _turnAround;
                _eventAround = Random.Range(1, 4);
                _evenAttack = 1;
                _currentEvent = (_Event)Random.Range(0, 3);
                Debug.Log("New event is " + _currentEvent);
                if (_currentEvent == _Event.Sunshine)
                {
                    _eventPanel.GetComponent<Image>().sprite = _eventIcon[0];
                    _core._environment[0].SetActive(false);
                }
                else if (_currentEvent == _Event.Rain)
                {
                    _core._environment[0].SetActive(true);
                    _eventPanel.GetComponent<Image>().sprite = _eventIcon[1];
                }
                else if(_currentEvent == _Event.Wind)
                {
                    _eventPanel.GetComponent<Image>().sprite = _eventIcon[2];
                    _core._environment[0].SetActive(false);
                }

            }
            _turnAround++;
        }
        
        EventActive EventFunction;

        private void OnEventFunction(string methodName, params object[] parameter)
        {
            if (EventFunction == null)
                EventFunction = new EventActive();

            var method = EventFunction.GetType().GetMethod(methodName);
            if (method != null)
                method.Invoke(EventFunction, parameter);
        }
        
        public void ShowDamage(int dmg, Vector3 pos)
        {
            if (dmg == 0)
            {
                ShowPopupText("miss", pos, _showDamage);
            }
            else
            {
                ShowPopupText(dmg.ToString(), pos, _showDamage);
            }
            
        }

        public void ShowDamage(string dmg, Vector3 pos)
        {
            ShowPopupText(dmg, pos, _showDamage);
        }

        public void ShowAction(string txt, Vector3 pos)
        {
            ShowPopupText(txt, pos, _showAction);
        }
        
        void ShowPopupText(string dmg, Vector3 pos, PopupText obj)
        {
            PopupText damage = Instantiate(obj);
            damage.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);
            damage.transform.localScale = new Vector3(1, 1, 1);
            damage.SetPopupText(dmg.ToString());
            damage.transform.position = new Vector3(pos.x, pos.y+0.25f, pos.z);
            _battleMode = _BattleState.Finish;
        }

        void ShowTurnBattleNotify()
        {
            PopupTurnBattleNotify popup = Instantiate(_showTurnBattle);
            popup.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);

            if (_roundBattle == _RoundBattle.PLAYER)
            {
                popup.SetNotifyText("Player phase");
                popup.mPopupTurnBattleAnim.SetTrigger("IsTurnPlayer");
            }
            else
            {
                popup.SetNotifyText("Enemy phase");
            }

            popup.SetTurnText("round " + _turnAround);
        }

        public void CloneAttackEffect(SkillDataSet skill,GameObject effect,Transform avatar,_Model target)
        {
            GameObject effectClone = Instantiate(effect);
            if (skill.effect == "Slash")
            {
                effectClone.transform.SetParent(avatar.parent);
                effectClone.transform.localPosition = new Vector3(avatar.localPosition.x, avatar.localPosition.y + 0.25f, avatar.localPosition.z);

            }
            effectClone.AddComponent<ParticleDestroy>();
            StartCoroutine(PlayAnimation(target, skill.delay));
        }
        
        public IEnumerator PlayAnimation(_Model target, float delay = 0.1f)
        {
            _battleMode = _BattleState.Wait;
            yield return new WaitForSeconds(delay);
            if (target == _Model.PLAYER)
            {
                FocusHero().PlayInjury();
            }
            else
            {
                FocusMonster().PlayInjury();
            }
        }
        public bool _waitEndTurn = false;

        void WaitEndTurn()
        {
            if ((_battleMode == _BattleState.Start|| _battleMode == _BattleState.Finish) && _waitEndTurn && _monster.ToList().Count > 0 && _hero.ToList().Count > 0)
            {
                _waitEndTurn = false;
                _battleMode = _BattleState.Start;
                StartCoroutine(EndTurn());
            }
        }

        IEnumerator EndTurn(int dalay=1)
        {
            yield return new WaitForSeconds(dalay);
            _focusMonster = 0;
            _focusHero = 0;
            if (_roundBattle == _RoundBattle.ENEMY)
            {
                _roundBattle = _RoundBattle.PLAYER;
                _effectFocus.SetActive(true);
                LoadEvent();
                _core.getATKCon().UpdateAttackSlot();
                yield return new WaitForSeconds(1);
                ShowTurnBattleNotify();
                _core._menuPanel.transform.Find("MenuMask").Find("GridView").Find("EndTurnButton").gameObject.SetActive(true);
                _core.getMenuCon().OpenBattleMenu();
                _isEscape = false;
                Crystal = _turnAround;
                _crystalMon = _turnAround;
                
            }
            else
            {
                _roundBattle = _RoundBattle.ENEMY;
                _effectFocus.SetActive(false);
                yield return new WaitForSeconds(1);
                ShowTurnBattleNotify();
                //Debug.Log("runAI 1");
                StartCoroutine(RunMonsterAI());
            }
        }
        
        public bool _isEscape;

        public void EndTurnSpeed()
        {
            Crystal = 0;
            _core._menuPanel.transform.Find("MenuMask").Find("GridView").Find("EndTurnButton").gameObject.SetActive(false);
            Debug.Log("end 5");
            _waitEndTurn = true;
        }
        
        IEnumerator BattleWin(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            _core.getMenuCon().CloseBattleMenu();
            _rewardPanel.SetActive(true);
        }

        IEnumerator BattleLose(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            for(int i = 0; i < _monster.Count; i++)
            {
                int damage = 1;
                if (_monster[i].getSpriteSet().Contains("hero"))
                {
                    damage = 5;
                }
                for(int a=0; a< damage; a++)
                {
                    GameObject effect = Instantiate(_hitEffect);
                    effect.transform.SetParent(_monster[i].getAvatarTrans());
                    effect.transform.localScale = new Vector3(1, 1, 1);
                    effect.transform.localPosition = new Vector3(0, 0.5f, 0);
                    yield return new WaitForSeconds(1.5f);
                    ShowAction("-1", _core._playerSoulBar.transform.position);
                    _core._player.currentSoul -= 1;
                }
            }
            _core.getMapCon()._dunBlock[_core._player.currentStayDunBlock].AddEscaped(1);
            yield return new WaitForSeconds(2);
            if(_core._player.currentSoul == 0)
            {
                _core.getSubMenuCore().OpenGameOver();
                _core.getSubMenuCore().setTopic("Game Over");
            }
            else
            {
                _core.OpenScene(_GameState.MAP);
            }
            
        }

        public void OnBattleEnd(bool win)
        {
            if (win)
            {
                StartCoroutine(BattleWin(3));
            }
            else
            {
                StartCoroutine(BattleLose());
                //_core.CallSubMenu(_SubMenuState.BattleEnd, "ทีมของคุณพ้ายแพ้ คุณต้องการต่อสู้อีกรอบหรือไม่?");

            } 
        }
        
        public void EndTurnBtn()
        {
            EndTurnSpeed();
        }
        
        public void AddCrystal(int point)
        {
            _crystalTotal +=  point;
            _core._playerLifePanel.GetComponentInChildren<Text>().text = _crystalTotal.ToString();
        }
        public int _crystalMon;

        public int Crystal
        {
            get
            {
                return this._crystalTotal;
            }
            set
            {
                if (value > 10)
                {
                    value = 10;
                }
                this._crystalTotal = value;
                _core._playerLifePanel.transform.Find("Crystal").GetComponentInChildren<Text>().text = "x"+_crystalTotal.ToString();
            }
        }

        public Character GetTargetFocus()
        {
            Character _char;
            if (_roundBattle == _RoundBattle.PLAYER)
            {
                _char = new Character(FocusMonster());
            }
            else
            {
                _char = new Character(FocusHero());
            }
            return _char;
        }

        public Hero FocusHero()
        {
            try
            {
                return _hero[_focusHero];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
                _waitEndTurn = false;
                return _heroCache[_focusHero];
            }
        }

        public Monster FocusMonster()
        {
            try
            {
                return _monster[_focusMonster];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
                _waitEndTurn = false;
                return _monCache[_focusMonster];
            }
        }
        public GameObject _focusEffect;
        GameObject _effectFocus;

        public void CreateFocusEffect(Transform tran)
        {
            if(_effectFocus == null)
                _effectFocus = Instantiate(_focusEffect);
            _effectFocus.transform.SetParent(tran);
            _effectFocus.transform.localPosition = Vector3.zero;
            _effectFocus.transform.localScale = new Vector3(1, 1, 1);

        }
        
        public void OnDisable()
        {
            
            _waitEndTurn = false;
            SetPanel(false);
            _core.getATKCon().ClearAttackList();
            _core.getMenuCon().CloseBattleMenu();
            _core._playerLifePanel.transform.Find("Crystal").gameObject.SetActive(false);
            _core.getMenuCon().setIconMapBtn("mapOpen");

        }

    }
}

