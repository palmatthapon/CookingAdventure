using UnityEngine;
using UnityEngine.EventSystems;
using system;

namespace battle
{
    public class AttackSlot : EventTrigger
    {
        public AttackBlock _attack;
        GameCore _core;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            Debug.Log("attack clicked");
            _core.getATKCon().UseAttack(_attack);
        }

        public override void OnDeselect(BaseEventData data)
        {
            _core._infoPanel.SetActive(false);
        }
    }
}

