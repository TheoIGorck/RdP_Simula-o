using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;
    [SerializeField] private Button _restart = default;
    [SerializeField] private Button _mainMenu = default;
    [SerializeField] private Text _gameOver = default;

    private void Start()
    {
        _roverController.OnStart();
        _restart.onClick.AddListener(Restart);
        _mainMenu.onClick.AddListener(GoToMainMenu);
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
        _restart.gameObject.SetActive(true);
        _mainMenu.gameObject.SetActive(true);
        _gameOver.gameObject.SetActive(true);
    }

    private void Win()
    {
        Debug.Log("You Win!");
    }

    private void Restart()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
