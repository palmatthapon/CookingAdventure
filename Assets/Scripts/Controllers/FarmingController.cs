using system;
using UnityEngine;

namespace controller
{
    public class FarmingController : MonoBehaviour
    {
        GameCore _core;

        public GameObject _farmTool;

        GameObject _getRawMaterial;
        bool _putRawMaterial;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        void OnEnable()
        {
            _core._actionMode = _ActionState.Farming;
            _core.getCampCon().setAllowTouch(false);
        }

        void Update()
        {
            if (_getRawMaterial && !_putRawMaterial)
            {
                Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
                newPos.z = _getRawMaterial.transform.position.z;


                _getRawMaterial.transform.position = newPos;
            }

            /*
            if(_getRawMaterial && _putRawMaterial)
            {
                var rot = _getRawMaterial.transform.rotation;
                rot.z += Time.deltaTime * 0.25f;
                _getRawMaterial.transform.rotation = rot;
            }*/


        }
        
        public void GetRawMaterial()
        {
            _putRawMaterial = false;
            _getRawMaterial = Instantiate(_farmTool);
            _getRawMaterial.transform.SetParent(this.transform);
            _getRawMaterial.transform.localScale = new Vector3(1, 1, 1);
            _getRawMaterial.transform.Find("Water").gameObject.SetActive(true);
        }


        public void PutRawMaterial()
        {
            _putRawMaterial = true;
            Destroy(_getRawMaterial);
        }

        private void OnDisable()
        {
            _core.getCampCon().setAllowTouch(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}

