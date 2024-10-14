using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPerlin : MonoBehaviour
{
    private LineRenderer _lineRender;
    public int size;
    public float _xSmooth = 0.06f;
    public float _ySmooth = 0.06f;
    void Start()
    {
        size = 150;
        _lineRender = GetComponent<LineRenderer>();
        _lineRender.positionCount = size;
        Vector3[] posArr = new Vector3[size];
        for(int i = 0; i < size; ++i)
        {
            posArr[i] = new Vector3(i * 0.1f, Mathf.PerlinNoise(i*_xSmooth*Random.Range(1,1000), i*_ySmooth* Random.Range(1, 1000)), 0);
        }
        _lineRender.SetPositions(posArr);
    }

    void Update()
    {
        
    }
}
