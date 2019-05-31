using Controller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class Cutscene : MonoBehaviour
    {

        MainCore _core;
        public Text _cutsceneText;
        public Text _tutorialText;
        public Image _bg;
        public Sprite[] _bgCutscene;
        public GameObject _npc;
        public GameObject _cutsceneTalk;
        public GameObject _tutorialTalk;

        bool _isSkip = false;
        
        public GameObject _tutorialEffect;

        void Start()
        {
            _core = Camera.main.GetComponent<MainCore>();

            CutscenePlay();
            //TutorialPlay(_core._landObj.GetComponent<LandController>()._landPanel.transform.Find("GridView").Find("CampButton"), true, "เจ้าลองจิ้มตามแสงกระพริบๆ ในหน้าจอของเจ้าดูซิ มันจะทำให้เจ้าเข้าใจการเล่นเบื้องต้นมากขึ้น");
        }

        public void CutscenePlay()
        {
            StartCoroutine(Play());
        }

        IEnumerator Play()
        {
            _cutsceneText.text = "";
            AudioListener.volume = 0;
            _bg.sprite = _bgCutscene[0];
            
            _npc.SetActive(true);
            _bg.gameObject.SetActive(true);
            _cutsceneTalk.SetActive(true);
            _tutorialTalk.SetActive(false);

            string story1 = "เมื่อ 100 ปีก่อน ได้มีอุกาบาตพุ่งตกลงมายังโลก";
            for (int i = 0; i < story1.Length; i++)
            {
                _cutsceneText.text += story1[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story1;
                    break;
                }

            }
            yield return new WaitForSeconds(1f);
            string story2 = "ผลกระทบจากอุกบาตครั้งนั้นทำให้เกิดอัคคีเพลิงเผาผลาญทุกสิ่งอย่างรุนแรง";
            _cutsceneText.text = "";
            for (int i = 0; i < story2.Length; i++)
            {
                _cutsceneText.text += story2[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story2;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            string story3 = "หลังจากเพลิงสงบ ก็ได้เกิดรอยแยกมิติขึ้นบนโลก ซึ่งเชื่อมต่อกับดินแดนของปีศาจ";
            _cutsceneText.text = "";
            
            for (int i = 0; i < story3.Length; i++)
            {
                _cutsceneText.text += story3[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story3;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            string story4 = "ทำให้ปีศาจพากันบุกเข้ามาที่โลก และเข้ามาสร้างความปั่นป่วนไปทั่วสารทิศ";
            _cutsceneText.text = "";
            for (int i = 0; i < story4.Length; i++)
            {
                _cutsceneText.text += story4[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story4;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            _bg.sprite = _bgCutscene[1];
            string story5 = "เมื่อเหล่าเทพได้รับรู้ ก็ได้ส่งกองทัพเข้ามาปราบแต่แล้วก็พ่ายแพ้";
            _cutsceneText.text = "";
            for (int i = 0; i < story5.Length; i++)
            {
                _cutsceneText.text += story5[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story5;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            string story6 = "ทัพปีศาจจึงได้เข้ายึดเขาพระสุเมรุเป็นที่ตั้งฐานทัพ เพื่อจะเดินทัพเข้ายึดสวรรค์";
            _cutsceneText.text = "";
            for (int i = 0; i < story6.Length; i++)
            {
                _cutsceneText.text += story6[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story6;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            string story7 = "เหล่าฮีโร่ที่ไม่ได้ถูกความมืดครอบงำจึงได้รวมตัวกัน เพื่อเข้าจัดการปีศาจในเขาพระสุเมรุ";
            _cutsceneText.text = "";
            for (int i = 0; i < story7.Length; i++)
            {
                _cutsceneText.text += story7[i];
                yield return new WaitForSeconds(0.1f);
                if (_isSkip)
                {
                    _isSkip = false;
                    _cutsceneText.text = story7;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            AudioListener.volume = _core.dataSetting[0].soundValue;
                
            //if(_bg.gameObject.activeSelf)
                //TutorialPlay(_core._landObj.GetComponent<LandController>()._landPanel.transform.Find("GridView").Find("CampButton"), true, "เจ้าลองจิ้มตามแสงกระพริบๆในหน้าจอของเจ้าดู มันจะสอนให้เจ้าเข้าใจการเล่นเบื้องต้น");
            _bg.gameObject.SetActive(false);
            _cutsceneTalk.SetActive(false);
        }

        public void Skip()
        {
            _isSkip = true;
        }
        GameObject effect;

        public void TutorialPlay(Transform tranTarget,bool npc=false,string talk="")
        {
            if(effect == null)
            {
                effect = Instantiate(_tutorialEffect);
            }
            effect.transform.SetParent(tranTarget);
            effect.transform.localScale = new Vector3(1, 1, 1);
            effect.transform.localPosition = Vector3.zero;
            
            _tutorialText.text = talk;
            _tutorialTalk.SetActive(npc);
            _npc.SetActive(npc);
        }
        
        public void EndTutorial()
        {
            Destroy(effect);
            Destroy(this.gameObject);
        }

        public void CloseBtn()
        {
            _bg.gameObject.SetActive(false);
            _cutsceneTalk.SetActive(false);
            TutorialPlay(_core._landObj.GetComponent<LandController>()._landPanel.transform.Find("GridView").Find("CampButton"), true, "เจ้าลองจิ้มตามแสงกระพริบๆในหน้าจอของเจ้าดู มันจะสอนให้เจ้าเข้าใจการเล่นเบื้องต้น");
        }

        public void OKBtn()
        {
            _npc.SetActive(false);
        }


    }
}

