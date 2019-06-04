using model;
using Model;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    GameCore _core;
    RectTransform _rtrans;
    public GameObject _playerLifePanel;

    void Awake()
    {
        //Fetch the RectTransform from the GameObject
        _rtrans = GetComponent<RectTransform>();
        _core = Camera.main.GetComponent<GameCore>();
    }

    public GameObject _mapBtn;
    public Sprite[] _mapIcon;

    private void OnEnable()
    {
        if(_core._gameMode == _GameStatus.BATTLE)
            _mapBtn.GetComponent<Image>().sprite = _mapIcon[0];
        else
            _mapBtn.GetComponent<Image>().sprite = _mapIcon[1];
    }

    bool _switchOff=false;
    float _oldHeight;

    public void SwitchShow()
    {
        _switchOff = !_switchOff;
        if (_switchOff)
        {
            _oldHeight = _rtrans.sizeDelta.y;
            _playerLifePanel.transform.Find("UltimateSlider").gameObject.SetActive(false);
            _playerLifePanel.transform.Find("HPSlider").gameObject.SetActive(false);
            transform.Find("MenuMask").gameObject.SetActive(false);
            SetHeight(0);
        }
        else
        {
            SetHeight(_oldHeight);
            _playerLifePanel.transform.Find("UltimateSlider").gameObject.SetActive(true);
            _playerLifePanel.transform.Find("HPSlider").gameObject.SetActive(true);
            transform.Find("MenuMask").gameObject.SetActive(true);
        }
        
    }

    void SetHeight(float height)
    {
        _rtrans.sizeDelta = new Vector2(_rtrans.sizeDelta.x, height);
    }

    void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
}
