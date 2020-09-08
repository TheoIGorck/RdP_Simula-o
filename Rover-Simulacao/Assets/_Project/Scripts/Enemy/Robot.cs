using RdPengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private PetriNet _robotPetriNet;
    [SerializeField]
    private GameObject _bullet;
    [SerializeField]
    private GameObject _shootPoint;
    private Quaternion _newRotation;

    public void OnAwake()
    {
        _robotPetriNet = new PetriNet("Assets/_Project/PetriNets/Robot.pflow");
        SetPetriNetCallbacks();
        _newRotation = transform.rotation;
    }
    
    public void OnUpdate()
    {
        _robotPetriNet.ExecCycle();

        if(IsDead())
        {
            Destroy(gameObject);
        }
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
        if (other.gameObject.CompareTag("RoverBullet"))
        {
            _robotPetriNet.GetPlaceByLabel("GotShot").AddTokens(1);
            Debug.Log("Enemy Got Shoot!");
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("Rover"))
        {
            _robotPetriNet.GetPlaceByLabel("RoverInNeighbourhood").AddTokens(1);
            Debug.Log("Rover in Neighbourhood!");
            //transform.LookAt(other.gameObject.transform.position);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Rover"))
        {
            _robotPetriNet.GetPlaceByLabel("RoverInNeighbourhood").RemTokens(1);
        }
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
        GameObject bullet = Instantiate(_bullet, _shootPoint.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);
        Debug.Log("Robot Attacking!");
        Destroy(bullet, 2f);
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
