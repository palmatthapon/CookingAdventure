using controller;
using system;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    private void OnEnable()
    {
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
        }
    }
}
