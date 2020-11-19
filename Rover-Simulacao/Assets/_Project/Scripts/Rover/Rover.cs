using RdPengine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoverStatusArgs : EventArgs
{
    public int Ammo { get; }
    public int Fuel { get; }
    public int Health { get; }
    public int RescuedSoldiers { get; }
    public bool Dijkstra { get; }
    public bool Shield { get; }
    public bool EmptyShield { get; }
    

    public RoverStatusArgs(int ammo, int fuel, int health, int rescuedSoldiers, bool dijkstra, bool shield, bool emptyShield)
    {
        Ammo = ammo;
        Fuel = fuel;
        Health = health;
        RescuedSoldiers = rescuedSoldiers;
        Dijkstra = dijkstra;
        Shield = shield;
        EmptyShield = emptyShield;
    }
}

public class Rover : MonoBehaviour
{
    public GameObject MapGen;
    public MapGenerator M;
    public int MaxFillPercent = 0;

    private PetriNet _roverPetriNet;
    private float _shieldTime = 3.0f;
    private float _shieldReloadTime = 3.0f;
    private float _dijkstraReloadTime = 10.0f;
    private bool _canUseShield = true;
    private bool _canUseDijkstra = true;
    private int _posX, _posY;
    private Vector3 _direction = Vector3.zero;
    private Quaternion _newRotation = Quaternion.identity;
    [SerializeField]
    private GameObject _bullet = default;
    [SerializeField]
    private Transform _shootPoint = default;
    private int cooldown = 0;

    private int _damage = 0;
    private bool _canTakeDamage = true;
    private Coroutine _takeDamageCooldown;

    public LCGRandom randomGenerator;

    public event EventHandler<RoverStatusArgs> OnRoverStatusChanged;

    public void OnStart()
    {
        M = GameObject.Find("Generator").GetComponent<MapGenerator>();
        randomGenerator = GameObject.Find("RandomGenerator").GetComponent<LCGRandom>();

        _roverPetriNet = new PetriNet("Assets/_Project/PetriNets/Rover.pflow");
        _roverPetriNet.GetPlaceByLabel("Health").Tokens = Mathf.Max(M.MaxHealth, 0);
        _roverPetriNet.GetPlaceByLabel("Ammo").Tokens = Mathf.Max(M.MaxAmmo, 0);
        _roverPetriNet.GetPlaceByLabel("Fuel").Tokens = Mathf.Max(M.MaxFuel, 0);
        _roverPetriNet.GetConnection(_roverPetriNet.GetPlaceByLabel("RescuedSoldiers").ID, _roverPetriNet.GetTransitionByLabel("EndTransition").Id).Multiplicity = Mathf.Max(M.SoldiersQuantity.Count, 0);

        SetPetriNetCallbacks();
        SetPetriNetTransitionsPriority();

        _posX = (int)transform.position.x;
        _posY = (int)transform.position.z;
        _newRotation = transform.rotation;

        StartCoroutine(Damage());
    }

