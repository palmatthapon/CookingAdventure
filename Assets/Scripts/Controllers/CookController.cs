using CollectionData;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookController : MonoBehaviour
{
    MainCore _core;

    public GameObject _item;
    public GameObject _cookItem;

    private void Awake()
    {
        _core = Camera.main.GetComponent<MainCore>();
    }

    private void OnEnable()
    {
        _core._ActionMode = _ActionStatus.Cook;
        _core._itemCon.ViewItem(_item);
        _core._campHeroSprite[0].GetComponent<CapsuleCollider2D>().enabled = false;
        _core._campHeroSprite[1].GetComponent<CapsuleCollider2D>().enabled = false;
        _core._campHeroSprite[2].GetComponent<CapsuleCollider2D>().enabled = false;
    }

    void Update()
    {
        if (_getRawMaterial && !_putRawMaterial)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z - _getRawMaterial.transform.position.z)));
            newPos.z = _getRawMaterial.transform.position.z;


            _getRawMaterial.transform.position = newPos;
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

    public void RemoveGetRawMaterial()
    {
        Destroy(_getRawMaterial);
    }

    public void PutRawMaterial()
    {
        _putRawMaterial = true;
    }

    private void OnDisable()
    {
        _core._campHeroSprite[0].GetComponent<CapsuleCollider2D>().enabled = true;
        _core._campHeroSprite[1].GetComponent<CapsuleCollider2D>().enabled = true;
        _core._campHeroSprite[2].GetComponent<CapsuleCollider2D>().enabled = true;
    }
}
