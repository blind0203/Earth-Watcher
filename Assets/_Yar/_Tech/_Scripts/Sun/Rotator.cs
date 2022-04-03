using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Material _nightCityMat;
    void Update()
    {
        transform.eulerAngles += transform.up * _speed * Time.deltaTime;

        if (_nightCityMat) 
        {
            _nightCityMat.SetVector("_SunDirection", transform.forward);
        }
    }
}
