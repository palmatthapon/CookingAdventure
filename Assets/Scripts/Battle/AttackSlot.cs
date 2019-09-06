using UnityEngine;
using UnityEngine.EventSystems;
using system;

namespace battle
{
    public class AttackSlot : EventTrigger
    {
        GameCore _core;
        public int number;
        public int skillSlot;
        public int color;
        public int defCrystal;
        public int crystal;
        public int blockStack;
        public GameObject obj;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        public override void OnPointerClick(PointerEventData data)
        {
            _core.getATKCon().UseAttack(this);
        }

        public override void OnDeselect(BaseEventData data)
        {
            _core._infoPanel.SetActive(false);
        }
    }
}

