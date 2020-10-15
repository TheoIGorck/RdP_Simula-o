using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;
    
    private void Start()
    {
        _roverController.OnStart();
    }

    private void Update()
    {
        _roverController.OnUpdate();
        
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
