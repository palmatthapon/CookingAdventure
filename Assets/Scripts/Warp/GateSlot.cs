using model;
using Model;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GateSlot : EventTrigger
{
    GameCore _core;
    public int _dungeonLayer;

    void Start () {
        _core = Camera.main.GetComponent<GameCore>();
    }

    public void WarpToDungeon()
    {

    }
    float lastTimeClick;

    public override void OnPointerClick(PointerEventData data)
    {
        //_core.SetTalk(this.gameObject.transform.Find("Text").GetComponent<Text>().text+" อันตรายมากนะ ระวังด้วยพ่อหนุ่ม");
        float currentTimeClick = data.clickTime;
        if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
        {
            if (_core._dungeon[_dungeonLayer - 1].roomIsPass.Count == 0)
            {
                _core.OpenErrorNotify("วาปชั้นนี้ยังไม่ได้เปิดใช้งาน!");
                return;
            }
            _core._player.currentDungeonFloor = _dungeonLayer;
            _core._player.currentRoomPosition = _core._dungeon[_core._player.currentDungeonFloor - 1].dungeon.startRoom;
            bool roomPass = false;
            foreach (Room room in _core._dungeon[_core._player.currentDungeonFloor - 1].roomIsPass)
            {
                if (room.id == _core._player.currentRoomPosition)
                {
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
                _core._dungeon[_core._player.currentDungeonFloor - 1].roomIsPass.Add(newRoom);
            }
            _core.LoadScene(_GameState.MAP);
            _core._gatePanel.SetActive(false);
            _core._talkPanel.SetActive(false);
        }
        lastTimeClick = currentTimeClick;
    }

    public override void OnDeselect(BaseEventData data)
    {
        //if (_core._talkPanel.activeSelf)
        //{
           // _core._talkPanel.SetActive(false);
        //}
    }
}
