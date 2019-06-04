
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using model;


    public class ShadowSlot : EventTrigger
    {
        public HeroStore _hero;
        CharacterController _teamCon;
        GameCore _core;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _teamCon = _core._CharacterPanel.GetComponent<CharacterController>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            //_teamCon._heroSwapIsSelect = _hero;
            //Debug.Log("hero is " + mHeroId);
            if (_core._gameMode == _GameStatus.LAND)
            {
                //_core.SetTalk("");
            }
            else
            {
                //_core._storyPanelTxt.text = "";
            }
        }


        public override void OnDeselect(BaseEventData data)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
#endif
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, -Vector3.up);
            if (!(hit.transform != null && hit.transform.tag == "IconTeam"))
            {
                //_teamCon._heroSwapIsSelect = null;
            }

        }
        
    }
