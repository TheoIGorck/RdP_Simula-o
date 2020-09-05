using RdPengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private PetriNet _robotPetriNet;

    public void OnStart()
    {
        _robotPetriNet = new PetriNet("Assets/_Project/PetriNets/Robot.pflow");
        SetPetriNetCallbacks();
    }
    
    public void OnUpdate()
    {
        _robotPetriNet.ExecCycle();
        HasRoverInNeighborhood();
    }

    public bool IsDead()
    {
        if(_robotPetriNet.GetPlaceByLabel("Dead").Tokens > 1)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            _robotPetriNet.GetPlaceByLabel("GotShot").AddTokens(1);
        }
    }

    private bool HasRoverInNeighborhood()
    {
        //Alguma lógica para saber se tem rover
        return false;
    }

    private void MoveNorth()
    {
        Debug.Log("Robot Moving North!");
    }

    private void MoveSouth()
    {
        Debug.Log("Robot Moving South!");
    }

    private void MoveWest()
    {
        Debug.Log("Robot Moving West!");
    }

    private void MoveEast()
    {
        Debug.Log("Robot Moving East!");
    }

    private void Attack()
    {
        Debug.Log("Robot Attacking!");
    }

    private void SetPetriNetCallbacks()
    {
        _robotPetriNet.GetPlaceByLabel("North").AddCallback(MoveNorth, "MoveNorth", Tokens.In);
        _robotPetriNet.GetPlaceByLabel("South").AddCallback(MoveSouth, "MoveSouth", Tokens.In);
        _robotPetriNet.GetPlaceByLabel("West").AddCallback(MoveWest, "MoveWest", Tokens.In);
        _robotPetriNet.GetPlaceByLabel("East").AddCallback(MoveEast, "MoveEast", Tokens.In);
        _robotPetriNet.GetPlaceByLabel("Attack").AddCallback(Attack, "Attack", Tokens.In);
    }
}
