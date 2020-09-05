using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    [SerializeField]
    private Rover _rover = default;

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

    public bool IsRoverDead()
    {
        return _rover.IsDead();
    }

    public bool HasRoverArrivedAtTheEnd()
    {
       return _rover.HasArrivedAtTheEnd();
    }
}
