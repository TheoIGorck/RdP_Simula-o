using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Text _title = default;
    [SerializeField] Text _description = default;
    [SerializeField] Text _instructionsText = default;
    [SerializeField] Button _start = default;
    [SerializeField] Button _instructions = default;
    [SerializeField] Button _exit = default;
    [SerializeField] Button _back = default;

    private void Start()
    {
        _start.onClick.AddListener(StartGame);
        _instructions.onClick.AddListener(GoToInstructions);
        _back.onClick.AddListener(Back);
        //_exit.onClick.AddListener(ExitGame);
    }
    
    private void StartGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    private void GoToInstructions()
    {
        _title.gameObject.SetActive(false);
        _start.gameObject.SetActive(false);
        _instructions.gameObject.SetActive(false);
        _exit.gameObject.SetActive(false);
        
        _description.gameObject.SetActive(true);
        _instructionsText.gameObject.SetActive(true);
        _back.gameObject.SetActive(true);
    }

    private void Back()
    {
        _title.gameObject.SetActive(true);
        _start.gameObject.SetActive(true);
        _instructions.gameObject.SetActive(true);
        _exit.gameObject.SetActive(true);

        _description.gameObject.SetActive(false);
        _instructionsText.gameObject.SetActive(false);
        _back.gameObject.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
