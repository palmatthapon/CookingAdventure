using system;
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
        if (_core._confirmNotifyMode == CONFIRMNOTIFY.NewGame)
        {
            _core.DeleteSave();
            foreach (Transform child in _core._mapSpace.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _core._mapSpace.transform.DetachChildren();
            _core.OpenStartScene();
            _core._subMenuPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }
        else if (_core._confirmNotifyMode == CONFIRMNOTIFY.ExitGame)
        {
            _core.OpenConfirmNotify("เจ้าต้องการเซฟเกมหรือไม่?", CONFIRMNOTIFY.SaveAndExit);

        }
        else if (_core._confirmNotifyMode == CONFIRMNOTIFY.SaveAndExit)
        {
            StartCoroutine(_core.SavePlayerData(true));
            this.gameObject.SetActive(false);
        }
    }

    public void CancelBtn()
    {
        if (_core._confirmNotifyMode == CONFIRMNOTIFY.NewGame|| _core._confirmNotifyMode == CONFIRMNOTIFY.ExitGame)
        {
            this.gameObject.SetActive(false);
        }
        else if (_core._confirmNotifyMode == CONFIRMNOTIFY.SaveAndExit)
        {
            this.gameObject.SetActive(false);
            Application.Quit();
        }
    }
}
