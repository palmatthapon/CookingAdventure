using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using system;

namespace warp
{
    public class GatePanel : MonoBehaviour
    {
        GameCore _core;

        public GameObject _gateSlot;

        void OnEnable()
        {
            _core = Camera.main.GetComponent<GameCore>();
            OpenGate();
            _core.getCampCon().setAllowTouch(false);
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
                slot.GetComponent<GateSlot>()._dungeonLayer = data.data.id;
                //slot.name = data.dungeon.id.ToString();
                slot.transform.Find("Text").GetComponent<Text>().text = data.data.id.ToString();
                //Button b = slot.GetComponent<Button>();
                //b.onClick.AddListener(delegate () { WarpToDungeon(data.dungeon.name); });
                
                if (data.blockIsPlayed.Count > 0)
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
            _dungeonLayerIsSelect = Calculator.IntParseFast(EventSystem.current.currentSelectedGameObject.name);
            //Debug.Log(_dungeonLayerIsSelect);
            _core.getSubMenuCore().OpenWarp();
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _core.getCampCon().setAllowTouch(true);
        }
    }
}

