using controller;
using system;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    private void OnEnable()
    {
        getBattCon()._battleMode = _BattleState.Wait;
    }

    BattleController getBattCon()
    {
        return Camera.main.GetComponent<GameCore>().getBattCon();
    }

    void Update()
    {
        if (!this.GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(this.gameObject);
            getBattCon()._battleMode = _BattleState.Finish;
        }
    }
}
