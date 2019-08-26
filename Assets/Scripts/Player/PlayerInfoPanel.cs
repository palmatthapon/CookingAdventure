using controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    GameCore _core;
    PlayerController _playerCon;
    public GameObject _playerProfile;
    public Text _playerName;

    private void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
        _playerCon = _core._menuPanel.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        Debug.Log("player name is " + _core._player.name);
        _playerName.text = _core._player.name;
    }
    
    public void Close()
    {
        this.gameObject.SetActive(false);
    }

}
