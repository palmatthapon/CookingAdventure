using system;
using UnityEngine;
using UnityEngine.UI;

namespace menu
{
    public class SettingPanel : MonoBehaviour
    {
        GameCore _core;
        JsonReadWrite _json;

        public Slider _soundValue;
        

        void OnEnable()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _json = new JsonReadWrite();
            AudioListener.volume = _core.dataSetting[0].soundValue;
            _soundValue.value = AudioListener.volume;

        }
        void SaveSetting()
        {
            AudioListener.volume = _soundValue.value;
            _core.dataSetting[0].soundValue = AudioListener.volume;
            
            _json.WriteSetting(_core.dataSetting); 
        }

        public void HowtoBtn()
        {
            Application.OpenURL("https://www.facebook.com/3worldadventure/photos/pcb.761335390916356/761334844249744/?type=3&theater");
        }

        public void FacebookBtn()
        {
            Application.OpenURL("https://www.facebook.com/3worldadventure");
        }

        public void ConfirmBtn()
        {
            if (_core._gameMode == _GameState.GAMEMENU)
                _core._startMenu.SetActive(true);
            this.gameObject.SetActive(false);
            SaveSetting();
        }

        public void CancelBtn()
        {
            if (_core._gameMode == _GameState.GAMEMENU)
                _core._startMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        
        public void CopyToClipboardBtn()
        {
            TextEditor te = new TextEditor();
            te.text = "";
            te.SelectAll();
            te.Copy();
            _core.OpenTrueNotify("คัดลอกเรียบร้อย");
        }
        
        public void TestConnectLinkBtn()
        {
            _core._loadingNotify.transform.Find("BG").GetComponentInChildren<Text>().text = "กำลังเชื่อมต่อ...";
            _core._loadingNotify.SetActive(true);
        }
        
    }

}
