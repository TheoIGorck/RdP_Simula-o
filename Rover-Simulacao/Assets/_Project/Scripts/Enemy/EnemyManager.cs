﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Robot[] _robot = default;

    public void OnStart(int index)
    {
        _robot = GameObject.FindObjectsOfType<Robot>();

        if (_robot[index] != null)
        {
            _robot[index].OnStart();
        }
    }

    public void OnUpdate(int index)
    {
        if (_robot[index] != null)
        {
            _robot[index].OnUpdate();
        }
    }
    
    public bool IsRoverDead(int index)
    {
        return _robot[index].IsDead();
    }

    public Robot GetRobots(int index)
    {
        return _robot[index];
    }

    public int GetNumberOfRobots()
    {
        return _robot.Length;
    }
}
