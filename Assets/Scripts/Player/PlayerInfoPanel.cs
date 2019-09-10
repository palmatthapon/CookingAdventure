using controller;
using model;
using system;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    GameCore _core;
    public GameObject _playerProfile;
    public Text _playerName;
    public Text _content;

    private void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
    }

    private void OnEnable()
    {
        Debug.Log("player name is " + _core._player.name);
        Status status = _core._player._heroIsPlaying.getStatus();
        _playerName.text = _core._player.name;
        _core.getCampCon().setAllowTouch(false);
        _content.text = "Status\nLevel "+status.getLvl()
            +"\nATK " + status.getATK() + "   DEF " + status.getDEF()
            +"\nHP-Max "+status.currentHPMax+"   Soul "+_core._player.currentSoul
            +"\nMoney "+_core._player.currentMoney
            +"\nSkill\n 1."+ status.attack[0].name+"    2."+status.attack[1].name
            +"\n3."+status.attack[2].name+"    4."+status.attack[3].name;
    }
    
    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _core.getCampCon().setAllowTouch(true);
    }
}
