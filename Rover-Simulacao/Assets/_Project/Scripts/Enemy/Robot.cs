using RdPengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public GameObject MapGen;
    public MapGenerator M;
    private bool Norte = false;
    private bool Sul = false;
    private bool Leste = false;
    private bool Oeste = false;
    public int xPosition = 0;
    public int yPosition = 0;
    public Transform RoverTransform;

    [SerializeField]
    private GameObject _bullet = default;
    [SerializeField]
    private GameObject _shootPoint = default;
    private PetriNet _robotPetriNet;
    //private Quaternion _newRotation;
    private int _moveDirection;
    private int _oldDirection;
    private bool _isAttacking = false;
    private bool _changeDirection = false;

    public void OnStart()
    {
        _robotPetriNet = new PetriNet("Assets/_Project/PetriNets/Robot.pflow");
        SetPetriNetCallbacks();
        //_newRotation = transform.rotation;
        M = GameObject.Find("Generator").GetComponent<MapGenerator>();
        xPosition = (int)transform.position.x;
        yPosition = (int)transform.position.z;
        StartCoroutine(RandomizeMovePositionCoroutine());
    }
    
    public void OnUpdate()
    {
        if (!IsDead())
        {
            _robotPetriNet.ExecCycle();
            ChecarColisao(xPosition, yPosition);
            transform.position = new Vector3(xPosition + 0.5f, transform.position.y, yPosition + 0.5f);

            if (_changeDirection && !_isAttacking)
            {
                StartCoroutine(RandomizeMovePositionCoroutine());
            }
        }
        else if (IsDead())
        {
            StopCoroutine(AttackCoroutine());
            StopCoroutine(RandomizeMovePositionCoroutine());
            M.RemoveFromActiveList(gameObject);
            M.setObjectToNotActiveList(gameObject);
            gameObject.SetActive(false);
            _isAttacking = false;
        }
    }

    public void Reset()
    {
        xPosition = (int)transform.position.x;
        yPosition = (int)transform.position.z;
        transform.rotation = Quaternion.identity;
        _robotPetriNet.GetPlaceByLabel("Health").Tokens = 5;
        _robotPetriNet.GetPlaceByLabel("RoverInNeighbourhood").Tokens = 0;

        if (IsDead())
        {
            _robotPetriNet.GetPlaceByLabel("Dead").Tokens = 0;
            StartCoroutine(RandomizeMovePositionCoroutine());
            //Debug.Log("Voltei a vida!");
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
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("RoverNeighbourhood"))
        {
            _robotPetriNet.GetPlaceByLabel("RoverInNeighbourhood").AddTokens(1);
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
        }
    }

    public void ChecarColisao(int x, int y)
    {
        Norte = false;
        Sul = false;
        Leste = false;
        Oeste = false;
        if (M.map[x, y + 1] == 1)
            Norte = true;
        
        if (M.map[x, y - 1] == 1)
            Sul = true;

        if (M.map[x + 1, y] == 1)
            Leste = true;

        if (M.map[x - 1, y] == 1)
            Oeste = true;
    }

    /*private bool CheckIsPlayerVisible()
    {
        RaycastHit hit;
        Vector3 direction = RoverTransform.position - transform.position;
        Physics.Raycast(transform.position, direction / 2, out hit);

        Debug.DrawRay(transform.position, direction / 2, Color.red);

        if (hit.transform != null)
        {
            if (hit.transform.gameObject.CompareTag("Rover"))
            {
                return true;
            }
        }

        return false;
    }*/

    private void MoveNorth()
    {
        yPosition++;
    }

    private void MoveSouth()
    {
        yPosition--;
    }

    private void MoveWest()
    {
        xPosition--;
    }

    private void MoveEast()
    {
        xPosition++;
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
        if (!IsDead())
        {
            GameObject bullet = Instantiate(_bullet, _shootPoint.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(_shootPoint.transform.forward * 500);

            yield return new WaitForSeconds(1.0f);

            _isAttacking = false;
            Destroy(bullet);
        }
    }

    private IEnumerator RandomizeMovePositionCoroutine()
    {
        if (!IsDead())
        {
            _moveDirection = Random.Range(0, 4);

            /*if (_moveDirection != _oldDirection)
            {

            }*/

            InsertDirectionInPetriNet();

            _changeDirection = false;

            yield return new WaitForSeconds(0.5f);

            _changeDirection = true;
            //_oldDirection = _moveDirection;
        }
    }

    private void InsertDirectionInPetriNet()
    {
        switch (_moveDirection)
        {
            case 0:
                if (Norte == false)
                {
                    _robotPetriNet.GetPlaceByLabel("North").AddTokens(1);
                }
                break;
            case 1:
                if (Oeste == false)
                {
                    _robotPetriNet.GetPlaceByLabel("West").AddTokens(1);
                }
                break;
            case 2:
                if (Sul == false)
                {
                    _robotPetriNet.GetPlaceByLabel("South").AddTokens(1);
                }
                break;
            case 3:
                if (Leste == false)
                {
                    _robotPetriNet.GetPlaceByLabel("East").AddTokens(1);
                }
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
