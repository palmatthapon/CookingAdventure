using system;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace controller
{
    public class CookingController : MonoBehaviour
    {
        GameCore _core;
        
        public GameObject _item;
        public GameObject _cookItem;

        public GameObject _cookSlot;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        private void OnEnable()
        {
            _core._actionMode = _ActionState.Cooking;
            _core.getItemCon().ViewItem(_item.transform,"rawmaterial");
            _core.getCampCon().setAllowTouch(false);
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

        public bool CheckTag(string tag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
#endif
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
            if (hit.transform != null && hit.transform.tag == tag)
            {
                Debug.Log("hit " + hit.transform.gameObject.name);
                return true;
            }
            return false;
        }


        private void OnDisable()
        {
            _core.getCampCon().setAllowTouch(true);
        }
    }
}

