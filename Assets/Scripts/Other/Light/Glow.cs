using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour {

    Renderer _rend;
    float _colInt;
    Color _color;
    public float _minColInt = 0.5f, _maxColInt = 1f;
	void Start () {
        _rend = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        _colInt = Random.Range(_minColInt, _maxColInt);
        _color = _rend.material.color;
        _color.a = _colInt;
        _rend.material.color = _color;

    }
}
