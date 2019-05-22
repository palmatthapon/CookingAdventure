using CollectionData;
using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamIconSlot : EventTrigger
{
    public HeroStore _hero;
    public int _teamSlot;
    TeamController _teamCon;
    MainCore _core;


    void Start()
    {
        _core = Camera.main.GetComponent<MainCore>();
        _teamCon = _core._teamPanel.GetComponent<TeamController>();
    }

    public override void OnPointerClick(PointerEventData data)
    {
        _teamCon._heroWaitRevive = _hero;
        if(_core._gameMode == _GameStatus.CAMP || _core._gameMode == _GameStatus.LAND)
        {
            if (_teamCon._heroSwapIsSelect != null && _teamCon._heroSwapIsSelect.id != 0)
            {
                _teamCon.ChangeHeroInTeam(_teamSlot);
                if (_core._cutscene != null)
                {
                    if (_teamSlot == 1)
                        _core._cutscene.GetComponent<Cutscene>().TutorialPlay(_core._landObj.GetComponent<LandController>()._landPanel.transform.Find("GridView").Find("ShopButton"),
                            true,"ตอนนี้เจ้าคงพอจะจัดการฮีโร่และทีมเป็นแล้วซินะ ต่อไปเราไปดูที่ร้านค้ากันดีกว่า");
                }
            }
        }
        
    }


    public override void OnDeselect(BaseEventData data)
    {
        
    }
}
