using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCGRandom : MonoBehaviour
{
    private const double _m = 4294967296;
    private const double _a = 1664525;
    private const double _c = 1013904223;
    private double _seed;
    
    private void Update()
    {
        //Debug.Log(Normal(7, 3));
    }

    public LCGRandom()
    {
        _seed = System.DateTime.Now.Ticks % _m;
    }

    public double Uniform(double min, double max)
    {
        double u = Next();

        return min + (u * (max - min));
    }

    public double Exponential(double mean)
    {
        double u = Next();

        //Debug.Log("U: " + u);

        return -mean * Mathf.Log((float)(1 - u), 2.71828f);
    }

    public double Normal(double mean, double standdev)
    {
        double u1;
        double u2;
        double v1 = 0;
        double v2 = 0;
        double w = 2;
        double y;

        while(w > 1)
        {
            u1 = Next();
            u2 = Next();

            v1 = 2 * u1 - 1;
            v2 = 2 * u2 - 1;

            w = v1 * v1 + v2 * v2;
        }

        y = Mathf.Sqrt((-2 * Mathf.Log((float)w, 2.71828f)) / (float)w);

        //Debug.Log("Y: " + y);

        double x1 = v1 * y;

        return mean + standdev * x1;
    }

    private double Next()
    {
        for(int i = 0; i < 5; i++)
        {
            _seed = ((_a * _seed) + _c) % _m;
        }
        
        return _seed / _m;
    }

    private double Next(double maxValue)
    {
        return Next() % maxValue;
    }

    public double Seed { get => _seed; set => _seed = value; }
}
