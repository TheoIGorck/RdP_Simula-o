using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : MonoBehaviour
{
    [SerializeField] private GameObject _waterPrefab = default;
    [SerializeField] private MapGenerator _mapGenerator = default;
    [SerializeField] private LCGRandom _random = default;
    [SerializeField] private int _width = default;
    [SerializeField] private int _height = default;
    
    private GameObject _waterObject;
    private List<GameObject> _water = new List<GameObject>();
    private int[,] _instantiatedPositions;
    
    private float _gamma = 0.9f;
    private float _beta = 0.5f;

    private char[,] _currentGrid;
    private char[,] _tempGrid;

    public void ResetFlood()
    {
        initilizeGrid();
    }

    private void Start()
    {
        initilizeGrid();
        StartCoroutine(RunStepCoroutine());
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StopCoroutine(RunStepCoroutine());
            DestroyWaterObjects();
            StartCoroutine(RunStepCoroutine());
        }
    }

    private IEnumerator RunStepCoroutine()
    {
        for (; ; )
        {
            if (_currentGrid != null)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        _tempGrid[x, y] = _currentGrid[x, y];
                    }
                }

                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        if (_currentGrid[x, y] == 'I')
                        {
                            ExposeNeighbours(x, y);
                            //TryRecovery(x, y);
                        }
                    }
                }

                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        _currentGrid[x, y] = _tempGrid[x, y];
                    }
                }
            }
            
            InstantiateWaterPrefab();

            yield return new WaitForSeconds((float)_random.Normal(10, 5));
        }
    }

    private void InstantiateWaterPrefab()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_currentGrid[x, y] == 'I' && _instantiatedPositions[x,y] != 1)
                {
                    Vector3 tilePosition = new Vector3(x + 0.5f, 1f, y + 0.5f);
                    _waterObject = Instantiate(_waterPrefab, tilePosition, Quaternion.identity);
                    _water.Add(_waterObject);
                    _instantiatedPositions[x, y] = 1;
                }
            }
        }
    }

    private void DestroyWaterObjects()
    {
        foreach(GameObject obj in _water)
        {
            Destroy(obj);
        }
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if(_currentGrid[x,y] == 'S')
                {
                    Gizmos.color = Color.gray;
                    Vector3 tilePosition = new Vector3(x + 0.5f, 5f, y + 0.5f);
                    Gizmos.DrawCube(tilePosition, Vector3.one);
                }
                /*if (_currentGrid[x, y] == 'I')
                {
                    Gizmos.color = Color.red;
                    Vector3 tilePosition = new Vector3(x + 0.5f, 5f, y + 0.5f);
                    Gizmos.DrawCube(tilePosition, Vector3.one);
                }
                if (_currentGrid[x, y] == 'R')
                {
                    Gizmos.color = Color.green;
                    Vector3 tilePosition = new Vector3(x + 0.5f, 5f, y + 0.5f);
                    Gizmos.DrawCube(tilePosition, Vector3.one);
                }*/
            }
        }
    }

    private void initilizeGrid()
    {
        _currentGrid = new char[_width, _height];
        _tempGrid = new char[_width, _height];
        _instantiatedPositions = new int[_width, _height];
        List<TileCoordinates> roomRegion = new List<TileCoordinates>();
        int startPosition = 0;

        for (int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                if (_mapGenerator.map[x, y] != 1)
                {
                    _currentGrid[x, y] = 'S';

                    roomRegion = _mapGenerator.FillRegion(x, y);
                }
            }
        }

        if (roomRegion.Count > 0)
        {
            startPosition = GetRandomNumber(0, roomRegion.Count - 1);
            _currentGrid[roomRegion[startPosition].TileX, roomRegion[startPosition].TileY] = 'I';
        }
    }

    private void ExposeNeighbours(int tileX, int tileY)
    {
        for(int x = tileX - 1; x <= tileX + 1; x++)
        {
            for(int y = tileY - 1; y <= tileY + 1; y++)
            {
                if(x != tileX || y != tileY)
                {
                    if(_mapGenerator.IsInMapRange(x,y))
                    {
                        TryFlood(x, y);
                    }
                }
            }
        }
    }

    private void TryFlood(int tileX, int tileY)
    {
        if(_currentGrid[tileX, tileY] == 'S')
        {
            if(GetRandomNumber(0, 2) < _beta)
            {
                _tempGrid[tileX, tileY] = 'I';
            }
        }
    }

    private void TryRecovery(int tileX, int tileY)
    {
        if (_currentGrid[tileX, tileY] == 'I')
        {
            if (GetRandomNumber(0, 2) < _gamma)
            {
                _tempGrid[tileX, tileY] = 'R';
            }
        }
    }
    
    private int GetRandomNumber(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

}
