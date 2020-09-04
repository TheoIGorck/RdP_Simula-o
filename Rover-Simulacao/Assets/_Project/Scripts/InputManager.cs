using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private RoverController _roverController = default;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _roverController.AddTokensAtRoverPetriNet("Norte", 1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _roverController.AddTokensAtRoverPetriNet("Sul", 1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _roverController.AddTokensAtRoverPetriNet("Oeste", 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _roverController.AddTokensAtRoverPetriNet("Leste", 1);
        }
    }
}
