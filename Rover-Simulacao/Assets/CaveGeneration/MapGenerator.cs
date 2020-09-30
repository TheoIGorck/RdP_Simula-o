using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct TileCoordinates
{
    public int TileX;
    public int TileY;

    public TileCoordinates(int x, int y)
    {
        TileX = x;
        TileY = y;
    }
}

public class MapGenerator : MonoBehaviour {

	public int width;
    public int width2;
    public int height;
    public int height2;
    public int x1, x2;
    public string seed;
	public bool useRandomSeed;
    public GameObject myPrefab;
    public GameObject myPrefab2;
    int playerPosX, playerPosY;
    int posID;
    bool _canFill = true;

    [SerializeField]
    private List<GameObject> _fillObjects = default;
    [SerializeField]
    private List<GameObject> _fillObjects2 = default;
    [SerializeField]
    private List<GameObject> _fillObjects3 = default;
    private int _randomObject = 0;

    [Range(0,45)]
	public int randomFillPercent;

    public bool Norte, Sul, Leste, Oeste;
	public int[,] map;
    int[,] mapID;
    int ID = 0;
    GameObject[,] Map;
    List<int> randomValue = new List<int>();
    
    private void Awake()
    {
        GenerateMap();
        Draw();
        FillMapWithObjects();
    }
    
	void Update() {
        //if (Input.GetMouseButtonDown(0)) {
        //          Debug.Log("Clique");
        //          DestroyMap();
        //          GenerateMap();
        //          Draw();} 
        if (Input.GetMouseButtonDown(1)) {
            ChecarColisao(x1, x2);
        }
    }

    public void GenerateMap() {;
        ID = 0;
        width2 = width;
        height2 = height;
		map = new int[width,height];
        mapID = new int[width, height];
        Map = new GameObject[width, height];
        RandomFillMap();

		for (int i = 0; i < 1; i ++) {
			SmoothMap();
		}

        /*for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (GetSurroundingWallCount(i, j) == 2)
                    map[i, j] = 1;*/
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (map[i,j] == 0)
                    mapID[i, j] = ID++;

       /*posID = UnityEngine.Random.Range(1, ID);
                for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (mapID[i, j] == posID)
                {
                    playerPosX = i;
                    playerPosY = j;
                    //Debug.Log(playerPosX);
                    //Debug.Log(playerPosY);
                }*/

        //MeshGenerator meshGen = GetComponent<MeshGenerator>();
        //meshGen.GenerateMesh(map, 1);
    }

