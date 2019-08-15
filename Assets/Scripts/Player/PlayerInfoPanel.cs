using controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoPanel : MonoBehaviour
{
    GameCore _core;
    PlayerController _playerCon;
    public GameObject _playerIcon;

    private void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
        _playerCon = _core._mainMenu.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _playerCon.LoadPlayerIcon(false, _playerIcon);
    }
    
    public void Close()
    {
        this.gameObject.SetActive(false);
    }

}
