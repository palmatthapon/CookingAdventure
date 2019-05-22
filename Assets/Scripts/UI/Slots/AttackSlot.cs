using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class AttackSlot : EventTrigger
    {
        public SkillBlock _skill;
        MainCore _core;
        SelectAttackController _selectAtkCon;

        void Start()
        {
            _core = Camera.main.GetComponent<MainCore>();
            _selectAtkCon = _core._attackPanel.GetComponent<SelectAttackController>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            _selectAtkCon.UseAttack(_skill);
        }

        public override void OnDeselect(BaseEventData data)
        {
            _core._infoPanel.SetActive(false);
        }
    }
}

