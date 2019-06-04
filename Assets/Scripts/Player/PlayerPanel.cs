
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace player
{
    public class PlayerPanel : MonoBehaviour
    {

        GameCore _core;
        public GameObject _light;
        public Slider _ultimageBar;
        public Image _gateHate;

        void OnEnable()
        {

        }

        public void UpdateHate(int hate,int hcd)
        {
            if (hate >= 100)
            {
                if (!_light.activeSelf)
                    _light.SetActive(true);
                if (_gateHate.color != Color.red)
                    _gateHate.color = Color.red;
                _ultimageBar.value = (float)hcd / 100;
            }
            else
            {
                if (_light.activeSelf)
                    _light.SetActive(false);
                if (_gateHate.color != Color.yellow)
                    _gateHate.color = Color.yellow;
                _ultimageBar.value = (float)hate / 100;
            }
        }
        
    }
}

