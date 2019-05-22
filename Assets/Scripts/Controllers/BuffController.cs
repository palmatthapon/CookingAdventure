using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuffController : MonoBehaviour
{

    MainCore _core;
    BattleController _battleCon;
    public GameObject _buffSlot;
    public GameObject _defenseSlot;
    public List<Buff> _buffListPlayer;
    public List<Defense> _defenseList;
    public List<Buff> _buffListMonster;

    Sprite[] loadSpriteBuff = null;
    Transform trans;

    private void Awake()
    {
        
    }

    private void Start()
    {
        _core = Camera.main.GetComponent<MainCore>();
        _battleCon = _core._battleCon;
        loadSpriteBuff = Resources.LoadAll<Sprite>("Sprites/UI/buff");
    }

    public void AddBuff(Buff buff)
    {
        if (buff.id == 0) return;
        bool haveBuff = false;
        int EulerZ = 0;
        bool targetPlayer = false;
        if ((buff.whoUse == _Model.PLAYER && buff.forMe) || (buff.whoUse == _Model.MONSTER && !buff.forMe))
        {
            foreach (Buff b in _buffListPlayer)
            {
                if (b.id == buff.id)
                {
                    if (b.obj == null) break;
                    b.obj.transform.Find("Text").GetComponent<Text>().text = buff.timeCount.ToString();
                    b.startTime = buff.startTime;
                    haveBuff = true;
                    break;
                }
            }
            trans = _core._actionPointPanel.transform.Find("BuffPanel").Find("GridView");
            EulerZ = -180;
            targetPlayer = true;
        }
        else
        {
            foreach (Buff b in _buffListMonster)
            {
                if (b.id == buff.id)
                {
                    if (b.obj == null) break;
                    b.obj.transform.Find("Text").GetComponent<Text>().text = buff.timeCount.ToString();
                    b.startTime = buff.startTime;
                    haveBuff = true;
                    break;
                }
            }
            trans = _core._monPanel.transform.Find("BuffPanel").Find("GridView");
            EulerZ = 180;
            targetPlayer = false;
        }
        if (haveBuff) { _battleCon._battleState = _BattleState.Finish; return; }
        GameObject slot = Instantiate(_buffSlot);
        slot.transform.SetParent(trans);
        slot.transform.localScale = new Vector3(1, 1, 1);
        slot.transform.localRotation = Quaternion.Euler(0, 0, EulerZ);
        slot.GetComponent<Image>().sprite = loadSpriteBuff.Single(s => s.name == "buff_" + (buff.icon - 1));
        slot.transform.Find("Text").GetComponent<Text>().text = buff.timeCount.ToString();
        buff.remove = false;
        buff.obj = slot;
        (targetPlayer ? _buffListPlayer : _buffListMonster).Add(buff);
        OnBuffFunction(((_Buff)buff.id).ToString(), buff);
        _battleCon._battleState = _BattleState.Finish;
    }

    public void AddDefense(int crystal)
    {
        Defense data = new Defense();
        data.id = _defenseList.Count;
        data.crystal = crystal;
        GameObject slot = Instantiate(_defenseSlot);
        slot.transform.SetParent(_core._actionPointPanel.transform.Find("DefensePanel").Find("GridView"));
        slot.transform.localScale = new Vector3(1, 1, 1);
        slot.GetComponent<Image>().sprite = loadSpriteBuff.Single(s => s.name == "buff_0");
        slot.transform.Find("Text").GetComponent<Text>().text = crystal + "";
        data.obj = slot;
        if(_defenseList.Count == 5)
        {
            Destroy(_defenseList[0].obj);
            _defenseList.RemoveAt(0);
        }
        _defenseList.Add(data);
        _battleCon._battleState = _BattleState.Finish;
    }

    public void RemoveDefense(int slot = 0)
    {
        if (_defenseList.ToList().Count == 0) return;
        Destroy(_defenseList[slot].obj);
        _defenseList.RemoveAt(slot);
    }

    public void ClearDefense()
    {
        foreach (Transform tran in _core._actionPointPanel.transform.Find("DefensePanel").Find("GridView"))
        {
            Destroy(tran.gameObject);
        }
        _buffListPlayer.Clear();
    }

    public void RemoveBuff(Buff buff)
    {
        buff.remove = true;
        OnBuffFunction(((_Buff)buff.id).ToString(), buff);
        Destroy(buff.obj);
        _buffListPlayer.Remove(buff);
    }

    public void RemoveBuff(int id, _Model target)
    {
        if (target == _Model.PLAYER)
        {
            foreach (Buff buff in _buffListPlayer.ToList())
            {
                if (id == buff.id)
                {
                    RemoveBuff(buff);
                    break;
                }

            }
        }
        else
        {
            foreach (Buff buff in _buffListMonster.ToList())
            {
                if (id == buff.id)
                {
                    RemoveBuff(buff);
                    break;
                }
            }
        }
    }

    public void RemoveBuffAll(_Model target)
    {
        if (target == _Model.PLAYER)
        {
            foreach (Buff buff in _buffListPlayer.ToList())
            {
                RemoveBuff(buff);
            }
        }
        else
        {
            foreach (Buff buff in _buffListMonster.ToList())
            {
                RemoveBuff(buff);
            }
        }

    }

    public void ClearBuffAll(_Model target)
    {
        if (target == _Model.PLAYER)
        {
            foreach (Buff buff in _buffListPlayer.ToList())
            {
                Destroy(buff.obj);
            }
            _buffListPlayer.Clear();
        }
        else
        {
            foreach (Buff buff in _buffListMonster.ToList())
            {
                Destroy(buff.obj);
            }
            _buffListMonster.Clear();
        }

    }

    private void OnBuffFunction(string methodName, params object[] parameter)
    {
        var myClass = new BuffActive();

        var method = myClass.GetType().GetMethod(methodName);
        if (method != null)
            method.Invoke(myClass, parameter);
    }
}