  public void Draw()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] == 0)
                    Map[i,j] = Instantiate(myPrefab, new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.identity);
                else if (map[i, j] == 1)
                    Map[i, j] = Instantiate(myPrefab2, new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.identity);
            }
    }
   public void ChecarColisao(int x, int y)
    {
        Norte = false;
        Sul = false;
        Leste = false;
        Oeste = false;
        if(map[x, y + 1] == 1)
        Norte = true;

        if (map[x, y - 1] == 1)
            Sul = true;

        if (map[x + 1, y] == 1)
           Leste = true;

        if (map[x - 1, y] == 1)
            Oeste = true;

        //Debug.Log(Norte);
        //Debug.Log(Sul);
        //Debug.Log(Leste);
        //Debug.Log(Oeste);
    }

    public int getPlayerPositionX()
    {
        return playerPosX;
    }

    public int getPlayerPositionY()
    {
        return playerPosY;
    }

    public void DestroyMap()
    {
        for (int i = 0; i < width2; i++)
            for (int j = 0; j < height2; j++)
            {
                Destroy(Map[i, j]);
            }
        DestroyObjects(_fillObjects3);
    }

    void RandomFillMap() {
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					map[x,y] = 1;
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0;
				}
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4)
					map[x,y] = 1;
				else if (neighbourWallTiles < 4)
					map[x,y] = 0;
			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (IsInMapRange(neighbourX, neighbourY)) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}

    /*private void ProcessMapRegions()
    {
        List<List<TileCoordinates>> wallRegions = GetRegions(1);
        int wallThresholdSize = 50;

        foreach(List<TileCoordinates> wallRegion in wallRegions)
        {
            if(wallRegion.Count < wallThresholdSize)
            {
                foreach(TileCoordinates tile in wallRegion)
                {
                    map[tile.TileX, tile.TileY] = 0;
                }
            }
        }

        List<List<TileCoordinates>> roomRegions = GetRegions(0);
        int roomThresholdSize = 50;

        foreach (List<TileCoordinates> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (TileCoordinates tile in roomRegion)
                {
                    map[tile.TileX, tile.TileY] = 1;
                }
            }
        }
    }*/

    private List<List<TileCoordinates>> GetRegions(int tileType)
    {
        List<List<TileCoordinates>> regions = new List<List<TileCoordinates>>();
        int[,] mapFlags = new int[width, height];
        
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(mapFlags[x,y] == 0 && map[x,y] == tileType)
                {
                    List<TileCoordinates> newRegion = FillRegion(x, y);
                    regions.Add(newRegion);

                    foreach(TileCoordinates tile in newRegion)
                    {
                        mapFlags[tile.TileX, tile.TileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    private List<TileCoordinates> FillRegion(int startX, int startY) 
    {
        List<TileCoordinates> tiles = new List<TileCoordinates>();
        int[,] mapFlags = new int[width, height]; //Indicates if the tile has been filled
        int tileType = map[startX, startY];

        Queue<TileCoordinates> tileQueue = new Queue<TileCoordinates>();
        tileQueue.Enqueue(new TileCoordinates(startX, startY));
        mapFlags[startX, startY] = 1;

        while(tileQueue.Count > 0)
        {
            TileCoordinates tile = tileQueue.Dequeue();
            tiles.Add(tile);

            for(int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
            {
                for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                {
                    if(IsInMapRange(x,y) && (x == tile.TileX || y == tile.TileY))
                    {
                        if(mapFlags[x,y] == 0 && map[x,y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            tileQueue.Enqueue(new TileCoordinates(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private GameObject PlaceObjectAtPosition(GameObject newObject, Vector3 position)
    {
        GameObject objectID;

        if(_fillObjects.Contains(newObject))
        {
            objectID = Instantiate(newObject, position, Quaternion.identity);
            _fillObjects.Remove(newObject);
            
            return objectID;
        }

        return null;
    }
    
    public void FillMapWithObjects()
    {
        randomValue.Clear();
        _fillObjects2.Clear();
       _fillObjects2.AddRange(_fillObjects);

        for(int i = 0; i < _fillObjects.Count; i++)
        { 
            randomValue.Add(UnityEngine.Random.Range(0, ID));
        }

        int idTile = 0;

        if (map != null && _canFill)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 0)
                    {
                        List<TileCoordinates> roomRegion = FillRegion(x, y);
                        foreach (TileCoordinates tile in roomRegion)
                        {
                            idTile++;
                            if (_fillObjects.Count > 1)
                            {
                                _randomObject = UnityEngine.Random.Range(0, _fillObjects.Count - 1);
                            }
                            else
                            {
                                _randomObject = 0;
                            }

                            if (_fillObjects.Count > 0 && randomValue.Contains(idTile))
                            {
                                _fillObjects3.Add(PlaceObjectAtPosition(_fillObjects[_randomObject], new Vector3(tile.TileX + 0.5f, 0.5f, tile.TileY + 0.5f)));
                            }
                        }
                    }
                }
            }
        }
        
        _fillObjects.AddRange(_fillObjects2);
    }

    void DestroyObjects(List<GameObject> objs)
    {
        for(int i = 0; i < objs.Count; i++)
        {
            Destroy(objs[i]);
        }

        objs.Clear();
    }

    /*private void OnDrawGizmos()
    {
        if(map != null && _canFill)
        {
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(map[x,y] == 0)
                    {
                        List<TileCoordinates> roomRegion = FillRegion(x, y);
                        foreach(TileCoordinates tile in roomRegion)
                        {
                            Vector3 tilePosition = new Vector3(tile.TileX + 0.5f, 0, tile.TileY + 0.5f);
                            Gizmos.color = Color.blue;
                            Gizmos.DrawCube(tilePosition, Vector3.one);
                        }
                    }
                }
            }
        }
    }*/
}