    public void OnUpdate()
    {
        cooldown--;
        //Debug.Log(_roverPetriNet.GetPlaceByLabel("North").Tokens + ": " + M.Norte);

        if (OnRoverStatusChanged != null)
        {
            OnRoverStatusChanged.Invoke(this, new RoverStatusArgs(_roverPetriNet.GetPlaceByLabel("Ammo").Tokens, _roverPetriNet.GetPlaceByLabel("Fuel").Tokens, _roverPetriNet.GetPlaceByLabel("Health").Tokens, _roverPetriNet.GetPlaceByLabel("RescuedSoldiers").Tokens, _canUseDijkstra, _roverPetriNet.GetPlaceByLabel("Defend").Tokens > 0, _roverPetriNet.GetPlaceByLabel("ReloadShield").Tokens > 0));
        }      

        if (_roverPetriNet.GetPlaceByLabel("North").Tokens == 1 && M.Norte == true)
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Collision").AddTokens(1);
        }
        else if (_roverPetriNet.GetPlaceByLabel("South").Tokens == 1 && M.Sul == true)
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Collision").AddTokens(1);
        }
        else if (_roverPetriNet.GetPlaceByLabel("West").Tokens == 1 && M.Oeste == true)
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Collision").AddTokens(1);
        }
        else if (_roverPetriNet.GetPlaceByLabel("East").Tokens == 1 && M.Leste == true)
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Collision").AddTokens(1);
        }

        _roverPetriNet.ExecCycle();
        transform.position = new Vector3(_posX + 0.5f, transform.position.y, _posY + 0.5f);
        M.ChecarColisao(_posX, _posY);
        
        /*if (Input.GetMouseButtonDown(0))
        {
            M.CreateNewLevel();
        }*/

        //Debug.Log(_damage);
    }

    public void Reset(int ammo, int fuel, int health, int soldiersQuantity)
    {
        _posX = (int)transform.position.x;
        _posY = (int)transform.position.z;
        transform.rotation = Quaternion.identity;
        _roverPetriNet.GetPlaceByLabel("Health").Tokens = Mathf.Max(health, 0);
        _roverPetriNet.GetPlaceByLabel("Ammo").Tokens = Mathf.Max(ammo, 0);
        _roverPetriNet.GetPlaceByLabel("Fuel").Tokens = Mathf.Max(fuel, 0);
        _roverPetriNet.GetPlaceByLabel("RescuedSoldiers").Tokens = 0;
        _roverPetriNet.GetPlaceByLabel("RobotInNeighbourhood").Tokens = 0;
        _roverPetriNet.GetPlaceByLabel("End").Tokens = 0;
        _roverPetriNet.GetPlaceByLabel("Quadrant:Portal").AddTokens(0);
        _roverPetriNet.GetConnection(_roverPetriNet.GetPlaceByLabel("RescuedSoldiers").ID, _roverPetriNet.GetTransitionByLabel("EndTransition").Id).Multiplicity = Mathf.Max(soldiersQuantity, 0);

        if (M.randomFillPercent < MaxFillPercent)
        {
            M.randomFillPercent++;
        }

        /*_roverPetriNet.GetPlaceByLabel("Defend").Tokens = 0;
        _roverPetriNet.GetPlaceByLabel("ShieldHasOver").Tokens = 0;
        _roverPetriNet.GetPlaceByLabel("ReloadShield").Tokens = 0;
        _roverPetriNet.GetPlaceByLabel("ShieldHasReload").Tokens = 0;*/

        if (IsDead())
        {
            _roverPetriNet.GetPlaceByLabel("Dead").Tokens = 0;
        }
    }

    public void ShowLessCostlyPath()
    {
        if (_canUseDijkstra)
        {
            M.Dijkstra(_posX, _posY);
            _roverPetriNet.GetPlaceByLabel("Dijkstra").AddTokens(1);
            _canUseDijkstra = false;
            StartCoroutine(ReloadDijkstraCountdownCoroutine());
        }
    }

    private IEnumerator ReloadDijkstraCountdownCoroutine()
    {
        while (_dijkstraReloadTime > 0 && !_canUseDijkstra)
        {
            _dijkstraReloadTime -= Time.deltaTime;

            if (_dijkstraReloadTime < 0)
            {
                _dijkstraReloadTime = 10.0f;
                _canUseDijkstra = true;
                Debug.Log("Dijkstra Available!");
            }

            yield return null;
        }
    }

    public void AddTokensAtPlace(string label, int nTokens)
    {
        _roverPetriNet.GetPlaceByLabel(label).AddTokens(nTokens);
    }

    public void RemoveTokensAtPlace(string label, int nTokens)
    {
        _roverPetriNet.GetPlaceByLabel(label).RemTokens(nTokens);
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
            M.CreateNewLevel();
            //_roverPetriNet.GetConnection(_roverPetriNet.GetPlaceByLabel("RescuedSoldiers").ID, _roverPetriNet.GetTransitionByLabel("EndTransition").Id).Multiplicity = 10;
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:RefillFuel").AddTokens(1);
            M.RemoveFromActiveList(other.gameObject);
            M.setObjectToNotActiveList(other.gameObject);
            other.gameObject.SetActive(false);
            //Debug.Log("Fuel!");
        }
        else if (other.gameObject.CompareTag("Ammo"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:ReloadAmmo").AddTokens(1);
            M.RemoveFromActiveList(other.gameObject);
            M.setObjectToNotActiveList(other.gameObject);
            other.gameObject.SetActive(false);
            //Debug.Log("Ammo!");
        }
        else if (other.gameObject.tag == "Soldier")
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Soldier").AddTokens(1);
            M.RemoveFromActiveList(other.gameObject);
            M.setObjectToNotActiveList(other.gameObject);
            other.gameObject.SetActive(false);
            //Debug.Log("Soldier!");
        }
        else if (other.gameObject.CompareTag("Portal"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Portal").AddTokens(1);
            //Debug.Log("Portal!");
        }
        else if (other.gameObject.CompareTag("EnemyBullet"))
        {
            _roverPetriNet.GetPlaceByLabel("GotShot").AddTokens(_damage);
            Destroy(other.gameObject);
            //Debug.Log("GotShot!");
        }
        else if (other.gameObject.CompareTag("Glider"))
        {
            if (cooldown < 0)
            {
                _roverPetriNet.GetPlaceByLabel("Glider").AddTokens(1);
                Debug.Log("Glider");
                cooldown = 200;
            }
        }
        else if (other.gameObject.CompareTag("EnemyNeighbourhood"))
        {
            _roverPetriNet.GetPlaceByLabel("RobotInNeighbourhood").AddTokens(1);
            //Debug.Log("Robot in Neighbourhood!");
        }
        else if (other.gameObject.CompareTag("Water"))
        {
            if (_canTakeDamage)
            {
                _canTakeDamage = false;
                _takeDamageCooldown = StartCoroutine(TakeDamageCoolDown());
            }
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            if (_canTakeDamage)
            {
                _canTakeDamage = false;
                _roverPetriNet.GetPlaceByLabel("IsDrowning").AddTokens(1);
                _takeDamageCooldown = StartCoroutine(TakeDamageCoolDown());
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyNeighbourhood"))
        {
            _roverPetriNet.GetPlaceByLabel("RobotInNeighbourhood").RemTokens(1);
        }
        else if (other.gameObject.CompareTag("Portal"))
        {
            _roverPetriNet.GetPlaceByLabel("Quadrant:Portal").RemTokens(1);
            _damage = 0;
            Debug.Log("Portal!");
        }
        if (other.gameObject.CompareTag("Water"))
        {
            StopCoroutine(_takeDamageCooldown);
            _canTakeDamage = true;
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

    private IEnumerator Damage()
    {
        for (; ;)
        {
            _damage += 1;

            yield return new WaitForSeconds((int)randomGenerator.Uniform(15, 25));
        }
    }
    
    private IEnumerator TakeDamageCoolDown()
    {
        for(; ; )
        {
            _roverPetriNet.GetPlaceByLabel("IsDrowning").AddTokens(1);

            yield return new WaitForSeconds(2f);
        }
    }

    private void MoveNorth()
    {
        if (M.Norte == false)
        {
            _posY++;
            _direction = Vector3.forward;

            if (_direction != Vector3.zero)
            {
                _newRotation = Quaternion.LookRotation(_direction);
            }

            transform.rotation = _newRotation; //Quaternion.Slerp(transform.rotation, _newRotation, _rotateSpeed * Time.deltaTime);
        }
    }

    private void MoveSouth()
    {
        if (M.Sul == false)
        { 
            _posY--;
            _direction = Vector3.back;

            if (_direction != Vector3.zero)
            {
                _newRotation = Quaternion.LookRotation(_direction);
            }

            transform.rotation = _newRotation;  //Quaternion.Slerp(transform.rotation, _newRotation, _rotateSpeed * Time.deltaTime);
        }
    }

    private void MoveWest()
    {
        if (M.Oeste == false)
        {
            _posX--;
            _direction = Vector3.left;

            if (_direction != Vector3.zero)
            {
                _newRotation = Quaternion.LookRotation(_direction);
            }

            transform.rotation = _newRotation; //Quaternion.Slerp(transform.rotation, _newRotation, _rotateSpeed * Time.deltaTime);
        }
    }

    private void MoveEast()
    {
        if (M.Leste == false)
        {
            _posX++;
            _direction = Vector3.right;

            if(_direction != Vector3.zero)
            {
                _newRotation = Quaternion.LookRotation(_direction);
            }

            transform.rotation = _newRotation; //Quaternion.Slerp(transform.rotation, _newRotation, _rotateSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        GameObject bullet = Instantiate(_bullet, _shootPoint.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(_direction * 500);
        //Debug.Log("Attacking!");
        Destroy(bullet, 2f);
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
        _roverPetriNet.GetPlaceByLabel("MoveNorth").AddCallback(MoveNorth, "MoveNorth", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("MoveSouth").AddCallback(MoveSouth, "MoveSouth", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("MoveWest").AddCallback(MoveWest, "MoveWest", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("MoveEast").AddCallback(MoveEast, "MoveEast", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Attack").AddCallback(Attack, "Attack", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("Defend").AddCallback(Defend, "Defend", Tokens.In);
        _roverPetriNet.GetPlaceByLabel("ReloadShield").AddCallback(ReloadShield, "ReloadShield", Tokens.In);
    }

    private void SetPetriNetTransitionsPriority()
    {
        _roverPetriNet.GetTransitionByLabel("AttackTransition").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("DefendTransition").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("TakeDamageTransition").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("North").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("South").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("West").Priority = 1;
        _roverPetriNet.GetTransitionByLabel("East").Priority = 1;
        //_roverPetriNet.GetTransitionByLabel("RescueTransition").Priority = 1;
    }
}
