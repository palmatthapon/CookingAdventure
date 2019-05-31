using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UnityEngine.UI;
using CollectionData;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using Json;
using System.Reflection;
using UI;

namespace Controller
{
    public class BattleController : MonoBehaviour
    {
        MainCore _core;
        Calculate _cal;
        SelectAttackController _selectATKCon;
        MapController _mapCon;
        BuffController _buffCon;
        MonPanel _monCom;
        int _answerCorrect;

        public PopupText _showDamage;
        public PopupText _showAction;
        public PopupText _showPassive;
        public PopupTurnBattleNotify _showTurnBattle;
        public GameObject _answerSlot;
        
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
        int monFocus, heroFocus;
        public GameObject _hitEffect;
        public GameObject _defenseEffect;
        public Sprite[] _eventIcon;
        public GameObject _playerLifePanel;


        private void Awake()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _cal = _core._cal;
            _mapCon = _core._mapCon;
            _monCom = _core._monCom;
            _buffCon = _core._buffCon;
            _selectATKCon = _core._selectATKCon;
            _buffCon._buffListPlayer = new List<Buff>();
            _buffCon._buffListMonster = new List<Buff>();
        }

        public Monster[] _monsterList;
        public HeroInTeam _teamList;
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
            for( int i= 0;i< _mapCon._teamList.Count;i++)
            {
                if (_mapCon._teamList[i].id == -1)
                {

                }
                else
                {
                    Hero newHero = new Hero(i);
                    newHero.hero = _mapCon._teamList[i];
                    newHero._icon = _playerLifePanel;
                    _hero.Add(newHero);
                }
            }
            _heroFocus = 0;
            _monFocus = 0;

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
            _core._playerSoulBar.transform.Find("ActionPointText").gameObject.SetActive(true);
            UpdateMonsterHP();
            _bgSprite = _core._bgList[Random.Range(0, _core._bgList.Length)];
            transform.Find("BG").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            transform.Find("BGLeft").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            transform.Find("BGRight").GetComponent<SpriteRenderer>().sprite = _bgSprite;
            _battleState = _BattleState.Start;
            _isEscape = false;
            _roundBattle = _RoundBattle.PLAYER;
            _monsterList = _core._currentMonsterBattle;
            _teamList = _core._teamSetup[_core._currentTeamIsSelect - 1];
            _turnAround = 0;
            _eventAround = 0;
            _selectATKCon._blockCount = 0;
            Crystal = 1;
            _crystalMon = 1;
            SetPanel(true);
            _selectATKCon.UpdateAttackSlot();
            _damage_of_each_hero = new int[_monData.Length, _heroData.Length];
            LoadEvent();
            CreateFocusEffect(GetMonFocus()._avatar.transform);
            ShowTurnBattleNotify();
            if (_core._cutscene != null)
            {
                _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._attackPanel.transform.Find("ActionMask").Find("GridView").GetChild(0), true,
                           "ในโหมดต่อสู้นี้ถ้าเจ้ากำจัดมอนสเตอร์หมดก็จะชนะ_แต่ถ้าทีมเจ้าแพ้จะโดนดาเมจตามจำนวนมอนสเตอร์ที่เหลืออยู่บนสนาม_หากหลอดเลือดที่มุมล่างจอเหลือศูนย์เจ้าคงรู้นะว่าจะเกิดอะไรขึ้น!");
            }

        }
        public float timeLeft = 3;

        void Update()
        {
            if (!_core.isPaused)
            {
                WaitEndTurn();

                foreach(Monster mon in _monster)
                {
                    mon.CheckInjurious();
                }
                foreach (Hero hero in _hero)
                {
                    hero.CheckInjurious();
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

        public int _monFocus
        {
            get
            {
                return this.monFocus;
            }
            set
            {
                foreach (Monster m in _monster.ToList())
                {
                    m._icon.transform.localScale = new Vector3(1, 1, 1);
                    m._icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 150);
                }
                if (value >= _monster.ToList().Count)
                    this.monFocus = 0;
                else
                    this.monFocus = value;
                CreateFocusEffect(GetMonFocus()._avatar.transform);
                GetMonFocus()._icon.transform.localScale = new Vector3(1.25f, 1.25f, 1);
                GetMonFocus()._icon.transform.Find("IconImage").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }

        public int _heroFocus
        {
            get
            {
                return this.heroFocus;
            }
            set
            {
                if (value >= _hero.ToList().Count)
                    this.heroFocus = 0;
                else
                    this.heroFocus = value;
            }
        }

        public void UpdateMonsterHP()
        {
            int hp = 0;
            int hpMax = 0;
            for (int i =0;i< _monData.Length; i++)
            {
                hp += _monData[i].hp;
                hpMax+= _monData[i].hpMax;
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
                _monFocus = Random.Range(0, _monster.ToList().Count);
                GetMonFocus().Attack(GetHeroFocus());
            }
            
        }

        void SetPanel(bool set)
        {
            _core._eventPanel.SetActive(set);
            _core._monPanel.SetActive(set);
            //_core._mainMenuBG.SetActive(set);
            //_core._menuPanel.transform.parent.gameObject.SetActive(set);
            _core._playerLifePanel.transform.Find("EndTurnButton").gameObject.SetActive(true);
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
            GameObject effect = Instantiate(_defenseEffect);
            effect.transform.SetParent(GetHeroFocus()._avatar.transform.parent);
            effect.transform.localPosition = new Vector3(GetHeroFocus()._avatar.transform.localPosition.x-0.25f, GetHeroFocus()._avatar.transform.localPosition.y+0.45f, GetHeroFocus()._avatar.transform.localPosition.z);
            effect.AddComponent<ParticleDestroy>();
            
            yield return new WaitForSeconds(1f);
            _counterATKSuccess = true;
            GetHeroFocus().PlayInjurious(0);
            yield return new WaitForSeconds(1f);
            
            ShowAction("โจมตีสวนกลับ", GetHeroFocus()._avatar.transform.position);

            yield return new WaitForSeconds(1f);
            
            _counterATKDamage = GetHeroFocus().CalDamageCounterAttack(GetMonFocus());
            AttackEffect(GetHeroFocus().hero.attack[0], _counterATKDamage,_Model.MONSTER);
        }
        int _eventStart=0;
        int _eventAround=0;
        public _event _currentEvent;
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
                _currentEvent = (_event)Random.Range(0, 3);
                Debug.Log("New event is " + _currentEvent);
                if (_currentEvent == _event.Sunshine)
                {
                    _core._eventPanel.GetComponent<Image>().sprite = _eventIcon[0];
                    _core._environment[0].SetActive(false);
                }
                else if (_currentEvent == _event.Rain)
                {
                    _core._environment[0].SetActive(true);
                    _core._eventPanel.GetComponent<Image>().sprite = _eventIcon[1];
                }
                else if(_currentEvent == _event.Wind)
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
        
        void AnswerClick(int ans)
        {
            //_core._mainMenuBG.SetActive(true);
            Camera.main.orthographicSize = 1f;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                    0f,
                    Camera.main.transform.position.z);
            _core.OpenActionPanel(_core._attackPanel);
            OnCheckAnswer(ans);
        }
        Hero _heroIsQuesttion;
        bool _isUltimate;
        int blockStack;

        public void OpenQuestion(Hero hero,bool ultimate,int stack)
        {
            _heroIsQuesttion = hero;
            _isUltimate = ultimate;
            blockStack = stack;
            //_core._mainMenuBG.SetActive(false);
            Camera.main.orthographicSize = 0.5f;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                0.15f,
                Camera.main.transform.position.z);
            _core.OpenActionPanel(_core._questionPanel);
            int ran;
            int errorLoop = 0;
            do
            {
                ran = Random.Range(0, _core._questionLoadComplete);
                errorLoop++;
            } while (ran == _questionRandom || ran == _questionRandom && errorLoop < 20);
            _questionRandom = ran;
            GetQuestion(_questionRandom);

        }
        int _questionRandom = 0;
        
        int _slotIsSelect;
        
        void OnCheckAnswer(int ans)
        {
            if (ans == _answerCorrect)
            {
                _core.OpenTrueNotify("เป็นคำตอบที่ถูกต้อง");

                if (_core._ActionMode == _ActionStatus.Attack)
                {
                    StartCoroutine(QuestionIsAttack(false));
                }
            }
            else
            {
                _core.OpenErrorNotify("เป็นคำตอบที่ผิด");
                _core.SetMonTalk("<color=#418d29>เฉลยคือ " + _answerList[_answerCorrect - 1].GetComponentInChildren<Text>().text+"</color>");
                
                if (_core._ActionMode == _ActionStatus.Attack)
                {
                    StartCoroutine(QuestionIsAttack(true));
                }
            }
        }

        public IEnumerator QuestionIsAttack(bool questionFail)
        {
            yield return new WaitForSeconds(1);
            _heroIsQuesttion.Attack(_isUltimate, questionFail, blockStack);
        }

        GameObject[] _answerList;

        void GetQuestion(int row)
        {
            Transform trans = _core._questionPanel.transform.Find("Answer").Find("GridView");
            if (_answerList == null)
            {
                _answerList = new GameObject[_core._columnQuestionMax - 2];
                for (int i = 2; i < _core._questionList.GetLength(1); i++)
                {
                    GameObject slot = Instantiate(_answerSlot);
                    slot.transform.SetParent(trans);
                    slot.transform.localScale = new Vector3(1, 1, 1);
                    slot.name = (i-1).ToString();
                    Button b = slot.transform.Find("AnswerButton").GetComponent<Button>();
                    b.onClick.AddListener(delegate () { AnswerClick(_cal.IntParseFast(slot.name)); });
                    _answerList[i - 2] = slot;
                }
            }
            
            _core._questionPanelTxt.text = _core._questionList[row, 0];
            for(int i=0;i< _answerList.Length; i++)
            {
                _answerList[i].transform.Find("AnswerButton").GetComponentInChildren<ScrollRect>().content.GetComponentInChildren<Text>().text = _core._questionList[row, i+2];
            }
            //Debug.Log(_core._questionList[row, 1]);
            _answerCorrect = Int32.Parse(_core._questionList[row, 1]);
        }
        
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
            damage.transform.SetParent(GameObject.Find("Canvas").transform, false);
            damage.transform.localScale = new Vector3(1, 1, 1);
            damage.SetPopupText(dmg.ToString());
            damage.transform.position = new Vector3(pos.x, pos.y+0.25f, pos.z);
            _battleState = _BattleState.Finish;
        }

        void ShowTurnBattleNotify()
        {
            PopupTurnBattleNotify popup = Instantiate(_showTurnBattle);
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);

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
                            effect.transform.SetParent(GetMonFocus()._avatar.transform.parent);
                            effect.transform.localPosition = new Vector3(GetMonFocus()._avatar.transform.localPosition.x, GetMonFocus()._avatar.transform.localPosition.y + 0.25f, GetMonFocus()._avatar.transform.localPosition.z);
                            
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
                            effect.transform.SetParent(GetHeroFocus()._avatar.transform.parent);
                            effect.transform.localPosition = new Vector3(GetHeroFocus()._avatar.transform.localPosition.x, GetHeroFocus()._avatar.transform.localPosition.y + 0.25f, GetHeroFocus()._avatar.transform.localPosition.z);
                            
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
                GetHeroFocus().PlayInjurious(damage);
            }
            else
            {
                GetMonFocus().PlayInjurious(damage);
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
            _monFocus = 0;
            _heroFocus = 0;
            if (_roundBattle == _RoundBattle.ENEMY)
            {
                _roundBattle = _RoundBattle.PLAYER;
                _effectFocus.SetActive(true);
                LoadEvent();
                _selectATKCon.UpdateAttackSlot();
                EndTurnSetting();
                yield return new WaitForSeconds(1);
                ShowTurnBattleNotify();
                _core._playerLifePanel.transform.Find("EndTurnButton").gameObject.SetActive(true);
                _core.OpenActionPanel(_core._attackPanel);
                _isEscape = false;
                Crystal = _turnAround;
                _crystalMon = _turnAround;
                if (_core._cutscene != null)
                {
                    if (_turnAround == 2)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._mainMenuBG.transform.Find("ItemButton"), true,
                               "ดูเหมือนฮีโร่จะได้รับบาดเจ็บ ไหนลองใช้ยาฟื้นฟูเลือดดูซิ..");
                    if (_turnAround == 3)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._miniGameMenu.transform.Find("TutorialButton"), true,
                               "และในรอบต่อๆไปเจ้าจะได้คริสตัลเพิ่มขึ้น ข้าหวังว่าตอนนี้เจ้าคงพอจะเข้าใจระบบการต่อสู้ขึ้นมาบ้างแล้วซินะ หากอยากรู้อะไรเพิ่มเติมก็ลองเปิดคู่มือนักผจญภัยดูนะ");
                }
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
            _core._playerLifePanel.transform.Find("EndTurnButton").gameObject.SetActive(false);
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
                if (_monster[i].spriteSet.Contains("hero"))
                {
                    damage = 5;
                }
                for(int a=0; a< damage; a++)
                {
                    GameObject effect = Instantiate(_hitEffect);
                    effect.transform.SetParent(_monster[i]._avatar.transform);
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
                if (_monsterList[i].spriteSet.Contains("hero"))
                {
                    if (_core._unlockHeroList == null)
                    {
                        _core._unlockHeroList = new List<string>();
                    }
                    int row = 0;
                    foreach (HeroStore h in _core._heroStore)
                    {
                        row++;
                        if (h.hero.spriteName == _monsterList[i].spriteName)
                        {
                            if (Random.Range(0f, 1f) < 0.1f)
                            {
                                _core._unlockHeroList.Add(_monsterList[i].spriteName);
                            }
                            break;
                        }
                        if (row == _core._heroStore.Count)
                        {
                            _core._unlockHeroList.Add(_monsterList[i].spriteName);
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
                _core._playerSoulBar.transform.Find("ActionPointText").GetComponent<Text>().text = _crystalTotal.ToString();
            }
        }

        public Hero GetHeroFocus()
        {
            try
            {
                return _hero[_heroFocus];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
                _waitEndTurn = false;
                return _heroData[_heroFocus];
            }
        }

        public Monster GetMonFocus()
        {
            try
            {
                return _monster[_monFocus];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
                _waitEndTurn = false;
                return _monData[_monFocus];
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
            _selectATKCon.ClearAttackList();
            _buffCon._defenseList.Clear();
            
            _core._playerSoulBar.transform.Find("ActionPointText").gameObject.SetActive(false);

        }

    }
}

