using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Robot[] _robot = default;

    public void OnAwake(int index)
    {
        _robot[index].OnAwake();
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
