using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Robot[] _robot = default;
    
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
