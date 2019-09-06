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
        Calculator _cal;
        
        public GameObject _eventPanel;
        public GameObject _rewardPanel;

        public PopupText _showDamage;
        public PopupText _showAction;
        public PopupTurnBattleNotify _showTurnBattle;

        public GameObject[] _heroAvatar;
        public GameObject[] _monAvatar;

        public bool _waitEndTurn = false;
        public bool _isEscape;

        _RoundBattle _roundBattle;

        public int _turnAround;
        
        int _crystalTotal = 4;
        public _BattleState _battleMode;
        
        public GameObject _hitEffect;
        public Sprite[] _eventIcon;
        public GameObject _playerLifePanel;
        public GameObject _monsterHPPanel;
        
        public Monster[] _currentMonBatt;
        public Hero _currentHeroBatt;

        public Material _injuryMat;
        float lastTimeClick;

        int _eventStart = 0;
        int _eventAround = 0;
        public _Event _currentEvent;
        int eventRan;
        public float _evenAttack;
        string[] _eventNamePlay;

        public int[] _monsterSlot;

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
            _monsterSlot = new int[_currentMonBatt.Length];
            for (int i = 0; i < _currentMonBatt.Length; i++)
            {
                _monsterSlot[i] = i;
                _currentMonBatt[i].CreateAvatar(i);
            }
            _currentHeroBatt = _core._player._heroIsPlaying;
            _currentHeroBatt.CreateAvatar(0);
            
            
            _battleMode = _BattleState.Start;
            _isEscape = false;
            _roundBattle = _RoundBattle.PLAYER;
            _turnAround = 0;
            _eventAround = 0;
            _core.getATKCon()._blockCount = 0;
            Crystal = 1;
            _crystalMon = 1;
            _core.getATKCon().UpdateAttackSlot();
            
            LoadEvent();

            ShowTurnBattleNotify();

            _core.getMenuCon().setIconMapBtn("escape");

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

                WaitEndTurn();

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
            _eventPanel.SetActive(input);
            _core._playerLifePanel.transform.Find("Crystal").gameObject.SetActive(input);
            _monsterHPPanel.SetActive(input);
            _core.getMenuCon().gridViewTrans.Find("EndTurnButton").gameObject.SetActive(input);
            _core.getMenuCon()._attackMenu.SetActive(input);
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
            Debug.Log("before target " + targetOfHero);
            if (_monsterSlot.Length <= 0) return;
            OpenFocusTargetOfHero(false);
            int count = 0;
            targetOfHero++;
            while (!_monsterSlot.Contains(targetOfHero)){
                count++;
                targetOfHero++;
                Debug.Log(targetOfHero + " check " + _monsterSlot.Contains(targetOfHero));
                if (count > 4) break;
            }
            Debug.Log("after target " + targetOfHero);
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
            getTargetOfHero().getAvatarTrans().Find("FocusEffect").gameObject.SetActive(input);
            if(input)
                getTargetOfHero().UpdateHPBar();
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
            if(_currentMonBatt.ToList().Count > 0 && _currentHeroBatt.getStatus().currentHP > 0)
            {
                targetOfHero = _monsterSlot[Random.Range(0, _monsterSlot.ToList().Count)];
                getTargetOfHero().Attack(getTargetOfMonster());
            }

        }

        
        
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
                getTargetOfMonster().PlayInjury();
            }
            else
            {
                getTargetOfHero().PlayInjury();
            }
        }
        

        void WaitEndTurn()
        {
            if ((_battleMode == _BattleState.Start|| _battleMode == _BattleState.Finish) && _waitEndTurn && _currentMonBatt.ToList().Count > 0 && _currentHeroBatt.getStatus().currentHP > 0)
            {
                _waitEndTurn = false;
                _battleMode = _BattleState.Start;
                StartCoroutine(EndTurn());
            }
        }

        IEnumerator EndTurn(int dalay=1)
        {
            yield return new WaitForSeconds(dalay);
            if (_roundBattle == _RoundBattle.ENEMY)
            {
                _roundBattle = _RoundBattle.PLAYER;
                OpenFocusTargetOfHero(false);
                targetOfHero = _monsterSlot[0];
                OpenFocusTargetOfHero(true);
                LoadEvent();
                _core.getATKCon().UpdateAttackSlot();
                yield return new WaitForSeconds(1);
                ShowTurnBattleNotify();
                _core.getMenuCon().gridViewTrans.Find("EndTurnButton").gameObject.SetActive(true);
                _core.getMenuCon()._attackMenu.SetActive(true);
                _isEscape = false;
                Crystal = _turnAround;
                _crystalMon = _turnAround;
                
            }
            else
            {
                _roundBattle = _RoundBattle.ENEMY;
                OpenFocusTargetOfHero(false);
                yield return new WaitForSeconds(1);
                ShowTurnBattleNotify();
                //Debug.Log("runAI 1");
                StartCoroutine(RunMonsterAI());
            }
        }
        

        public void EndTurnSpeed()
        {
            Crystal = 0;
            _core.getMenuCon().gridViewTrans.Find("EndTurnButton").gameObject.SetActive(false);
            Debug.Log("end 5");
            _waitEndTurn = true;
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
            for(int i = 0; i < _currentMonBatt.Length; i++)
            {
                int damage = 1;
                if (_currentMonBatt[i].getSpriteSet().Contains("hero"))
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

        public Character getTarget()
        {
            Character _char;
            if (_roundBattle == _RoundBattle.PLAYER)
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
            _waitEndTurn = false;
            _core.getATKCon().ClearAttackList();
            _core.getMenuCon().setIconMapBtn("mapOpen");
            OpenPanelOnStartBattle(false);
        }

    }
}

