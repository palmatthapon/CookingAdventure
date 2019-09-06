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
        List<AttackSlot> _attackList = new List<AttackSlot>();
        public GameObject _attackSlot;
        public int _blockCount;
        Color32[] _color = { new Color32(130, 207, 242, 150), new Color32(196, 210, 164, 150), new Color32(200, 202, 255, 150), new Color32(254, 255, 91, 150), new Color32(251, 192, 192, 150), new Color32(195, 174, 154, 150) };

        private GameCore getCore()
        {
            return Camera.main.GetComponent<GameCore>();
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
                if (_attackList[i].skillSlot >= 3) continue;
                if (i>0 && _attackList[i].skillSlot == _attackList[i - 1].skillSlot)
                {
                    if (_attackList[i - 1].skillSlot >= 3) continue;
                    if (_attackList[i - 1].blockStack < 3)
                    {
                        _attackList[i].blockStack = _attackList[i - 1].blockStack + 1;

                        if (_attackList[i].defCrystal > 1)
                            _attackList[i].crystal = _attackList[i].defCrystal * _attackList[i].blockStack - (_attackList[i].blockStack - 1);
                        else
                            _attackList[i].crystal = _attackList[i].defCrystal * _attackList[i].blockStack == 3 ? 2 : _attackList[i].blockStack;

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
                        _attackList[i].crystal = _attackList[i].defCrystal;
                        _attackList[i].obj.transform.Find("Crystal").GetComponentInChildren<Text>().text = _attackList[i].crystal.ToString();
                    }
                    
                }
                else
                {
                    _attackList[i].obj.transform.Find("LightEffect").gameObject.SetActive(false);
                    _attackList[i].blockStack = 1;
                    _attackList[i].crystal = _attackList[i].defCrystal;
                    _attackList[i].obj.transform.Find("Crystal").GetComponentInChildren<Text>().text = _attackList[i].crystal.ToString();
                }
            }

        }
        Sprite[] loadSprite = null;
        string getSpriteSet = "";

        public void LoadAttack()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            int skillSlot = Random.RandomRange(0, 3);
#pragma warning restore CS0618 // Type or member is obsolete
            Hero hero = getCore().getBattCon()._currentHeroBatt;
            if (hero.getStatus().hate == 100)
            {
                skillSlot = 3;
                hero.getStatus().hate = 0;
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
            slot.transform.Find("Image").GetComponent<Image>().sprite = loadSprite.Single(s => s.name == "Skill_" + hero.getSpriteName() + "_" + skillSlot);
            AttackSlot script = slot.GetComponent<AttackSlot>();

            script.skillSlot = skillSlot;
            script.number = _blockCount;
            script.defCrystal = hero.getStatus().attack[skillSlot].crystal;
            slot.transform.Find("Crystal").GetComponentInChildren<Text>().text = script.defCrystal.ToString();
            script.blockStack = 1;
            script.color = skillSlot;
            script.crystal = script.defCrystal;
            script.obj = slot;
            
            _attackList.Add(script);
            _blockCount++;
            transform.Find("ActionMask").Find("GridView").localPosition = new Vector3(1, 0, 0);
        }

        public void UseAttack(AttackSlot attack)
        {
            getCore()._actionMode = attack.number < 3?_ActionState.Attack: _ActionState.Defense;
            
            if (getCore().getMenuCon().UseCrystal(attack.crystal))
            {
                getCore().getBattCon()._battleMode = _BattleState.Wait;

                getCore().getBattCon()._currentHeroBatt.Attack(attack.skillSlot, attack.blockStack);

                DeleteBlock(attack);
                    
                UpdateAttackSlot();
            }
            else
            {
                getCore().OpenErrorNotify("คริสตัลของคุณไม่พอ!");
            }
        }

        void DeleteBlock(AttackSlot attack)
        {
            string numberIsDelete = "";
            for (int i = 0; i < _attackList.Count; i++)
            {
                if (_attackList[i].number == attack.number)
                {
                    if (_attackList[i].blockStack == 3)
                    {
                        Destroy(_attackList[i - 1].obj);
                        numberIsDelete += _attackList[i - 1].number + ":";
                        Destroy(_attackList[i - 2].obj);
                        numberIsDelete += _attackList[i - 2].number + ":";
                    }
                    else if (_attackList[i].blockStack == 2)
                    {
                        Destroy(_attackList[i - 1].obj);
                        numberIsDelete += _attackList[i - 1].number + ":";
                    }
                    Destroy(_attackList[i].obj);
                    numberIsDelete += _attackList[i].number + "";
                    break;
                }
            }
            string[] splitSlotId = numberIsDelete.Split(':');
            for (int a = 0; a < splitSlotId.Length; a++)
            {
                foreach (AttackSlot data in _attackList.ToList())
                {
                    if (data.number == System.Int32.Parse(splitSlotId[a]))
                    {
                        _attackList.Remove(data);
                        break;
                    }
                }
            }
        }
        
        public void ClearAttackList()
        {
            foreach (AttackSlot atk in _attackList.ToList())
            {
                Destroy(atk.obj);
            }
            _attackList.Clear();
        }
    }
}