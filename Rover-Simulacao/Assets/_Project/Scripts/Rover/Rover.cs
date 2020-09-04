using RdPengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rover : MonoBehaviour
{
    private PetriNet _roverPetriNet;

    public void OnStart()
    {
        _roverPetriNet = new PetriNet("Assets/_Project/PetriNets/Rover.pflow");
        SetPetriNetCallbacks();
    }

    public void OnUpdate()
    {
        _roverPetriNet.ExecCycle();
    }

    public void AddTokensAtPlace(string label, int nTokens)
    {
        _roverPetriNet.GetPlaceByLabel(label).AddTokens(nTokens);
    }

    public void MoveNorth()
    {
        Debug.Log("Moving North!");
    }

    public void MoveSouth()
    {
        Debug.Log("Moving South!");
    }

    public void MoveWest()
    {
        Debug.Log("Moving West!");
    }

    public void MoveEast()
    {
        Debug.Log("Moving East!");
    }

    public void Attack()
    {

    }

    private void SetPetriNetCallbacks()
    {
        _roverPetriNet.GetPlaceByLabel("Norte").AddCallback(MoveNorth, "MoveNorth", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Sul").AddCallback(MoveSouth, "MoveSouth", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Oeste").AddCallback(MoveWest, "MoveWest", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Leste").AddCallback(MoveEast, "MoveEast", Tokens.In);
    }

    private void SetPetriNetTransitionsPriority()
    {

    }
}
