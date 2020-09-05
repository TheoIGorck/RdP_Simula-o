using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Robot[] _robot = default;

    public void OnStart(int index)
    {
        _robot[index].OnStart();
    }

    public void OnUpdate(int index)
    {
        _robot[index].OnUpdate();
    }
    
    public bool IsRoverDead(int index)
    {
        return _robot[index].IsDead();
    }

    public int GetNumberOfRobots()
    {
        return _robot.Length;
    }
}
