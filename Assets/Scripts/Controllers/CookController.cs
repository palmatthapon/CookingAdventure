﻿using model;
using Model;

using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _itemCon = _core._mainMenu.GetComponent<ItemController>();
        }

        private void OnEnable()
        {
            _core._ActionMode = _ActionStatus.Cook;
            _itemCon.ViewItem(_item);
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


            if (_getRawMaterial && _putRawMaterial && _getRawMaterial.transform.localScale.x >= 0.5)
            {
                var scale = _getRawMaterial.transform.localScale;
                scale.x -= Time.deltaTime * 0.25f;
                scale.y -= Time.deltaTime * 0.25f;
                _getRawMaterial.transform.localScale = scale;
            }

        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        GameObject _getRawMaterial;
        bool _putRawMaterial;

        public void GetRawMaterial(Sprite icon)
        {
            _putRawMaterial = false;
            _getRawMaterial = Instantiate(_cookItem);
            _getRawMaterial.GetComponent<Image>().sprite = icon;
            _getRawMaterial.transform.SetParent(this.transform);
            _getRawMaterial.transform.localScale = new Vector3(1, 1, 1);
        }


        public void PutRawMaterial()
        {
            _putRawMaterial = true;
            //Destroy(_getRawMaterial, 1);
        }

        private void OnDisable()
        {
            _core.SetColliderCamp(true);
        }
    }
}

