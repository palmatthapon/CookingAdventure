using controller;
using model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    BattleController _battleCon;
    private void OnEnable()
    {
        _battleCon = Camera.main.GetComponent<GameCore>()._battleCon;
        _battleCon._battleState = _BattleState.Wait;
    }
    void Update()
    {
        if (!this.GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(this.gameObject);
            _battleCon._battleState = _BattleState.Finish;
        }
    }
}
