using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    [SerializeField]
    private Rover _rover;

    public void OnStart()
    {
        _rover.OnStart();
    }
    
    public void OnUpdate()
    {
        _rover.OnUpdate();
    }

    public void AddTokensAtRoverPetriNet(string label, int nTokens)
    {
        _rover.AddTokensAtPlace(label, nTokens);
    }
}
