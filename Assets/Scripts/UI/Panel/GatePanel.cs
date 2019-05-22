﻿using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class GatePanel : MonoBehaviour
    {
        MainCore _core;
        PlayerInfoPanel _plyInfoPan;
        Calculate _cal;

        public GameObject _gateSlot;

        void OnEnable()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _plyInfoPan = _core._playerInfoPanel.GetComponent<PlayerInfoPanel>();
            _plyInfoPan.SetObjPanel(transform.gameObject);
            _cal = new Calculate();
            OpenGate();
        }

        void OpenGate()
        {
            Transform trans = transform.Find("GridView");
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
            int count = 0;
            foreach (Dungeon data in _core._dungeon)
            {
                GameObject slot = Instantiate(_gateSlot);
                slot.transform.SetParent(trans);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.GetComponent<GateSlot>()._dungeonLayer = data.dungeon.id;
                //slot.name = data.dungeon.id.ToString();
                slot.transform.Find("Text").GetComponent<Text>().text = data.dungeon.id.ToString();
                //Button b = slot.GetComponent<Button>();
                //b.onClick.AddListener(delegate () { WarpToDungeon(data.dungeon.name); });
                if (_core._cutscene != null)
                {
                    if (count == 0)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(slot.transform);
                }
                if (data.roomIsPass.Count > 0)
                {
                    slot.transform.Find("Lock").gameObject.SetActive(false);
                }
                else
                {
                    slot.transform.Find("Lock").gameObject.SetActive(true);
                }
                count++;
            }
        }
        public int _dungeonLayerIsSelect;
    
        public void WarpToDungeon(string name)
        {
            _dungeonLayerIsSelect = _cal.IntParseFast(EventSystem.current.currentSelectedGameObject.name);
            //Debug.Log(_dungeonLayerIsSelect);
            _core.SetTalk(name+" อันตรายมากนะ ระวังด้วยพ่อหนุ่ม");
            _core.CallSubMenu(_SubMenu.Warp,"เจ้าแน่ใจแล้วใช่ไหมที่จะเข้าไป");
        }
    }
}

