using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _roverController.AddTokensAtRoverPetriNet("North", 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _roverController.AddTokensAtRoverPetriNet("South", 1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _roverController.AddTokensAtRoverPetriNet("West", 1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _roverController.AddTokensAtRoverPetriNet("East", 1);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            _roverController.AddTokensAtRoverPetriNet("AttackButton", 1);
            _roverController.AddTokensAtRoverPetriNet("RobotInNeighbourhood", 1);
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            _roverController.AddTokensAtRoverPetriNet("DefendButton", 1);
        }
    }
}
