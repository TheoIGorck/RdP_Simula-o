using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;
    [SerializeField]
    private EnemyManager _enemyManager = default;
    
    private void Start()
    {
        _roverController.OnStart();

        for (int i = 0; i < _enemyManager.GetNumberOfRobots(); i++)
        {
            _enemyManager.OnStart(i);
        }
    }

    private void Update()
    {
        _roverController.OnUpdate();
        
        for (int i = 0; i < _enemyManager.GetNumberOfRobots(); i++)
        {
            if (_enemyManager.GetRobots(i).isActiveAndEnabled)
            {
                _enemyManager.OnUpdate(i);

                if (_enemyManager.GetRobots(i).IsDead() == true)
                {
                    _roverController.RemTokensAtRoverPetriNet("RobotInNeighbourhood", 1);
                }
            }
        }
        
        if (_roverController.IsRoverDead())
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
