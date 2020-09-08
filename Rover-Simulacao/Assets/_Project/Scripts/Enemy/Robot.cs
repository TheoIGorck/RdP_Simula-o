using RdPengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField]
    private GameObject _bullet = default;
    [SerializeField]
    private GameObject _shootPoint = default;
    private PetriNet _robotPetriNet;
    private Quaternion _newRotation;
    private int _moveDirection;
    private int _oldDirection;
    private bool _isAttacking = false;
    private bool _changeDirection = false;

    public void OnAwake()
    {
        _robotPetriNet = new PetriNet("Assets/_Project/PetriNets/Robot.pflow");
        SetPetriNetCallbacks();
        _newRotation = transform.rotation;

        StartCoroutine(RandomizeMovePositionCoroutine());
    }
    
    public void OnUpdate()
    {
        _robotPetriNet.ExecCycle();

        if(IsDead())
        {
            StopCoroutine(AttackCoroutine());
            StopCoroutine(RandomizeMovePositionCoroutine());
            Destroy(gameObject);
            _isAttacking = false;
        }

        if(_changeDirection && !_isAttacking)
        {
            StartCoroutine(RandomizeMovePositionCoroutine());
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
        else if(other.gameObject.CompareTag("RoverNeighbourhood"))
        {
            _robotPetriNet.GetPlaceByLabel("RoverInNeighbourhood").AddTokens(1);
            Debug.Log("Rover in Neighbourhood!");
            //transform.LookAt(other.gameObject.transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("RoverNeighbourhood"))
        {
            transform.LookAt(other.gameObject.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RoverNeighbourhood"))
        {
            _robotPetriNet.GetPlaceByLabel("RoverInNeighbourhood").RemTokens(1);
            transform.rotation = Quaternion.identity;
            StopCoroutine(AttackCoroutine());
            _isAttacking = false;
        }
    }

    private void MoveNorth()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        Debug.Log("Robot Moving North!");
    }

    private void MoveSouth()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        Debug.Log("Robot Moving South!");
    }

    private void MoveWest()
    {
        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        Debug.Log("Robot Moving West!");
    }

    private void MoveEast()
    {
        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        Debug.Log("Robot Moving East!");
    }

    private void Attack()
    {
        if (!_isAttacking)
        {
            StartCoroutine(AttackCoroutine());
            _isAttacking = true;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        GameObject bullet = Instantiate(_bullet, _shootPoint.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(_shootPoint.transform.forward * 500);

        yield return new WaitForSeconds(3f);

        _isAttacking = false;
        Destroy(bullet);
    }

    private IEnumerator RandomizeMovePositionCoroutine()
    {
        _moveDirection = Random.Range(0, 4);
        Debug.Log(_moveDirection);

        if (_moveDirection != _oldDirection)
        {
            InsertDirectionInPetriNet();
        }
        _changeDirection = false;

        yield return new WaitForSeconds(1.5f);

        _changeDirection = true;
        _oldDirection = _moveDirection;
    }

    private void InsertDirectionInPetriNet()
    {
        switch (_moveDirection)
        {
            case 0:
                _robotPetriNet.GetPlaceByLabel("North").AddTokens(1);
                break;
            case 1:
                _robotPetriNet.GetPlaceByLabel("West").AddTokens(1);
                break;
            case 2:
                _robotPetriNet.GetPlaceByLabel("South").AddTokens(1);
                break;
            case 3:
                _robotPetriNet.GetPlaceByLabel("East").AddTokens(1);
                break;
        }
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
