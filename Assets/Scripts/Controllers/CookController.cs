using model;
using Model;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace controller
{
    public class CookController : MonoBehaviour
    {
        GameCore _core;
        ItemController _itemCon;

        public GameObject _item;
        public GameObject _cookItem;

        public GameObject _cookSlot;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _itemCon = _core._mainMenu.GetComponent<ItemController>();
        }

        private void OnEnable()
        {
            _core._ActionMode = _ActionStatus.Cook;
            _itemCon.ViewItem(_item,"rawmaterial");
            _core.SetColliderCamp(false);
        }
        
        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        GameObject _getRawMaterial;
        int _rawMaterialCount=0;

        public void GetRawMaterial(Sprite icon,ItemStore item)
        {
            _getRawMaterial = Instantiate(_cookItem);
            _getRawMaterial.GetComponent<Image>().sprite = icon;
            _getRawMaterial.transform.SetParent(this.transform);
            _getRawMaterial.transform.localScale = new Vector3(1, 1, 1);
            _getRawMaterial.GetComponent<RawMaterial>()._item = item;
            _getRawMaterial.GetComponent<RawMaterial>()._id = _rawMaterialCount++;
            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            newPos.z = transform.position.z;
            _getRawMaterial.transform.position = newPos;

        }

        Dictionary<int, GameObject> _cookList = new Dictionary<int, GameObject>();
        public Transform _cookListGridView;

        public void AddCookList(string name,int id)
        {
            GameObject obj = Instantiate(_cookSlot);
            obj.transform.SetParent(_cookListGridView);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponentInChildren<Text>().text = "+"+name+ " 1 piece";
            _cookList.Add(id, obj);
        }

        public void RemoveCookList(int id)
        {
            var dicArrray = _cookList.ToArray();
            foreach (var list in _cookList.ToList())
            {
                if (list.Key.Equals(id))
                {
                    Debug.Log("list "+list.Key+" "+ list.Value.name);
                    _cookList.Remove(list.Key);
                    Destroy(list.Value);
                    break;
                }
            }
            
        }

        private void OnDisable()
        {
            _core.SetColliderCamp(true);
        }
    }
}

