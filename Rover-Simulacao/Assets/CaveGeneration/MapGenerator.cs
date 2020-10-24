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

public class Room : IComparable<Room>
{
    public List<TileCoordinates> tiles;
    public List<TileCoordinates> edgeTiles;
    public List<Room> connectedRooms;
    public int roomSize;
    public bool isAccessibleFromMainRoom;
    public bool isMainRoom;

    public Room()
    {

    }

    public Room(List<TileCoordinates> roomTiles, int[,] map)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();

        edgeTiles = new List<TileCoordinates>();
        foreach(TileCoordinates tile in tiles)
        {
            for(int x = tile.TileX - 1; x < tile.TileX + 1; x++)
            {
                for(int y = tile.TileY; y < tile.TileY + 1; y++)
                {
                    if(x == tile.TileX || y == tile.TileY)
                    {
                        if(map[x, y] == 1)
                        {
                            edgeTiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    public void SetAccessibleFromMainRoom()
    {
        if(!isAccessibleFromMainRoom)
        {
            isAccessibleFromMainRoom = true;
            foreach(Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if(roomA.isAccessibleFromMainRoom)
        {
            roomB.SetAccessibleFromMainRoom();
        }
        else if(roomB.isAccessibleFromMainRoom)
        {
            roomA.SetAccessibleFromMainRoom();
        }

        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }

    public bool IsConnected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }

    public int CompareTo(Room otherRoom)
    {
        return otherRoom.roomSize.CompareTo(roomSize);
    }
}

public class MapGenerator : MonoBehaviour {

    public int level = 0;
    public int MaxLevels = 0;
    public GameObject Soldier;
    public GameObject Robot;

    public int width;
    private int width2;
    public int height;
    private int height2;
    public int x1, x2;
    public string seed;
	public bool useRandomSeed;
    public GameObject myPrefab;
    public GameObject myPrefab2;
    int playerPosX, playerPosY;
    int posID;
    bool _canFill = true;
    
    public int MaxAmmo = 0;
    public int MaxHealth = 0;
    public int MaxFuel = 0;

    [SerializeField]
    public List<GameObject> SoldiersQuantity = new List<GameObject>();
    [SerializeField]
    public List<GameObject> RobotsQuantity = new List<GameObject>();

    [SerializeField]
    private List<GameObject> _fillObjects = default;
    [SerializeField]
    private List<GameObject> _notActiveObjects = default;
    [SerializeField]
    private List<GameObject> _activeObjects = default;
    private int _randomObject = 0;

    private List<int> randomPosition = new List<int>();
    [SerializeField]
    private List<GameObject> _replacedObjects = new List<GameObject>();

    [Range(0, 100)]
	public int randomFillPercent;

    public bool Norte, Sul, Leste, Oeste;
	public int[,] map;
    int[,] mapID;
    int ID = 0;
    GameObject[,] Map;
    List<int> randomValue = new List<int>();

    private int[,] _costMap;
    private int[,] _distanceMap;
    private Dictionary<Vector2, int> _minCost = new Dictionary<Vector2, int>();
    private Dictionary<Vector2, Vector2> _prevPos = new Dictionary<Vector2, Vector2>();
    private List<Vector2> _visitedTiles = new List<Vector2>();
    List<Vector2> _finishedPath = new List<Vector2>(); //Lista de posições que montam o caminho;
    public GameObject _from;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        GenerateMap();
        Draw();
        FillMapWithObjects();
        _propertyBlock = new MaterialPropertyBlock();

        _from = GameObject.Find("Rover(Clone)");
    }
    
	void Update() {
        //if (Input.GetMouseButtonDown(0)) {
        //          Debug.Log("Clique");
        //          DestroyMap();
        //          GenerateMap();
        //          Draw();} 
        /*if (Input.GetMouseButtonDown(1)) {
            ChecarColisao(x1, x2);
        }*/
    }

    public void GenerateMap() {;
        ID = 0;
        width2 = width;
        height2 = height;
		map = new int[width,height];
        mapID = new int[width, height];
        Map = new GameObject[width, height];
        RandomFillMap();

		for (int i = 0; i < 2; i ++) {
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
        
        ProcessMapRegions();
        GenerateRandomCostMatrix();
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
        //DestroyObjects(_fillObjects3);
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

    private void ProcessMapRegions()
    {
        List<List<TileCoordinates>> wallRegions = GetRegions(1);
        int wallThresholdSize = 5;

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
        int roomThresholdSize = 5;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<TileCoordinates> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (TileCoordinates tile in roomRegion)
                {
                    map[tile.TileX, tile.TileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }

    private void ConnectClosestRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if(forceAccessibilityFromMainRoom)
        {
            foreach(Room room in rooms)
            {
                if(room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = rooms;
            roomListB = rooms;
        }

        int bestDistance = 0;
        TileCoordinates bestTileRoom = new TileCoordinates();
        TileCoordinates bestTileComparedRoom = new TileCoordinates();
        Room bestRoom = new Room();
        Room bestComparedRoom = new Room();
        bool possibleConnectionFound = false;

        foreach(Room room in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false; //Consider other rooms before make the connection with main room

                if(room.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach(Room comparedRoom in roomListB)
            {
                if(room == comparedRoom || room.IsConnected(comparedRoom))
                {
                    continue;
                }

                for(int roomTileIndex = 0; roomTileIndex < room.edgeTiles.Count; roomTileIndex++)
                {
                    for(int comparedRoomTileIndex = 0; comparedRoomTileIndex < comparedRoom.edgeTiles.Count; comparedRoomTileIndex++)
                    {
                        TileCoordinates roomTile = room.edgeTiles[roomTileIndex];
                        TileCoordinates comparedRoomTile = comparedRoom.edgeTiles[comparedRoomTileIndex];
                        int distanceBetweenRooms = (int)(Mathf.Pow(roomTile.TileX - comparedRoomTile.TileX, 2) + Mathf.Pow(roomTile.TileY - comparedRoomTile.TileY, 2));

                        if(distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileRoom = roomTile;
                            bestTileComparedRoom = comparedRoomTile;
                            bestRoom = room;
                            bestComparedRoom = comparedRoom;
                        }
                    }
                }
            }

            if(possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoom, bestComparedRoom, bestTileRoom, bestTileComparedRoom);
            }
        }

        if(possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoom, bestComparedRoom, bestTileRoom, bestTileComparedRoom);
            ConnectClosestRooms(rooms, true);
        }

        if(!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(rooms, true);
        }
    }

    private void CreatePassage(Room room, Room otherRoom, TileCoordinates tileRoom, TileCoordinates tileOtherRoom)
    {
        Room.ConnectRooms(room, otherRoom);
        //Debug.DrawLine(CoordToWorldPoint(tileRoom), CoordToWorldPoint(tileOtherRoom), Color.green, 100);

        List<TileCoordinates> line = GetLine(tileRoom, tileOtherRoom);
        foreach(TileCoordinates coordinate in line)
        {
            DrawCircle(coordinate, 2);
        }
    }

    void DrawCircle(TileCoordinates coordinate, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if(x*x + y*y <= radius*radius)
                { 
                    int drawX = coordinate.TileX + x;
                    int drawY = coordinate.TileY + y;
                    if(drawX >= 2 && drawX < width - 2 && drawY >= 2 && drawY < height - 2)
                    {
                        map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    List<TileCoordinates> GetLine(TileCoordinates from, TileCoordinates to)
    {
        List<TileCoordinates> line = new List<TileCoordinates>();

        int x = from.TileX;
        int y = from.TileY;

        int dx = to.TileX - from.TileX;
        int dy = to.TileY - from.TileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;

        for(int i = 0; i < longest; i++)
        {
            line.Add(new TileCoordinates(x, y));

            if(inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest)
            {
                if(inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    private Vector3 CoordToWorldPoint(TileCoordinates tile)
    {
        //return new Vector3(-width / 2 + 0.5f + tile.TileX, 2, -height / 2 + 0.5f + tile.TileY);
        return new Vector3(tile.TileX + 0.5f, 2, tile.TileY + 0.5f);
    }

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

    public void GenerateRandomCostMatrix()
    {
        _costMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != 1)
                {
                    int randomCost = UnityEngine.Random.Range(1, 5);
                    _costMap[x, y] = randomCost;
                }
            }
        }
    }

    public void Dijkstra(int fromX, int fromY)
    {
        _distanceMap = new int[width, height];
        _minCost.Clear();
        _visitedTiles.Clear();
        _prevPos.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _distanceMap[x, y] = int.MaxValue;
            }
        }

        _distanceMap[fromX, fromY] = 0;
        _minCost.Add(new Vector2(fromX, fromY), _distanceMap[fromX, fromY]);

        if (!_prevPos.ContainsKey(new Vector2(fromX, fromY)))
        {
            _prevPos.Add(new Vector2(fromX, fromY), new Vector2(fromX, fromY));
        }

        while (_minCost.Count != 0)
        {
            KeyValuePair<Vector2, int> newSmallest = GetMinorCostAndRemove();
            
            Dictionary<Vector2, int> smallestNeighbours = GetSurroundingNeighboursCost((int)newSmallest.Key.x, (int)newSmallest.Key.y);

            foreach (KeyValuePair<Vector2, int> neighbour in smallestNeighbours)
            {
                if (!_visitedTiles.Contains(neighbour.Key))
                {
                    int alternativePathCost = _distanceMap[(int)newSmallest.Key.x, (int)newSmallest.Key.y] + _costMap[(int)neighbour.Key.x, (int)neighbour.Key.y];

                    if (alternativePathCost < _distanceMap[(int)neighbour.Key.x, (int)neighbour.Key.y])
                    {
                        _distanceMap[(int)neighbour.Key.x, (int)neighbour.Key.y] = alternativePathCost;
                        if (_minCost.ContainsKey(neighbour.Key))
                        {
                            _minCost.Remove(neighbour.Key);
                        }
                        _minCost.Add(neighbour.Key, _distanceMap[(int)neighbour.Key.x, (int)neighbour.Key.y]);

                        if (_prevPos.ContainsKey(neighbour.Key))
                        {
                            _prevPos.Remove(neighbour.Key);
                        }
                        _prevPos.Add(neighbour.Key, newSmallest.Key);
                    }
                }
            }

            _visitedTiles.Add(newSmallest.Key);
        }

        _finishedPath.Clear();

        GameObject minorDistance = GetObjectWithMinorDistance(_activeObjects);
        if (minorDistance != null)
        {
            CreateLessCostlyPath(new Vector2(minorDistance.transform.position.x, minorDistance.transform.position.z));
            StartCoroutine(DrawLessCostlyPathCoroutine());
        }
    }

    public KeyValuePair<Vector2, int> GetMinorCostAndRemove()
    {
        KeyValuePair<Vector2, int> minorCostPair;
        int minorCost = -1;

        foreach (Vector2 position in _minCost.Keys)
        {
            foreach (int cost in _minCost.Values)
            {
                if (minorCost < 0)
                {
                    minorCost = cost;
                    minorCostPair = new KeyValuePair<Vector2, int>(position, cost);
                }

                if (cost < minorCost)
                {
                    minorCost = cost;
                    minorCostPair = new KeyValuePair<Vector2, int>(position, cost);
                }
            }
        }

        _minCost.Remove(minorCostPair.Key);
        return minorCostPair;
    }

    public Dictionary<Vector2, int> GetSurroundingNeighboursCost(int gridX, int gridY)
    {
        Dictionary<Vector2, int> _neighboursCost = new Dictionary<Vector2, int>();

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if ((neighbourX == gridX || neighbourY == gridY) && map[neighbourX, neighbourY] != 1)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            _neighboursCost.Add(new Vector2(neighbourX, neighbourY), _costMap[neighbourX, neighbourY]);
                        }
                    }
                }
            }
        }
        
        return _neighboursCost;
    }

    public List<Vector2> CreateLessCostlyPath(Vector2 position)
    {
        Vector2 integerPosition = new Vector2((int)position.x, (int)position.y);

        if (_prevPos.ContainsKey(integerPosition))
        {
            Vector2 previousPosition = _prevPos[integerPosition];

            _finishedPath.Add(integerPosition);

            if (position != new Vector2((int)_from.transform.position.x, (int)_from.transform.position.z))
            {
                CreateLessCostlyPath(previousPosition);
            }
            /*else
            {
                Debug.Log("Cheguei no fim");
            }*/
        }

        return _finishedPath;
    }

    public IEnumerator DrawLessCostlyPathCoroutine()
    {
        foreach (Vector2 tile in _finishedPath)
        {
            Renderer _render = Map[(int)tile.x, (int)tile.y].GetComponent<Renderer>();

            _render.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", Color.black);
            _render.SetPropertyBlock(_propertyBlock);
        }

        yield return new WaitForSeconds(5f);

        foreach (Vector2 tile in _finishedPath)
        {
            Renderer _render = Map[(int)tile.x, (int)tile.y].GetComponent<Renderer>();

            _render.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", new Color(1.0f, 0.722f, 0.96f, 1.0f));
            _render.SetPropertyBlock(_propertyBlock);
        }
    }

    public GameObject GetObjectWithMinorDistance(List<GameObject> objects)
    {
        float minorDistance = -1;
        GameObject minorDistanceObject = default;

        foreach (GameObject obj in objects)
        {
            if (!obj.CompareTag("Rover") && !obj.CompareTag("Enemy") && !obj.CompareTag("Portal"))
            {
                float distance = Vector3.Distance(_from.transform.position, obj.transform.position);

                if (minorDistance < 0)
                {
                    minorDistance = distance;
                    minorDistanceObject = obj;
                }
                if (distance < minorDistance)
                {
                    minorDistance = distance;
                    minorDistanceObject = obj;
                }
            }
        }

        return minorDistanceObject;
    }

    private void ReplaceObjectAtPosition(GameObject newObject, Vector3 position)
    {
        if (_activeObjects.Contains(newObject))
        {
            newObject.transform.position = position;
            _activeObjects.Remove(newObject);
        }
    }

    public void FillMapWithObjects()
    {
        _fillObjects.AddRange(SoldiersQuantity);
        _fillObjects.AddRange(RobotsQuantity);
        randomValue.Clear();

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

                            if (_fillObjects.Count > 0 && randomValue.Contains(idTile) && map[tile.TileX, tile.TileY] == 0)
                            {
                                _activeObjects.Add(PlaceObjectAtPosition(_fillObjects[_randomObject], new Vector3(tile.TileX + 0.5f, 0.5f, tile.TileY + 0.5f)));
                            }
                        }
                    }
                }
            }
        }
    }

    public void setObjectToNotActiveList(GameObject obj)
    {
        _notActiveObjects.Add(obj);
    }

    public void RemoveFromNotActiveList(GameObject obj)
    {
        _notActiveObjects.Remove(obj);
    }

    public void setObjectToActiveList(GameObject obj)
    {
        _activeObjects.Add(obj);
    }

    public void RemoveFromActiveList(GameObject obj)
    {
        _activeObjects.Remove(obj);
    }

    public void ActivateObjects()
    {
        //Debug.Log(_notActiveObjects.Count);
        for(int i = 0; i < _notActiveObjects.Count; i++)
        {
            if(_notActiveObjects != null)
            {
                _activeObjects.Add(_notActiveObjects[i]);
                _notActiveObjects[i].gameObject.SetActive(true);
            }
        }
        
        _notActiveObjects.Clear();
        CreateMoreObjects();
        resetObjectsPosition();

        for (int i = 0; i < _activeObjects.Count; i++)
        {
            if (_activeObjects[i].gameObject.CompareTag("Enemy"))
            {
                _activeObjects[i].GetComponent<Robot>().Reset();
            }
            else if (_activeObjects[i].gameObject.CompareTag("Rover"))
            {
                _activeObjects[i].GetComponent<Rover>().Reset(MaxAmmo, MaxFuel, MaxHealth, SoldiersQuantity.Count); 
            }
        }
    }

    public void CreateMoreObjects()
    {
        int soldiersCount = (int)Mathf.Log(level, 2f);
        int soldiersToAdd = soldiersCount - SoldiersQuantity.Count;
        //Debug.Log(soldiersToAdd);

        if (soldiersToAdd > 0)
        {
            GameObject soldier = Instantiate(Soldier, transform.position, Quaternion.identity);
            _activeObjects.Add(soldier);
            SoldiersQuantity.Add(soldier);
        }

        int robotsCount = (int)Mathf.Log(level, 2f);
        int robotsToAdd = robotsCount - RobotsQuantity.Count;
        //Debug.Log(soldiersToAdd);

        if (robotsToAdd > 0)
        {
            GameObject robot = Instantiate(Robot, transform.position, Quaternion.identity);
            _activeObjects.Add(robot);
            RobotsQuantity.Add(robot);
        }
    }

    public void DecrementFuelAndAmmo()
    {
        if (level % 5 == 0)
        {
            MaxFuel -= 5;
            MaxAmmo -= 2;
        }
    }

    private void resetObjectsPosition()
    {
        randomPosition.Clear();
        _replacedObjects.AddRange(_activeObjects);

        while (_activeObjects.Count > 0)
        {
            for (int i = 0; i < _activeObjects.Count; i++)
            {
                randomPosition.Add(UnityEngine.Random.Range(0, ID));
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
                                if (_activeObjects.Count > 1)
                                {
                                    _randomObject = UnityEngine.Random.Range(0, _activeObjects.Count - 1);
                                }
                                else
                                {
                                    _randomObject = 0;
                                }

                                if (_activeObjects.Count > 0 && randomPosition.Contains(idTile))
                                {
                                    ReplaceObjectAtPosition(_activeObjects[_randomObject], new Vector3(tile.TileX + 0.5f, 0.5f, tile.TileY + 0.5f));
                                }
                            }
                        }
                    }
                }
            }
        }
        _activeObjects.AddRange(_replacedObjects);
        _replacedObjects.Clear();
    }
    
    public void CreateNewLevel()
    {
        if (level < MaxLevels)
        {
            level++;
        }

        DestroyMap();
        GenerateMap();
        Draw();
        DecrementFuelAndAmmo();
        ActivateObjects();
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

    private void OnDrawGizmos()
    {
        if(map != null && _canFill)
        {
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(map[x,y] == 0)
                    {
                        Vector3 tilePosition = new Vector3(x + 0.5f, 0, y + 0.5f);

                        if (_costMap[x,y] == 1)
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawCube(tilePosition, Vector3.one);
                        }
                        if (_costMap[x, y] == 2)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawCube(tilePosition, Vector3.one);
                        }
                        if (_costMap[x, y] == 3)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawCube(tilePosition, Vector3.one);
                        }
                        if (_costMap[x, y] == 4)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawCube(tilePosition, Vector3.one);
                        }
                    }
                }
            }
        }
    }
}

