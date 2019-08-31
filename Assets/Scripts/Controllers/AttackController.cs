using model;
using system;
using System.Collections.Generic;
using System.Linq;
using battle;
using UnityEngine;
using UnityEngine.UI;

namespace controller
{
    public class AttackController : MonoBehaviour
    {
        GameCore _core;
        List<AttackBlock> _attackList = new List<AttackBlock>();
        public GameObject _attackSlot;
        public Sprite[] _attackIcon;
        public Sprite[] _defenseIcon;
        public int _blockCount;
        Color32[] _color = { new Color32(130, 207, 242, 25), new Color32(196, 210, 164, 25), new Color32(200, 202, 255, 25), new Color32(254, 255, 91, 25), new Color32(251, 192, 192, 25), new Color32(195, 174, 154, 25) };

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        void LoadBlockAttack()
        {

        }
        
        public void UpdateAttackSlot()
        {
            while (_attackList.Count < 7)
            {
                LoadAttack();
            }

            UpdateBlockStack();
        }

        void UpdateBlockStack()
        {
            for (int i = 0; i < _attackList.ToList().Count; i++)
            {
                bool heroDead = true;
                if (_attackList[i].isAttack)
                {
                    foreach (Hero hero in _core.getBattCon()._hero.ToList())
                    {
                        if (hero.getStoreId() == _attackList[i].heroStoreId)
                        {
                            _attackList[i].obj.transform.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                            heroDead = false;
                            break;
                        }
                    }
                    if (heroDead)
                    {
                        if (_attackList[i].isUltimate)
                        {
                            DeleteBlock(_attackList[i]);
                            continue;
                        }
                        else
                        {
                            _attackList[i].crystal = 1;
                            _attackList[i].obj.transform.Find("Crystal").GetComponentInChildren<Text>().text = 1.ToString();
                            _attackList[i].obj.transform.Find("Image").GetComponent<Image>().color = new Color32(46, 46, 46, 255);
                        }
                    }
                }
                else
                {
                    heroDead = false;
                }
                
                if (_attackList[i].isUltimate) continue;
                if (i>0 && _attackList[i].heroStoreId == _attackList[i - 1].heroStoreId && _attackList[i].isAttack == _attackList[i - 1].isAttack)
                {
                    if (_attackList[i - 1].isUltimate) continue;
                    if (_attackList[i - 1].blockStack < 3)
                    {
                        _attackList[i].blockStack = _attackList[i - 1].blockStack + 1;
                        if (!heroDead)
                        {
                            if(_attackList[i].defCrystal>1)
                                _attackList[i].crystal = _attackList[i].defCrystal * _attackList[i].blockStack - (_attackList[i].blockStack-1);
                            else
                                _attackList[i].crystal = _attackList[i].defCrystal * _attackList[i].blockStack==3?2: _attackList[i].blockStack;
                        }
                        _attackList[i].obj.transform.Find("Crystal").GetComponentInChildren<Text>().text = _attackList[i].crystal.ToString();

#pragma warning disable CS0618 // Type or member is obsolete
                            _attackList[i - 1].obj.transform.Find("LightEffect").GetComponent<ParticleSystem>().startColor = _color[_attackList[i].color];
                            _attackList[i].obj.transform.Find("LightEffect").GetComponent<ParticleSystem>().startColor = _color[_attackList[i].color];
#pragma warning restore CS0618 // Type or member is obsolete
                            _attackList[i - 1].obj.transform.Find("LightEffect").gameObject.SetActive(true);
                            _attackList[i].obj.transform.Find("LightEffect").gameObject.SetActive(true);
                            
                    }
                    else
                    {
                        _attackList[i].obj.transform.Find("LightEffect").gameObject.SetActive(false);
                        _attackList[i].blockStack = 1;
                        if (!heroDead)
                        {
                            _attackList[i].crystal = _attackList[i].defCrystal;
                        }
                        _attackList[i].obj.transform.Find("Crystal").GetComponentInChildren<Text>().text = _attackList[i].crystal.ToString();
                    }
                }
                else
                {
                    _attackList[i].obj.transform.Find("LightEffect").gameObject.SetActive(false);
                    _attackList[i].blockStack = 1;
                    if (!heroDead)
                    {
                        _attackList[i].crystal = _attackList[i].defCrystal;
                    }
                    _attackList[i].obj.transform.Find("Crystal").GetComponentInChildren<Text>().text = _attackList[i].crystal.ToString();
                }
            }

        }
        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void LoadAttack()
        {
            if(_core ==null)
                _core = Camera.main.GetComponent<GameCore>();

            int ranSlot = Random.Range(0, _core.getBattCon()._heroCache.Length);
            Hero hero = _core.getBattCon()._heroCache[ranSlot];

            if(hero.getStatus().currentHP == 0)
            {
                //Debug.Log("old block "+ranSlot);
                ranSlot = Random.Range(0, _core.getBattCon()._heroCache.Length);
                //Debug.Log("new ran block "+ ranSlot);
            }

            GameObject slot = Instantiate(_attackSlot);
            slot.transform.SetParent(transform.Find("ActionMask").Find("GridView"));
            slot.transform.localScale = new Vector3(1, 1, 1);
            slot.transform.localPosition = Vector3.zero;
            
            if (getSpriteSet != hero.getSpriteSet())
            {
                getSpriteSet = hero.getSpriteSet();
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            
            slot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Map_" + hero.getSpriteName());
            AttackBlock atk = new AttackBlock();

            atk.slotId = _blockCount;
            atk.defCrystal = hero.getStatus().attack[0].data.crystal;
            slot.transform.Find("Crystal").GetComponentInChildren<Text>().text = atk.defCrystal.ToString();
            slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Skill_" + hero.getSpriteName());
            atk.heroStoreId = hero.getStoreId();
            atk.blockStack = 1;
            atk.color = hero.getSlot();
            atk.isAttack = true;
            atk.isUltimate = false;
            atk.crystal = atk.defCrystal;
            atk.obj = slot;
            AttackSlot atkSlot = slot.GetComponent<AttackSlot>();
            atkSlot._attack = atk;
            _attackList.Add(atk);
            _blockCount++;
            transform.Find("ActionMask").Find("GridView").localPosition = new Vector3(1, 0, 0);
        }
        
        public void LoadUltimate(int slotId)
        {
            if (_core == null)
                _core = Camera.main.GetComponent<GameCore>();
            
            GameObject slot = Instantiate(_attackSlot);
            slot.transform.SetParent(transform.Find("ActionMask").Find("GridView"));
            slot.transform.localScale = new Vector3(1, 1, 1);
            slot.transform.localPosition = Vector3.zero;

            Hero hero = _core.getBattCon()._heroCache[slotId];
            if (getSpriteSet != hero.getSpriteSet())
            {
                getSpriteSet = hero.getSpriteSet();
                loadSprite = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + getSpriteSet);
            }
            slot.transform.Find("Icon").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Map_" + hero.getSpriteName());
            slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Ultimate_" + hero.getSpriteName());
            AttackBlock atk = new AttackBlock();
            atk.slotId = _blockCount;
            atk.defCrystal = hero.getStatus().attack[1].data.crystal;
            slot.transform.Find("Crystal").GetComponentInChildren<Text>().text = atk.defCrystal.ToString();
            atk.heroStoreId = hero.getStoreId();
            atk.blockStack = 1;
            atk.color = hero.getSlot();
            atk.isAttack = true;
            atk.isUltimate = true;
            atk.crystal = atk.defCrystal;
            atk.obj = slot;
            AttackSlot atkSlot = slot.GetComponent<AttackSlot>();
            atkSlot._attack = atk;
            _attackList.Add(atk);
            _blockCount++;
            transform.Find("ActionMask").Find("GridView").localPosition = new Vector3(1, 0, 0);
        }

