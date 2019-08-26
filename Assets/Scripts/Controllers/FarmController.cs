using model;
using Model;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace controller
{
    public class FarmController : MonoBehaviour
    {
        GameCore _core;


        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        public GameObject _farmTool;

        void OnEnable()
        {
            _core._actionMode = _ActionState.Farm;
            _core.SetColliderCamp(false);
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

        GameObject _getRawMaterial;
        bool _putRawMaterial;

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
            _core.SetColliderCamp(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}

