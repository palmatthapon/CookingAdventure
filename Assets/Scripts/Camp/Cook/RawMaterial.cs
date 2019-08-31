using system;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RawMaterial : EventTrigger
{
    GameCore _core;
    bool _move;
    bool _drag;
    bool _put;
    bool _add;

    public ItemStore _item;
    public int _id;

    void Start()
    {
        _drag = true;
        _core = Camera.main.GetComponent<GameCore>();
    }
    
    void Update()
    {
        if (_move && _drag)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            newPos.z = transform.position.z;


            transform.position = newPos;
        }

        if (!_drag && transform.localScale.x > 0.75)
        {
            transform.localScale = new Vector2(transform.localScale.x - Time.deltaTime * 0.5f, transform.localScale.y - Time.deltaTime * 0.5f);
        }
        else if(_drag && transform.localScale.x < 1)
        {
            var scale = transform.localScale;
            scale.x += Time.deltaTime * 0.75f;
            scale.y += Time.deltaTime * 0.75f;
            transform.localScale = scale;
        }
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _drag = true;
        _move = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _drag = false;
    }
    
    public override void OnDrag(PointerEventData data)
    {
        _drag = true;
        //Debug.Log("OnDrag");
    }

    public override void OnDrop(PointerEventData data)
    {
        _drag = false;
        _move = false;
        if (_core.getCookCon().CheckTag("Cookware"))
        {
            Debug.Log("yes");
            if (_add) return;
            _core.getCookCon().AddCookList(_item.data.name,_id);
            _add = true;
        }else
        {
            _item.obj.transform.Find("Count").GetComponent<Text>().text = (++_item.amount).ToString();
            _core.getCookCon().RemoveCookList(_id);
            _add = false;
            Destroy(this.gameObject);
        }
    }

    public override void OnEndDrag(PointerEventData data)
    {
        _drag = false;
        //Debug.Log("OnEndDrag");
        if (_core.getCookCon().CheckTag("RawMaterial"))
        {
            _item.obj.transform.Find("Count").GetComponent<Text>().text = (++_item.amount).ToString();
            _core.getCookCon().RemoveCookList(_id);
            _add = false;
            Destroy(this.gameObject);
        }
    }
    
    public override void OnPointerEnter(PointerEventData data)
    {
        _move = true;
    }
    
}
