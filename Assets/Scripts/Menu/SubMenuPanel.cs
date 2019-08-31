using system;
using UnityEngine;
using UnityEngine.UI;
using warp;

public class SubMenuPanel : MonoBehaviour
{

    GameCore _core;
    string _subMenuMode;

    void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
        
    }

    private void OnEnable()
    {
        foreach(Transform child in getTrans())
        {
            child.gameObject.SetActive(false);
        }
    }

    Transform getTrans()
    {
        return this.transform.Find("GridView").transform;
    }

    public void OpenUseItem()
    {
        _subMenuMode = "useitem";
        this.gameObject.SetActive(true);
        getTrans().Find("CancelButton").gameObject.SetActive(true);
        getTrans().Find("UseButton").gameObject.SetActive(true);
    }

    public void OpenSellItem()
    {
        _subMenuMode = "sellitem";
        this.gameObject.SetActive(true);
        getTrans().Find("CancelButton").gameObject.SetActive(true);
        getTrans().Find("SellButton").gameObject.SetActive(true);
    }

    public void OpenAlert()
    {
        _subMenuMode = "alert";
        this.gameObject.SetActive(true);
        getTrans().Find("Post").gameObject.SetActive(true);
        getTrans().Find("ConfirmButton").gameObject.SetActive(true);
    }
    public void OpenBuyShop()
    {
        _subMenuMode = "buyshop";
        this.gameObject.SetActive(true);
        getTrans().Find("CancelButton").gameObject.SetActive(true);
        getTrans().Find("BuyButton").gameObject.SetActive(true);
    }
    public void OpenWarp()
    {
        _subMenuMode = "warp";
        this.gameObject.SetActive(true);
        getTrans().Find("Post").gameObject.SetActive(true);
        getTrans().Find("ConfirmButton").gameObject.SetActive(true);
        getTrans().Find("CancelButton").gameObject.SetActive(true);
    }
    public void OpenGameMenu()
    {
        _subMenuMode = "gamemenu";
        this.gameObject.SetActive(true);
        getTrans().Find("ContinueButton").gameObject.SetActive(true);
        getTrans().Find("SaveButton").gameObject.SetActive(true);
        getTrans().Find("NewGameButton").gameObject.SetActive(true);
        getTrans().Find("SettingButton").gameObject.SetActive(true);
        getTrans().Find("ExitGameButton").gameObject.SetActive(true);
    }
    public void OpenBattleEnd()
    {
        _subMenuMode = "battleend";
        this.gameObject.SetActive(true);
        getTrans().Find("Post").gameObject.SetActive(true);
        getTrans().Find("BackTownButton").gameObject.SetActive(true);
    }
    public void OpenGameOver()
    {
        _subMenuMode = "gameover";
        this.gameObject.SetActive(true);
        getTrans().Find("Post").gameObject.SetActive(true);
        getTrans().Find("ConfirmButton").gameObject.SetActive(true);
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
        _core.getMapCon()._dunBlock[_core._player.currentStayDunBlock].AddEscaped(1);
        this.gameObject.SetActive(false);
        _core.OpenScene(_GameState.LAND);
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
        _core._startMenu.SetActive(false);
    }

    public void ExitGame()
    {
        _core.OpenConfirmNotify("เจ้าแน่ใจว่าต้องการออกจากเกม?", _ConfirmNotify.ExitGame);
    }

    public void UseItem()
    {
        _core.getItemCon().UseItem();
    }

    public void Confirm()
    {
        this.gameObject.SetActive(false);
        if (_subMenuMode == "alert")
        {

        }
        else if (_subMenuMode == "warp")
        {
            _core._player.currentDungeonFloor = _core.getLandCon()._gatePanel.GetComponent<GatePanel>()._dungeonLayerIsSelect;
            _core._player.currentStayDunBlock = _core._dungeon[_core._player.currentDungeonFloor - 1].data.warpBlock;
            _core.OpenScene(_GameState.MAP);
            _core.getLandCon()._gatePanel.SetActive(false);
            _core._talkPanel.SetActive(false);
        }
        else if (_subMenuMode == "battleend")
        {
            _core.OpenScene(_GameState.MAP);
        }
        else if (_subMenuMode == "gameover")
        {
            _core.DeleteSave();
            foreach (Transform child in _core._mapSpace.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _core._mapSpace.transform.DetachChildren();
            _core.SettingBeforeStart();
            _core.OpenStartMenu();
        }
    }


    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
}
