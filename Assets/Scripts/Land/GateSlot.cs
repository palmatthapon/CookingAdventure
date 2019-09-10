using system;
using UnityEngine;
using UnityEngine.EventSystems;

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
        float currentTimeClick = data.clickTime;
        if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
        {
            if (_core._dungeon[_dungeonLayer - 1].blockIsPlayed.Count == 0)
            {
                Sprite icon = Resources.Load<Sprite>("Sprites/Icon16x16/rift16x16");
                _core.Notify(icon,"warp not active!");
                return;
            }
            _core._player.currentDungeonFloor = _dungeonLayer;
            _core._player.currentStayDunBlock = _core._dungeon[_core._player.currentDungeonFloor - 1].data.warpBlock;
            bool roomPass = false;
            foreach (DungeonBlock block in _core._dungeon[_core._player.currentDungeonFloor - 1].blockIsPlayed)
            {
                if (block.getNumber() == _core._player.currentStayDunBlock)
                {
                    roomPass = true;
                    break;
                }
            }

            if (roomPass == false)
            {
                DungeonBlock newRoom = new DungeonBlock(_core._player.currentStayDunBlock, 1,0);
                _core._dungeon[_core._player.currentDungeonFloor - 1].blockIsPlayed.Add(newRoom);
            }
            _core.OpenScene(GAMESTATE.MAP);
            _core.getLandCon()._gatePanel.SetActive(false);
            _core.getMenuCon()._itemDetail.SetActive(false);
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
