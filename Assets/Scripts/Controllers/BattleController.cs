using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using battle;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UI;
using model;
using system;

namespace controller
{
    public class BattleController : MonoBehaviour
    {
        GameCore _core;
        public BATTLESTATE IsBattleState;
        public BATTLEROUND _battleRound;
        public int _turnAround;
        public int _crystalPlayer;
        public int _crystalMon;

        public GameObject _eventPanel;
        public GameObject _rewardPanel;

        public PopupText _showDamage;
        public PopupText _showAction;
        public PopupTurnBattleNotify _showTurnBattle;

        public GameObject[] _heroAvatar;
        public GameObject[] _monAvatar;
        
        public bool _isEscape;
        
        public GameObject _hitEffect;
        public GameObject _playerLifePanel;
        public GameObject _monsterHPPanel;
        
        public Monster[] _currentMonBatt;
        public Hero _currentHeroBatt;

        public Material _injuryMat;
        float lastTimeClick;

        int _eventStart = 0;
        int _eventAround = 0;
        public EVENT _currentEvent;
        int eventRan;
        public float _evenAttack;
        string[] _eventNamePlay;

        public int[] _monsterSlot;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }
        
        void OnEnable()
        {
            Camera.main.orthographicSize = 1f;
            
            for(int i=0;i<_monAvatar.Length;i++)
            {
                _monAvatar[i].SetActive(false);
            }
            _monsterSlot = new int[_currentMonBatt.Length];
            for (int i = 0; i < _currentMonBatt.Length; i++)
            {
                _monsterSlot[i] = i;
                _currentMonBatt[i].CreateAvatar(i);
            }
            _currentHeroBatt = _core._player._heroIsPlaying;
            _currentHeroBatt.CreateAvatar(0);
            targetOfHero = _monsterSlot[0];
            _battleState = BATTLESTATE.Start;
            _isEscape = false;
            _battleRound = BATTLEROUND.PLAYER;
            _turnAround = 0;
            _eventAround = 0;
            _core.getATKCon()._blockCount = 0;
            Crystal = 1;
            _crystalMon = 1;
            _core.getATKCon().UpdateAttackSlot();
            
            LoadEvent();

            ShowTurnBattleNotify();
            
            getTargetOfHero().UpdateHPBar();
            _currentHeroBatt.UpdateHPBar();
            OpenFocusTargetOfHero(true);
            OpenPanelOnStartBattle(true);
        }
        

        void Update()
        {
            if (_core.IsPaused)
            {
                if (!_core.IsPauseLock)
                {
                    OpenFocusTargetOfHero(false);
                }
            }
            else
            {
                if (_core.IsPauseLock)
                {
                    OpenFocusTargetOfHero(true);
                }
                
                foreach(Monster mon in _currentMonBatt)
                {
                    if(mon.getStatus().currentHP>0)
                        mon.CheckInjury();
                }
                if (_currentHeroBatt.getStatus().currentHP > 0)
                    _currentHeroBatt.CheckInjury();

            }
        }
        
        private void LateUpdate()
        {
            
        }

        void OpenPanelOnStartBattle(bool input)
        {
            //_eventPanel.SetActive(input);
            _core._playerLifePanel.transform.Find("Crystal").gameObject.SetActive(input);
            _monsterHPPanel.SetActive(input);
            _core.getMenuCon().gridViewTrans.Find("EndTurnButton").gameObject.SetActive(input);
        }

        int IsTargetOfHero = 0;

        public int targetOfHero
        {
            get
            {
                return IsTargetOfHero;
            }
            set
            {
                if (value < 5)
                    IsTargetOfHero = value;
                else
                    IsTargetOfHero = 0;
            }
        }

        public void OnNextTargetOfHero()
        {
            if (_monsterSlot.Length <= 0) return;
            OpenFocusTargetOfHero(false);
            int count = 0;
            targetOfHero++;
            while (!_monsterSlot.Contains(targetOfHero)){
                count++;
                targetOfHero++;
                if (count > 4) break;
            }
            OpenFocusTargetOfHero(true);
        }
        
        public Monster getTargetOfHero()
        {
            return _currentMonBatt[targetOfHero];
        }

        public Hero getTargetOfMonster()
        {
            return _currentHeroBatt;
        }

        public void OpenFocusTargetOfHero(bool input)
        {
            if (input)
            {
                getTargetOfHero().UpdateHPBar();
                _monsterHPPanel.SetActive(true);
            }
            else
                _monsterHPPanel.SetActive(false);
            getTargetOfHero().getAvatarTrans().Find("FocusEffect").gameObject.SetActive(input);
            
        }

        public void RunMonsterAI(int delay = 1)
        {
            Debug.Log("runAI 2 mons count " + _monsterSlot.ToList().Count + " hero hp " + _currentHeroBatt.getStatus().currentHP);
            if (_monsterSlot.ToList().Count > 0 && _currentHeroBatt.getStatus().currentHP > 0)
                StartCoroutine(CallMonsterAttack(delay));
        }

        IEnumerator CallMonsterAttack(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            targetOfHero = _monsterSlot[Random.Range(0, _monsterSlot.ToList().Count)];
            getTargetOfHero().Attack(getTargetOfMonster());
            Debug.Log("Run AI finished");

        }