        public void UseAttack(AttackBlock attack)
        {
            _core._actionMode = attack.isAttack?_ActionState.Attack: _ActionState.Defense;
            
            if (_core.getMenuCon().UseCrystal(attack.crystal))
            {
                _core.getBattCon()._battleMode = _BattleState.Wait;
                if (attack.isAttack)
                {
                    bool have = false;
                    foreach (Hero hero in _core.getBattCon()._hero.ToList())
                    {
                        if (hero.getStoreId() == attack.heroStoreId)
                        {
                            have = true;
                            hero.Attack(attack.isUltimate, attack.blockStack);
                            break;
                        }
                    }
                    if (!have)
                    {
                        _core.getBattCon()._battleMode = _BattleState.Finish;
                    }
                    
                }
                DeleteBlock(attack);
                    
                UpdateAttackSlot();
            }
            else
            {
                //_core.CallSubMenu(_SubMenuState.Alert, "คริสตัลของคุณไม่พอ!");
                _core.OpenErrorNotify("คริสตัลของคุณไม่พอ!");
            }
        }

        void DeleteBlock(AttackBlock attack)
        {
            string slotIdIsDelete = "";
            for (int i = 0; i < _attackList.Count; i++)
            {
                if (_attackList[i].slotId == attack.slotId)
                {
                    if (_attackList[i].blockStack == 3)
                    {
                        Destroy(_attackList[i - 1].obj);
                        slotIdIsDelete += _attackList[i - 1].slotId + ":";
                        Destroy(_attackList[i - 2].obj);
                        slotIdIsDelete += _attackList[i - 2].slotId + ":";
                    }
                    else if (_attackList[i].blockStack == 2)
                    {
                        Destroy(_attackList[i - 1].obj);
                        slotIdIsDelete += _attackList[i - 1].slotId + ":";
                    }
                    Destroy(_attackList[i].obj);
                    slotIdIsDelete += _attackList[i].slotId + "";
                    break;
                }
            }
            string[] splitSlotId = slotIdIsDelete.Split(':');
            for (int a = 0; a < splitSlotId.Length; a++)
            {
                foreach (AttackBlock data in _attackList.ToList())
                {
                    if (data.slotId == System.Int32.Parse(splitSlotId[a]))
                    {
                        _attackList.Remove(data);
                        break;
                    }
                }
            }
        }
        
        public void ClearAttackList()
        {
            foreach (AttackBlock atk in _attackList.ToList())
            {
                Destroy(atk.obj);
            }
            _attackList.Clear();
        }
    }
}