using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmNotify : MonoBehaviour
{
    GameCore _core;
    void Awake()
    {
        _core = Camera.main.GetComponent<GameCore>();
    }
    public void OkayBtn()
    {
        if (_core._confirmNotifyMode == _ConfirmNotify.NewGame)
        {
            _core.DeleteSave();
            foreach (Transform child in _core._mapObj.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _core._mapObj.transform.DetachChildren();
            _core.LoadStartScene();
            _core._subMenuPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }
        else if (_core._confirmNotifyMode == _ConfirmNotify.ExitGame)
        {
            _core.OpenConfirmNotify("เจ้าต้องการเซฟเกมหรือไม่?", _ConfirmNotify.SaveAndExit);

        }
        else if (_core._confirmNotifyMode == _ConfirmNotify.SaveAndExit)
        {
            StartCoroutine(_core.SavePlayerData(true));
            this.gameObject.SetActive(false);
        }
    }

    public void CancelBtn()
    {
        if (_core._confirmNotifyMode == _ConfirmNotify.NewGame|| _core._confirmNotifyMode == _ConfirmNotify.ExitGame)
        {
            this.gameObject.SetActive(false);
        }
        else if (_core._confirmNotifyMode == _ConfirmNotify.SaveAndExit)
        {
            this.gameObject.SetActive(false);
            Application.Quit();
        }
    }
}
