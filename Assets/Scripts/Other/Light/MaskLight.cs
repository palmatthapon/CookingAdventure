using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskLight : MonoBehaviour {

    //SpriteMask _fireLight;
    //float _lightInt;
    public float _speed;
    //public float minimum = 0.7f;
    //public float maximum = 0.8f;
    
    public Vector3 scale;
    public Vector3 scaleDefault;
    
    static float t = 0.0f;

    void Start () {
        //_fireLight = GetComponent<SpriteMask>();
        _speed = 4;
        scale = new Vector3(0.95f, 0.95f, 1);
        scaleDefault = new Vector3(1, 1, 1);
    }
	
	void LateUpdate () {

        //_fireLight.alphaCutoff = Mathf.Lerp(minimum, maximum, t);
        transform.localScale = Vector3.Lerp(transform.localScale, scale, t);
        
        t += _speed * Time.deltaTime;
        
        if (t > 1.0f)
        {
            Vector3 temp = scale;
            scale = scaleDefault;
            scaleDefault = temp;
            t = 0.0f;
        }
    }
}
