
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using model;
using controller;

namespace UI
{
    public class AttackSlot : EventTrigger
    {
        public SkillBlock _skill;
        GameCore _core;
        AttackController _attackCon;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _attackCon = _core._attackPanel.GetComponent<AttackController>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            Debug.Log("attack clicked");
            _attackCon.UseAttack(_skill);
        }

        public override void OnDeselect(BaseEventData data)
        {
            _core._infoPanel.SetActive(false);
        }
    }
}

