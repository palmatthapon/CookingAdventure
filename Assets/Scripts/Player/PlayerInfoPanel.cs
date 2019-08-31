using controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    GameCore _core;
    public GameObject _playerProfile;
    public Text _playerName;

    private void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
    }

    private void OnEnable()
    {
        Debug.Log("player name is " + _core._player.name);
        _playerName.text = _core._player.name;
        _core.getCampCon().setAllowTouch(false);
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
