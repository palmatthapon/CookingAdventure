using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class PlayerInfoPanel : MonoBehaviour
    {
        MainCore _core;
        GameObject _objPanel;
        public GameObject _CloseBtn;

        void OnEnable()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _core._playerInfoPanel.transform.Find("GridView").Find("Money").GetComponent<Text>().text = _core._currentMoney.ToString();
        }

        public void SetObjPanel(GameObject obj)
        {
            _objPanel = obj;
            _CloseBtn.SetActive(true);
        }

        public void CloseBtn()
        {
            if (_objPanel != null)
            {
                _objPanel.SetActive(false);
                _CloseBtn.SetActive(false);
                _core._talkPanel.SetActive(false);
                _objPanel = null;
            }
        }
        private void OnDisable()
        {
            _CloseBtn.SetActive(false);
        }
    }
}