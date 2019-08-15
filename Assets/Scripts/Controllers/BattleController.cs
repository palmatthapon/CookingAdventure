using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using Json;
using System.Reflection;
using UI;
using character;
using model;

namespace controller
{
    public class BattleController : MonoBehaviour
    {
        GameCore _core;
        Calculate _cal;
        AttackController _attackCon;
        MapController _mapCon;
        BuffController _buffCon;
        MonPanel _monCom;

        public PopupText _showDamage;
        public PopupText _showAction;
        public PopupText _showPassive;
        public PopupTurnBattleNotify _showTurnBattle;
        
        _RoundBattle _roundBattle;

        public int _turnAround;

        public List<Monster> _monster;
        public List<Hero> _hero;
        public Monster[] _monData;
        public Hero[] _heroData;

        int _crystalTotal = 4;
        public _BattleState _battleState;

        public int[,] _damage_of_each_hero;
        public Sprite[] _skillSlot;
        
        public GameObject _monHpBar;
        int IsFocusMonster, IsFocusHero;
        public GameObject _hitEffect;
        public GameObject _defenseEffect;
        public Sprite[] _eventIcon;
        public GameObject _playerLifePanel;

        public Material _injuryMat;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _cal = _core._cal;
            _mapCon = _core._mapCon;
            _monCom = _core._monCom;
            _buffCon = _core._buffCon;
            _attackCon = _core._attackCon;
            _buffCon._buffListPlayer = new List<Buff>();
            _buffCon._buffListMonster = new List<Buff>();
        }

        public Monster[] _monsterList;
        Sprite _bgSprite;
        
        void OnEnable()
        {
            Camera.main.orthographicSize = 1f;
            _monster = new List<Monster>();
            foreach(Monster mon in _mapCon.monsterList)
            {
                mon._icon = _monCom.LoadMonIcon(mon);
                _monster.Add(mon);
            }
            for (int i = _mapCon.monsterList.Length; i < 5; i++)
            {
                _monCom._monAvatarList[i].gameObject.SetActive(false);
            }
            _hero = new List<Hero>();
            for( int i= 0;i< _core._heroStore.Count; i++)
            {
                _core._heroStore[i]._icon = _playerLifePanel;
                _hero.Add(_core._heroStore[i]);
            }
            _focusHero = 0;
            _focusMonster = 0;

            _monData = new Monster[_monster.Count];
            for (int i = 0; i < _monster.Count; i++)
            {
                _monster[i].LoadSprite();
                _monData[i] = _monster[i];
            }
            _heroData = new Hero[_hero.Count];
            for(int i=0;i< _hero.Count; i++)
            {
                _hero[i].LoadSprite();
                _heroData[i] = _hero[i];
            }
            _core._playerLifePanel.transform.Find("Crystal").gameObject.SetActive(true);
            UpdateMonsterHP();
            _bgSprite = _core._bgList[Random.Range(0, _core._bgList.Length)];
            transform.Find("BG").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            transform.Find("BGLeft").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            transform.Find("BGRight").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            _battleState = _BattleState.Start;
            _isEscape = false;
            _roundBattle = _RoundBattle.PLAYER;
            _monsterList = _core._currentMonsterBattle;
            _turnAround = 0;
            _eventAround = 0;
            _attackCon._blockCount = 0;
            Crystal = 1;
            _crystalMon = 1;
            SetPanel(true);
            _attackCon.UpdateAttackSlot();
            _damage_of_each_hero = new int[_monData.Length, _heroData.Length];
            LoadEvent();
            CreateFocusEffect(FocusMonster().GetAvatar().transform);
            ShowTurnBattleNotify();
            _core._mainMenu.GetComponent<MainMenu>()._mapBtn.GetComponent<Image>().sprite = _core._mainMenu.GetComponent<MainMenu>()._mapIcon[0];

        }
        public float timeLeft = 3;

        void Update()
        {
            if (!_core.isPaused)
            {
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
                

            }
            
        }
        
        private void LateUpdate()
        {
            
        }