        void LoadEvent()
        {
            //Debug.Log(_turnBattleCount + " - " + _eventStart + " == " + _eventAround);
            if (_turnAround - _eventStart == _eventAround)
            {
                _eventStart = _turnAround;
                _eventAround = Random.Range(1, 4);
                _evenAttack = 1;
                _currentEvent = (EVENT)Random.Range(0, 3);
                Debug.Log("New event is " + _currentEvent);
                if (_currentEvent == EVENT.Sunshine)
                {
                    //_eventPanel.GetComponent<Image>().sprite = _eventIcon[0];
                    _core._environment[0].SetActive(false);
                }
                else if (_currentEvent == EVENT.Rain)
                {
                    _core._environment[0].SetActive(true);
                    //_eventPanel.GetComponent<Image>().sprite = _eventIcon[1];
                }
                else if(_currentEvent == EVENT.Wind)
                {
                    //_eventPanel.GetComponent<Image>().sprite = _eventIcon[2];
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
        }

        void ShowTurnBattleNotify()
        {
            PopupTurnBattleNotify popup = Instantiate(_showTurnBattle);
            popup.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);

            if (_battleRound == BATTLEROUND.PLAYER)
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

        public void CloneAttackEffect(SkillDataSet skill,GameObject effect,Transform avatar, MODEL target)
        {
            GameObject effectClone = Instantiate(effect);
            effectClone.transform.SetParent(avatar.parent);
            effectClone.transform.localPosition = new Vector3(avatar.localPosition.x, avatar.localPosition.y + 0.25f, avatar.localPosition.z);
            effectClone.AddComponent<ParticleDestroy>();
            StartCoroutine(PlayAnimation(target, skill.delay));
        }
        
        public IEnumerator PlayAnimation(MODEL target, float delay = 0.1f)
        {
            yield return new WaitForSeconds(delay);
            if (target == MODEL.PLAYER)
            {
                getTargetOfMonster().PlayInjury();
            }
            else
            {
                getTargetOfHero().PlayInjury();
            }
        }


        public BATTLESTATE _battleState
        {
            get
            {
                return IsBattleState;
            }
            set
            {
                if (value == BATTLESTATE.Finish && (_crystalMon<=0|| _crystalPlayer <= 0))
                {
                    IsBattleState = BATTLESTATE.Start;
                    StartCoroutine(EndTurn());
                }
                else
                {
                    IsBattleState = BATTLESTATE.Wait;
                }
            }
            
        }

        IEnumerator EndTurn(int dalay=1)
        {
            yield return new WaitForSeconds(dalay);

            if (_battleRound == BATTLEROUND.ENEMY)
            {
                _battleRound = BATTLEROUND.PLAYER;
                OpenFocusTargetOfHero(false);
                targetOfHero = _monsterSlot[0];
                _core.getATKCon().UpdateAttackSlot();
                yield return new WaitForSeconds(1);
                _core.getMenuCon().gridViewTrans.Find("EndTurnButton").gameObject.SetActive(true);
                OpenFocusTargetOfHero(true);
                LoadEvent();
                ShowTurnBattleNotify();
                _isEscape = false;
                Crystal = _turnAround;
                _crystalMon = _turnAround;
                
            }
            else
            {
                _battleRound = BATTLEROUND.ENEMY;
                yield return new WaitForSeconds(1);
                OpenFocusTargetOfHero(false);
                _core.getMenuCon()._attackMenu.SetActive(false);
                if (_monsterSlot.ToList().Count > 0)
                {
                    ShowTurnBattleNotify();
                    Debug.Log("runAI 1");
                    RunMonsterAI();
                }
            }
        }
        

        public void EndTurnSpeed()
        {
            Crystal = 0;
            _core.getMenuCon().gridViewTrans.Find("EndTurnButton").gameObject.SetActive(false);
            _battleState = BATTLESTATE.Finish;
        }
        
        IEnumerator BattleWin(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            _core.getMenuCon()._attackMenu.SetActive(false);
            _rewardPanel.SetActive(true);
        }

        IEnumerator BattleLose(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            for(int i = 0; i < _monsterSlot.Length; i++)
            {
                int damage = 1;
                if (_core._dungeon[_core._player.currentDungeonFloor-1].data.bossBlock == _core._player.currentStayDunBlock)
                {
                    damage = 5;
                }
                for(int a=0; a< damage; a++)
                {
                    GameObject effect = Instantiate(_hitEffect);
                    effect.transform.SetParent(_currentMonBatt[i].getAvatarTrans());
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
                _core.OpenScene(GAMESTATE.MAP);
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

            } 
        }
        
        public void EndTurnBtn()
        {
            EndTurnSpeed();
        }
        
        public int Crystal
        {
            get
            {
                return this._crystalPlayer;
            }
            set
            {
                if (value > 10)
                {
                    value = 10;
                }
                this._crystalPlayer = value;
                _core._playerLifePanel.transform.Find("Crystal").GetComponentInChildren<Text>().text = "x"+ _crystalPlayer.ToString();
            }
        }

        public Character getTarget()
        {
            Character _char;
            if (_battleRound == BATTLEROUND.PLAYER)
            {
                _char = new Character(getTargetOfHero());
            }
            else
            {
                _char = new Character(getTargetOfMonster());
            }
            return _char;
        }
        
        public void OnDisable()
        {
            _core.getATKCon().ClearAttackList();
            OpenPanelOnStartBattle(false);
            _core.getMenuCon()._attackMenu.SetActive(false);
        }

    }
}

