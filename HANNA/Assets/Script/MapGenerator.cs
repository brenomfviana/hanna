using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This class is responsible by the cavern generation.
 * This code was created based on: https://unity3d.com/pt/learn/tutorials/projects/procedural-cave-generation-tutorial/cellular-automata
 */
public class MapGenerator : MonoBehaviour {

    // Map dimension
    public int width;
    public int height;

    // Map
    public Tilemap grid;
    public Tile floor;
    public Tile wall;

    // Generation parameters
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int randomFillPercent;
    int[,] map;
    
    void Start() {
        GenerateMap();
    }

    void Update() {
        // Check if the map was created
        if (map != null) {
            // Print map
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++)  {
                    if (map[x, y] == 1) {
                        grid.SetTile(new Vector3Int(x, y, 0), wall);
                    } else {
                        grid.SetTile(new Vector3Int(x, y, 0), floor);
                    }
                }
            }
        }
    }

    void GenerateMap() {
        // Init map
        map = new int[width, height];
        RandomFillMap();
        // Build cave
        for (int i = 0; i < 5; i++) {
            BuildCave();
        }
    }

    void RandomFillMap() {
        // Check if the random seed is on
        if (useRandomSeed) {
            // Get a seed
            seed = System.DateTime.Now + " ";
        }
        // Init random system
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        // Fill map
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1;
                } else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void BuildCave() {
        // Cellular Automata approach
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Get amount of neighbours
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                // Check if is a wall
                if (neighbourWallTiles > 3) {
                    map[x, y] = 1;
                } else if (neighbourWallTiles < 3) {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int w, int h) {
        int wallCount = 0;
        if (h - 1 >= 0 && map[w, h - 1] == 1) {
            wallCount++;
        }
        if (h + 1 < height && map[w, h + 1] == 1) {
            wallCount++;
        }
        if (w - 1 >= 0 && map[w - 1, h] == 1) {
            wallCount++;
        }
        if (w + 1 < width && map[w + 1, h] == 1) {
            wallCount++;
        }
        // Check if y is even
        if (w % 2 == 0) {
            if ((w - 1 >= 0 && h - 1 >= 0) && map[w - 1, h - 1] == 1) {
                wallCount++;
            }
            if (w + 1 < width && (h - 1 >= 0) && map[w + 1, h - 1] == 1) {
                wallCount++;
            }
        } else {
            if ((w - 1 >= 0 && h + 1 < height) && map[w - 1, h + 1] == 1) {
                wallCount++;
            }
            if ((w + 1 < width && h + 1 < height) && map[w + 1, h + 1] == 1) {
                wallCount++;
            }
        }
        return wallCount;
    }
}
