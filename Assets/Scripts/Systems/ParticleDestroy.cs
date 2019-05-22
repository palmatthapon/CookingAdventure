using Controller;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    BattleController _battleCon;
    private void OnEnable()
    {
        _battleCon = Camera.main.GetComponent<MainCore>()._battleCon;
        _battleCon._battleState = CollectionData._BattleState.Wait;
    }
    void Update()
    {
        if (!this.GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(this.gameObject);
            _battleCon._battleState = CollectionData._BattleState.Finish;
        }
    }
}
