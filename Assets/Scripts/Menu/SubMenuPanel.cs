using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using warp;

public class SubMenuPanel : MonoBehaviour
{

    GameCore _core;
    void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
    }

    private void OnEnable()
    {
        Transform trans = this.transform.Find("GridView").transform;
        foreach(GameObject child in this.transform.Find("GridView").transform)
        {
            child.SetActive(false);
        }
        
        if (_core._subMenuMode == _SubMenu.Item)
        {
            trans.Find("CancelButton").gameObject.SetActive(true);
            trans.Find("SellButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.Alert)
        {
            trans.Find("Post").gameObject.SetActive(true);
            trans.Find("ConfirmButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.Shop)
        {
            trans.Find("CancelButton").gameObject.SetActive(true);
            trans.Find("BuyButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.Warp)
        {
            trans.Find("Post").gameObject.SetActive(true);
            trans.Find("ConfirmButton").gameObject.SetActive(true);
            trans.Find("CancelButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.GameMenu)
        {
            trans.Find("ContinueButton").gameObject.SetActive(true);
            trans.Find("SaveButton").gameObject.SetActive(true);
            trans.Find("NewGameButton").gameObject.SetActive(true);
            trans.Find("SettingButton").gameObject.SetActive(true);
            trans.Find("ExitGameButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.BattleEnd)
        {
            trans.Find("Post").gameObject.SetActive(true);
            trans.Find("BackTownButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.ManageHero)
        {
            trans.Find("CancelButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.LoadBattleRevive)
        {
            trans.Find("Post").gameObject.SetActive(true);
            trans.Find("HeroReviveButton").gameObject.SetActive(true);
        }
        else if (_core._subMenuMode == _SubMenu.GameOver)
        {
            trans.Find("Post").gameObject.SetActive(true);
            
            trans.Find("ConfirmButton").gameObject.SetActive(true);
        }
    }

    public void setTopic(string topic)
    {
        this.transform.Find("Post").gameObject.GetComponentInChildren<Text>().text = topic;
    }

    public void Continue()
    {
        this.gameObject.SetActive(false);
    }

    public void BackTown()
    {
        //_battleCon.IsRevive = true;
        _core.CalEscapeRoom();
        this.gameObject.SetActive(false);
        _core.LoadScene(_GameState.LAND);
    }

    public void Save()
    {
        StartCoroutine(_core.SavePlayerData());
    }

    public void NewGame()
    {
        _core.OpenConfirmNotify("เจ้าแน่ใจว่าต้องการเริ่มเกมใหม่?", _ConfirmNotify.NewGame);
    }

    public void OpenSetting()
    {
        _core._settingPanel.SetActive(true);
        _core._gameMenu.SetActive(false);
    }

    public void ExitGame()
    {
        _core.OpenConfirmNotify("เจ้าแน่ใจว่าต้องการออกจากเกม?", _ConfirmNotify.ExitGame);
    }

    public void Confirm()
    {
        this.gameObject.SetActive(false);
        if (_core._subMenuMode == _SubMenu.Alert)
        {

        }
        else if (_core._subMenuMode == _SubMenu.Warp)
        {
            _core._player.currentDungeonFloor = _core._gatePanel.GetComponent<GatePanel>()._dungeonLayerIsSelect;
            _core._player.currentRoomPosition = _core._dungeon[_core._player.currentDungeonFloor - 1].dungeon.startRoom;
            _core.LoadScene(_GameState.MAP);
            _core._gatePanel.SetActive(false);
            _core._talkPanel.SetActive(false);
        }
        else if (_core._subMenuMode == _SubMenu.BattleEnd)
        {
            _core.LoadScene(_GameState.MAP);
        }
        else if (_core._subMenuMode == _SubMenu.GameOver)
        {
            _core.DeleteSave();
            foreach (Transform child in _core._mapObj.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _core._mapObj.transform.DetachChildren();
            _core.SettingBeforeStart();
            _core.OpenGameMenu();
        }
    }


    public void Cancel()
    {
        this.gameObject.SetActive(false);
        if (_core._subMenuMode == _SubMenu.Alert)
        {

        }
        else if (_core._subMenuMode == _SubMenu.ManageTeam)
        {
            //_menuPanel.transform.parent.gameObject.SetActive(true);
        }
    }
}
