using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;
    [SerializeField]
    private EnemyManager _enemyManager = default;

    private void Awake()
    {
        
    }

    private void Start()
    {
        _roverController.OnStart();
    }
    
    private void Update()
    {
        _roverController.OnUpdate();
    }
}