        public int _focusMonster
        {
            get
            {
                return this.IsFocusMonster;
            }
            set
            {
                foreach (Monster m in _monster.ToList())
                {
                    m._icon.transform.localScale = new Vector3(1, 1, 1);
                    m._icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 150);
                }
                if (value >= _monster.ToList().Count)
                    this.IsFocusMonster = 0;
                else
                    this.IsFocusMonster = value;
                CreateFocusEffect(FocusMonster().GetAvatar().transform);
                FocusMonster()._icon.transform.localScale = new Vector3(1.25f, 1.25f, 1);
                FocusMonster()._icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
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
            for (int i =0;i< _monData.Length; i++)
            {
                hp += _monData[i].GetStatus().currentHP;
                hpMax+= _monData[i].GetStatus().currentHPMax;
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
                _battleState = _BattleState.Wait;
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
            _core._eventPanel.SetActive(set);
            _core._monPanel.SetActive(set);
            _core._mainMenu.transform.Find("MenuMask").Find("GridView").Find("EndTurnButton").gameObject.SetActive(true);
        }
        
        public void RunCounterAttack()
        {
            StartCoroutine(CounterAttack());
        }
        
        GameObject _counterAttackEffect;
        public bool _counterATKSuccess =false;
        public int _counterATKDamage;

        IEnumerator CounterAttack()
        {
            Hero heroTarget = FocusHero();
            Transform getAvatarTrans = heroTarget.GetAvatar().transform;
            GameObject effect = Instantiate(_defenseEffect);
            effect.transform.SetParent(getAvatarTrans.parent);
            effect.transform.localPosition = new Vector3(getAvatarTrans.localPosition.x-0.25f, getAvatarTrans.localPosition.y+0.45f, getAvatarTrans.localPosition.z);
            effect.AddComponent<ParticleDestroy>();
            
            yield return new WaitForSeconds(1f);
            _counterATKSuccess = true;
            heroTarget.PlayInjury(0);
            yield return new WaitForSeconds(1f);
            
            ShowAction("โจมตีสวนกลับ", getAvatarTrans.position);

            yield return new WaitForSeconds(1f);
            
            _counterATKDamage = heroTarget.CalDamageCounterAttack(FocusMonster());
            AttackEffect(heroTarget.GetStatus().attack[0], _counterATKDamage,_Model.MONSTER);
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
                    _core._eventPanel.GetComponent<Image>().sprite = _eventIcon[0];
                    _core._environment[0].SetActive(false);
                }
                else if (_currentEvent == _Event.Rain)
                {
                    _core._environment[0].SetActive(true);
                    _core._eventPanel.GetComponent<Image>().sprite = _eventIcon[1];
                }
                else if(_currentEvent == _Event.Wind)
                {
                    _core._eventPanel.GetComponent<Image>().sprite = _eventIcon[2];
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
        
        Hero _heroIsQuesttion;
        bool _isUltimate;
        int blockStack;
        
        int _slotIsSelect;
        
        public void ShowDamage(int dmg, Vector3 pos)
        {
            if (dmg == 0)
            {
                ShowPopupText("พลาดเป้า", pos, _showDamage);
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

        public void ShowPassive(string txt, Vector3 pos)
        {
            ShowPopupText(txt, pos, _showPassive);
        }

        void ShowPopupText(string dmg, Vector3 pos, PopupText obj)
        {
            PopupText damage = Instantiate(obj);
            damage.transform.SetParent(GameObject.Find("FrontCanvas").transform, false);
            damage.transform.localScale = new Vector3(1, 1, 1);
            damage.SetPopupText(dmg.ToString());
            damage.transform.position = new Vector3(pos.x, pos.y+0.25f, pos.z);
            _battleState = _BattleState.Finish;
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
        GameObject[] loadEffectPlayer;
        GameObject[] loadEffectMonster;

        public void AttackEffect(Skill skill,int damage, _Model target)
        {
            bool NoBuff = false;

            if (target == _Model.MONSTER)
            {
                if(loadEffectPlayer==null)
                    loadEffectPlayer = Resources.LoadAll<GameObject>("Effects/Effect_Player/");

                for (int i=0;i< loadEffectPlayer.Length;i++)
                {
                    if(skill.skill.effect == loadEffectPlayer[i].name)
                    {
                        GameObject effect = Instantiate(loadEffectPlayer[i]);
                        if(skill.skill.effect == "Slash")
                        {
                            effect.transform.SetParent(FocusMonster().GetAvatar().transform.parent);
                            effect.transform.localPosition = new Vector3(FocusMonster().GetAvatar().transform.localPosition.x, FocusMonster().GetAvatar().transform.localPosition.y + 0.25f, FocusMonster().GetAvatar().transform.localPosition.z);
                            
                        }
                        effect.AddComponent<ParticleDestroy>();
                        if (skill.skill.type != _Attack.BUFF)
                        {
                            StartCoroutine(PlayAnimation(damage, _Model.MONSTER, skill.skill.delay));
                            NoBuff = true;
                        }
                        break;
                    }
                }
                if (!NoBuff)
                {
                    _battleState = _BattleState.Finish;
                }

            }
            else
            {
               
                if (loadEffectMonster == null)
                    loadEffectMonster = Resources.LoadAll<GameObject>("Effects/Effect_Monster/");

                for (int i = 0; i < loadEffectMonster.Length; i++)
                {
                    if (skill.skill.effect == loadEffectMonster[i].name)
                    {
                        GameObject effect = Instantiate(loadEffectMonster[i]);
                        if (skill.skill.effect == "Slash")
                        {
                            effect.transform.SetParent(FocusHero().GetAvatar().transform.parent);
                            effect.transform.localPosition = new Vector3(FocusHero().GetAvatar().transform.localPosition.x, FocusHero().GetAvatar().transform.localPosition.y + 0.25f, FocusHero().GetAvatar().transform.localPosition.z);
                            
                        }
                        effect.AddComponent<ParticleDestroy>();
                        if (skill.skill.type != _Attack.BUFF)
                        {
                            StartCoroutine(PlayAnimation(damage, _Model.PLAYER, skill.skill.delay));
                            NoBuff = true;
                        }
                        break;
                    }
                }
                if (!NoBuff)
                {
                    Debug.Log("end 1");
                    _waitEndTurn = true;
                }
            }
            
        }
        
        public IEnumerator PlayAnimation(int damage, _Model target, float delay = 0.1f)
        {
            _battleState = _BattleState.Wait;
            yield return new WaitForSeconds(delay);
            if (target == _Model.PLAYER)
            {
                FocusHero().PlayInjury(damage);
            }
            else
            {
                FocusMonster().PlayInjury(damage);
            }
        }
        public bool _waitEndTurn = false;

        void WaitEndTurn()
        {
            if ((_battleState == _BattleState.Start||_battleState == _BattleState.Finish) && _waitEndTurn && _monster.ToList().Count > 0 && _hero.ToList().Count > 0)
            {
                _waitEndTurn = false;
                _battleState = _BattleState.Start;
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
                _attackCon.UpdateAttackSlot();
                EndTurnSetting();
                yield return new WaitForSeconds(1);
                ShowTurnBattleNotify();
                _core._mainMenu.transform.Find("MenuMask").Find("GridView").Find("EndTurnButton").gameObject.SetActive(true);
                _core.OpenActionPanel(_core._attackPanel);
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

        void EndTurnSetting()
        {
            foreach (Buff buff in _buffCon._buffListPlayer.ToList())
            {
                if (buff.timeCount <= _turnAround - buff.startTime)
                {
                    //Debug.Log("remove buff");
                    _buffCon.RemoveBuff(buff);
                }
                else
                {
                    //Debug.Log("upate buff"+ (buff.timeCount - (_turnBattleCount - buff.startTime)));
                    buff.obj.transform.Find("Text").GetComponent<Text>().text = (buff.timeCount - (_turnAround - buff.startTime)).ToString();
                }
            }
            foreach (Buff buff in _buffCon._buffListMonster.ToList())
            {
                if (buff.timeCount <= _turnAround - buff.startTime)
                {
                    _buffCon.RemoveBuff(buff);
                }
                else
                {
                    buff.obj.transform.Find("Text").GetComponent<Text>().text = (buff.timeCount - (_turnAround - buff.startTime)).ToString();
                }
            }
        }
        public bool _isEscape;

        public void EndTurnSpeed()
        {
            Crystal = 0;
            _core._mainMenu.transform.Find("MenuMask").Find("GridView").Find("EndTurnButton").gameObject.SetActive(false);
            Debug.Log("end 5");
            _waitEndTurn = true;
        }
        
        IEnumerator BattleWin(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            _core._attackPanel.SetActive(false);
            _core._rewardPanel.SetActive(true);
        }

        IEnumerator BattleLose(int delay=1)
        {
            yield return new WaitForSeconds(delay);
            for(int i = 0; i < _monster.Count; i++)
            {
                int damage = 1;
                if (_monster[i].GetData().spriteSet.Contains("hero"))
                {
                    damage = 5;
                }
                for(int a=0; a< damage; a++)
                {
                    GameObject effect = Instantiate(_hitEffect);
                    effect.transform.SetParent(_monster[i].GetAvatar().transform);
                    effect.transform.localScale = new Vector3(1, 1, 1);
                    effect.transform.localPosition = new Vector3(0, 0.5f, 0);
                    yield return new WaitForSeconds(1.5f);
                    ShowAction("-1", _core._playerSoulBar.transform.position);
                    _core._playerHP -= 1;
                }
            }
            _core.CalEscapeRoom();
            yield return new WaitForSeconds(2);
            if(_core._playerHP == 0)
            {
                _core.CallSubMenu(_SubMenu.GameOver, "Game Over");
            }
            else
            {
                _core.LoadScene(_GameStatus.MAP);
            }
            
        }

        public void OnBattleEnd(bool win)
        {
            if (win)
            {
                UnlockHeroList();
                StartCoroutine(BattleWin(3));
            }
            else
            {
                StartCoroutine(BattleLose());
                //_core.CallSubMenu(_SubMenu.BattleEnd, "ทีมของคุณพ้ายแพ้ คุณต้องการต่อสู้อีกรอบหรือไม่?");

            } 
        }

        void UnlockHeroList()
        {
            for (int i = 0; i < _monsterList.Length; i++)
            {
                if (_monsterList[i].GetData().spriteSet.Contains("hero"))
                {
                    if (_core._unlockHeroList == null)
                    {
                        _core._unlockHeroList = new List<string>();
                    }
                    int row = 0;
                    foreach (Hero h in _core._heroStore)
                    {
                        row++;
                        if (h.GetData().spriteName == _monsterList[i].GetData().spriteName)
                        {
                            if (Random.Range(0f, 1f) < 0.1f)
                            {
                                _core._unlockHeroList.Add(_monsterList[i].GetData().spriteName);
                            }
                            break;
                        }
                        if (row == _core._heroStore.Count)
                        {
                            _core._unlockHeroList.Add(_monsterList[i].GetData().spriteName);
                        }
                    }
                }
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
                return _heroData[_focusHero];
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
                return _monData[_focusMonster];
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
            foreach (Transform tran in _core._monPanel.transform.Find("GridView"))
            {
                Destroy(tran.gameObject);
            }
            
            _waitEndTurn = false;
            SetPanel(false);
            _buffCon.ClearBuffAll(_Model.PLAYER);
            _buffCon.ClearBuffAll(_Model.MONSTER);
            _buffCon.ClearDefense();
            _attackCon.ClearAttackList();
            _buffCon._defenseList.Clear();
            
            _core._playerLifePanel.transform.Find("Crystal").gameObject.SetActive(false);
            _core._mainMenu.GetComponent<MainMenu>()._mapBtn.GetComponent<Image>().sprite = _core._mainMenu.GetComponent<MainMenu>()._mapIcon[2];

        }

    }
}

