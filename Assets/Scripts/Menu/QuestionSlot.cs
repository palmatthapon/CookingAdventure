
using menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace menu
{
    public class QuestionSlot : EventTrigger
    {

        GameCore _core;
        SettingPanel _settingPan;
        public string _link;

        void Start()
        {
            _core = Camera.main.GetComponent<GameCore>();
            _settingPan = _core._settingPanel.GetComponent<SettingPanel>();
        }

        float lastTimeClick;

        public override void OnPointerClick(PointerEventData data)
        {
            float currentTimeClick = data.clickTime;
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.75f)
            {
                //Debug.Log("Select");
                _settingPan.AddQuestionLink(_link);
            }
            lastTimeClick = currentTimeClick;
        }

        public override void OnDeselect(BaseEventData data)
        {
        }
    }

}
