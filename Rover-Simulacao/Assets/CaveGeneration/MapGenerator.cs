using UnityEngine;
using System.Collections;
using System;

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

    [Range(0,100)]
	public int randomFillPercent;

    bool Norte, Sul, Leste, Oeste;
	int[,] map;
    GameObject[,] Map;

    void Start() {
		GenerateMap();
        Draw();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Clique");
            DestroyMap();
            GenerateMap();
            Draw();
        }if (Input.GetMouseButtonDown(1)) {
            ChecarColisao(x1, x2);
        }
    }

	void GenerateMap() {
        width2 = width;
        height2 = height;
		map = new int[width,height];
        Map = new GameObject[width, height];
        RandomFillMap();

		for (int i = 0; i < 5; i ++) {
			SmoothMap();
		}

		//MeshGenerator meshGen = GetComponent<MeshGenerator>();
		//meshGen.GenerateMesh(map, 1);
	}

    void Draw()
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
void ChecarColisao(int x, int y)
    {
        Norte = false;
        Sul = false;
        Leste = false;
        Oeste = false;
        if(map[x, y - 1] == 1)
        Norte = true;

        if (map[x, y + 1] == 1)
            Sul = true;

        if (map[x + 1, y] == 1)
           Leste = true;

        if (map[x - 1, y] == 1)
            Oeste = true;

        Debug.Log(Norte);
        Debug.Log(Sul);
        Debug.Log(Leste);
        Debug.Log(Oeste);
    }
    void DestroyMap()
    {
        for (int i = 0; i < width2; i++)
            for (int j = 0; j < height2; j++)
            {
                Destroy(Map[i, j]);
            }
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
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
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


	void OnDrawGizmos() {
		/*
		if (map != null) {
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					Gizmos.color = (map[x,y] == 1)?Color.black:Color.white;
					Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
					Gizmos.DrawCube(pos,Vector3.one);
				}
			}
		}
		*/
	}

}
