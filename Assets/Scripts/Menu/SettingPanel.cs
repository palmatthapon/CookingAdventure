
using model;
using Model;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace menu
{
    public class SettingPanel : MonoBehaviour
    {
        GameCore _core;
        JsonReadWrite _json;

        public Slider _soundValue;
        public GameObject _questionPanel,_questionSlot;
        public Dropdown _question;
        public GameObject _questionLink;
        public InputField _questionLinkTxt;
        string _linkQuestionList = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTwp464KjsBWgIawwnygcxv2D0oSCz5gZ0WVRxj6SDDqKcB1rhzo_Df2BLPKXbzYMhVPtg35YezWEs6/pub?output=tsv";

        void OnEnable()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _json = new JsonReadWrite();
            AudioListener.volume = _core.dataSetting[0].soundValue;
            _soundValue.value = AudioListener.volume;
            _questionLinkTxt.text = _core.dataSetting[0].questionLink;
            _questionLink.transform.Find("TestConnectButton").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            _question.value = _core.dataSetting[0].question==true?1:0;

        }
        void SaveSetting()
        {
            AudioListener.volume = _soundValue.value;
            _core.dataSetting[0].soundValue = AudioListener.volume;
            _core.dataSetting[0].question = _question.value==1?true:false;
            _core.dataSetting[0].questionLink = _questionLink.GetComponentInChildren<InputField>().text;
            if(_core._questionUrl != _core.dataSetting[0].questionLink)
            {
                _core._questionUrl = _core.dataSetting[0].questionLink;
                _core.LoadQuestion();
            }
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
            if (_core._gameMode == _GameStatus.GAMEMENU)
                _core._gameMenu.SetActive(true);
            this.gameObject.SetActive(false);
            SaveSetting();
        }

        public void CancelBtn()
        {
            if (_core._gameMode == _GameStatus.GAMEMENU)
                _core._gameMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        
        public void CopyToClipboardBtn()
        {
            TextEditor te = new TextEditor();
            te.text = _questionLinkTxt.text;
            te.SelectAll();
            te.Copy();
            _core.OpenTrueNotify("คัดลอกเรียบร้อย");
        }
        
        public void AddQuestionLink(string link)
        {
            _questionLink.GetComponentInChildren<InputField>().text = link;
            _questionPanel.SetActive(false);
        }

        public void LoadQuestionCollectionBtn()
        {
            _core._loadingNotify.transform.Find("BG").GetComponentInChildren<Text>().text = "กำลังโหลดชุดข้อสอบ";
            _core._loadingNotify.SetActive(true);
            LoadQuestionCollection();
        }

        public void CloseQuestionCollectionBtn()
        {
            _questionPanel.SetActive(false);
        }

        public void TestConnectLinkBtn()
        {
            _core._loadingNotify.transform.Find("BG").GetComponentInChildren<Text>().text = "กำลังเชื่อมต่อ...";
            _core._loadingNotify.SetActive(true);
            _core.googleSheet.GetCell(_questionLink.GetComponentInChildren<InputField>().text, 9, 1, TestConnectLink);
        }

        public void TestConnectLink(string link, int c, int r, string data)
        {
            StartCoroutine(TestConnectLinkNotify(data != ""?true:false, data));
        }

        IEnumerator TestConnectLinkNotify(bool input,string count)
        {
            yield return new WaitForSeconds(1.5f);
            if (input)
            {
                _core._loadingNotify.transform.Find("BG").GetComponentInChildren<Text>().text = "เชื่อมต่อสำเร็จ! จำนวนข้อสอบ "+ count+" ข้อ";
                _questionLink.transform.Find("TestConnectButton").GetComponent<Image>().color = new Color32(130, 164, 38, 255);
            }
            else
            {
                _core._loadingNotify.transform.Find("BG").GetComponentInChildren<Text>().text = "เชื่อมต่อไม่สำเร็จ!";
                _questionLink.transform.Find("TestConnectButton").GetComponent<Image>().color = new Color32(195, 47, 47, 255);
            }
               
            yield return new WaitForSeconds(1.5f);
            _core._loadingNotify.SetActive(false);
        }

        void LoadQuestionCollection()
        {
            _core.googleSheet.GetCell(_linkQuestionList, 4, 1, SettingQuestionCollection);
        }
        int _rowQuestionListMax;
        string[,] _questionCollectionList;

        void SettingQuestionCollection(string link, int c, int r, string data)
        {
            if (data != "")
            {
                _rowQuestionListMax = Int32.Parse(data);
                _questionCollectionList = new string[_rowQuestionListMax, 2];

                //Debug.Log("row : " + mRowQuestionMax + " Array length : " + mAllQuestion.Length);

                for (int row = 0; row < _rowQuestionListMax; row++)
                {
                    for (int col = 0; col < 2; col++)
                    {
                        _core.googleSheet.GetCell(_linkQuestionList, col + 1, row + 2, AddQuestionCollection);
                    }
                }
            }
            else
            {
                Debug.Log("can't load questionCollection..");
            }
        }

        void AddQuestionCollection(string link, int c, int r, string data)
        {
            if (data != "")
            {
                //Debug.Log((r-2) + " " + (c-1) +" = "+data);
                _questionCollectionList[r - 2, c - 1] = data;
                if (c == 2 && r - 1 == _rowQuestionListMax)
                {
                    //print("load " + _rowQuestionListMax + " questionsCollection.");
                    StartCoroutine(OpenQuestionCollection());
                }
            }
            else
            {
                Debug.LogError("load questionCollection error!");
            }
        }

        IEnumerator OpenQuestionCollection()
        {
            yield return new WaitForSeconds(5);
            _core._loadingNotify.SetActive(false);
            _questionPanel.SetActive(true);
            Transform trans = _questionPanel.transform.Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < _questionCollectionList.GetLength(0); i++)
            {
                //Debug.Log("name "+_questionCollectionList[i, 0]);
                GameObject slot = Instantiate(_questionSlot);
                slot.transform.SetParent(trans);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.transform.Find("Text").GetComponent<Text>().text = _questionCollectionList[i, 0];
                slot.GetComponent<QuestionSlot>()._link = _questionCollectionList[i, 1];
            }


        }
    }

}
