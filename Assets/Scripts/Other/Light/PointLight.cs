using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLight : MonoBehaviour {

    Light _fireLight;
    float _lightInt;
    public float minimum = 3f;
    public float maximum = 5f;
    
    void Start()
    {
        _fireLight = GetComponent<Light>();
        minimum = 3f;
        maximum = 5f;
    }

    void Update()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        _lightInt = Random.RandomRange(minimum, maximum);
#pragma warning restore CS0618 // Type or member is obsolete
        _fireLight.intensity = _lightInt;
        
    }
}
