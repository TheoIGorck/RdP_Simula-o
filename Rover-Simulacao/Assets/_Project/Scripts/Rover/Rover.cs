using RdPengine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rover : MonoBehaviour
{
    private PetriNet _roverPetriNet;
    private float _shieldTime = 3.0f;
    private float _shieldReloadTime = 3.0f;
    private float _moveSpeed = 5.0f;
    private bool _canUseShield = true;
    [SerializeField]
    private Transform _movePoint;
    
    public void OnAwake()
    {
        _roverPetriNet = new PetriNet("Assets/_Project/PetriNets/Rover.pflow");
        SetPetriNetCallbacks();
        SetPetriNetTransitionsPriority();
    }
    
    public void OnUpdate()
    {
        _roverPetriNet.ExecCycle();
        HasRobotInNeighborhood();
        //Debug.Log(_roverPetriNet.GetPlaceByLabel("Combustível").Tokens);
        //Debug.Log(_roverPetriNet.GetPlaceByLabel("Munição").Tokens);
    }

    public void AddTokensAtPlace(string label, int nTokens)
    {
        _roverPetriNet.GetPlaceByLabel(label).AddTokens(nTokens);
    }

    public bool IsDead()
    {
        if (_roverPetriNet.GetPlaceByLabel("Dead").Tokens > 0)
        {
            return true;
        }

        return false;
    }

    public bool HasArrivedAtTheEnd()
    {
        if (_roverPetriNet.GetPlaceByLabel("End").Tokens > 0)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:RefillFuel").AddTokens(1);
            Destroy(other.gameObject);
            Debug.Log("Fuel!");
        }
        else if (other.gameObject.CompareTag("Ammo"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:ReloadAmmo").AddTokens(1);
            Destroy(other.gameObject);
            Debug.Log("Ammo!");
        }
        else if (other.gameObject.tag == "Soldier")
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Soldier").AddTokens(1);
            Destroy(other.gameObject);
            Debug.Log("Soldier!");
        }
        else if (other.gameObject.CompareTag("Portal"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Portal").AddTokens(1);
            Debug.Log("Portal!");
        }
        else if (other.gameObject.CompareTag("EnemyBullet"))
        {
            _roverPetriNet.GetPlaceByLabel("GotShot").AddTokens(1);
            Destroy(other.gameObject);
            Debug.Log("GotShot!");
        }
    }

    private IEnumerator StartShieldCountdownCoroutine()
    {
        while(_shieldTime > 0 && _canUseShield)
        {
            _shieldTime -= Time.deltaTime;
            
            if(_shieldTime < 0)
            {
                _roverPetriNet.GetPlaceByLabel("ShieldHasOver").AddTokens(1);
                _shieldTime = 3.0f;
                _canUseShield = false;
            }

            yield return null;
        }
    }

    private IEnumerator StartReloadShieldCountdownCoroutine()
    {
        while (_shieldReloadTime > 0 && !_canUseShield)
        {
            _shieldReloadTime -= Time.deltaTime;

            if (_shieldReloadTime < 0)
            {
                _roverPetriNet.GetPlaceByLabel("ShieldHasReload").AddTokens(1);
                _shieldReloadTime = 3.0f;
                _canUseShield = true;
                Debug.Log("Shield Available!");
            }

            yield return null;
        }
    }

    private bool HasRobotInNeighborhood()
    {
        //Alguma lógica para saber se tem robô
        return false;
    }
    
    private void MoveNorth()
    {
        transform.position += Vector3.forward;
        /*transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, _moveSpeed * Time.deltaTime);
        Debug.Log("Moving North!");
        if(Vector3.Distance(transform.position, _movePoint.position) <= 0.05f)
        {
            _movePoint.position += Vector3.forward;
        }*/
    }

    private void MoveSouth()
    {
        transform.position += Vector3.back;
        /*transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, _moveSpeed * Time.deltaTime);
        Debug.Log("Moving South!");
        _movePoint.position += Vector3.back;*/
    }

    private void MoveWest()
    {
        transform.position += Vector3.left;
        /*transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, _moveSpeed * Time.deltaTime);
        Debug.Log("Moving West!");
        _movePoint.position += Vector3.left;*/
    }

    private void MoveEast()
    {
        transform.position += Vector3.right;
        /*transform.position = Vector3.MoveTowards(transform.position, _movePoint.position, _moveSpeed * Time.deltaTime);
        Debug.Log("Moving East!");
        _movePoint.position += Vector3.right;*/
    }

    private void Attack()
    {
        Debug.Log("Attacking!");
    }

    private void Defend()
    {
        Debug.Log("Defending!");
        StartCoroutine(StartShieldCountdownCoroutine());
    }

    private void ReloadShield()
    {
        Debug.Log("Reloading Shield!");
        StartCoroutine(StartReloadShieldCountdownCoroutine());
    }
    
    private void SetPetriNetCallbacks()
    {
        _roverPetriNet.GetPlaceByLabel("North").AddCallback(MoveNorth, "MoveNorth", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("South").AddCallback(MoveSouth, "MoveSouth", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("West").AddCallback(MoveWest, "MoveWest", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("East").AddCallback(MoveEast, "MoveEast", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Attack").AddCallback(Attack, "Attack", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Defend").AddCallback(Defend, "Defend", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("ReloadShield").AddCallback(ReloadShield, "ReloadShield", Tokens.In);
    }

    private void SetPetriNetTransitionsPriority()
    {
        _roverPetriNet.GetTransitionByLabel("AttackTransition").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("DefendTransition").Priority = 1;
        //_roverPetriNet.GetTransitionByLabel("RescueTransition").Priority = 1;
    }
}
