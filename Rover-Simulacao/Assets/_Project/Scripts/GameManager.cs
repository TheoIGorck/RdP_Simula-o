using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;
    [SerializeField]
    private EnemyManager _enemyManager = default;

    private void Awake()
    {
        _roverController.OnAwake();

        for (int i = 0; i < _enemyManager.GetNumberOfRobots(); i++)
        {
            _enemyManager.OnAwake(i);
        }
    }
    
    private void Update()
    {
        _roverController.OnUpdate();
        //_enemyManager.OnUpdate();

        if(_roverController.IsRoverDead())
        {
            GameOver();
        }
        else if(_roverController.HasRoverArrivedAtTheEnd())
        {
            Win();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }

    private void Win()
    {
        Debug.Log("You Win!");
    }
}
