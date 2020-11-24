using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Drawing;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] int        width = 0;
    [SerializeField] int        height = 0;
    [SerializeField] int        continentsNumber = 0;
    [SerializeField] TileBase   grassTile = null;
    [SerializeField] TileBase   grass2Tile = null;
    [SerializeField] TileBase   grass3Tile = null;
    [SerializeField] TileBase   waterTile = null;
    [SerializeField] TileBase   equatorial = null;
    [SerializeField] TileBase   tropical = null;
    [SerializeField] TileBase   desertic = null;
    [SerializeField] TileBase   continental = null;
    [SerializeField] TileBase   polar = null;
    [SerializeField] Tilemap    mapTileMap = null;
    [SerializeField] Tilemap    biomeTileMap = null;

    void Start()
    {
        File.Delete("debug.txt");
        int[,] tmpWorldMap = new int[width * 5, height * 5];
        int totalLandSize = (int)(width * height * 0.3f);
        int[] continentSizes = ContinentGeneration.determineContinentSizes(totalLandSize, continentsNumber);
        int[,] continentShapes = new int[continentsNumber, 4];
        tmpWorldMap = ContinentGeneration.GenerateContinents(tmpWorldMap, continentSizes, continentsNumber, width, height, continentShapes);
        int[,] worldMap = shrinkWorldMapToSize(tmpWorldMap, continentShapes);
        iterativeHomeMadeFloodFill(worldMap, new Vector2(0, 0));
        removeInsideWaters(worldMap, continentSizes, continentShapes);
        worldMap = MountainGeneration.generateMountainsOnMap(worldMap, continentSizes, continentShapes, continentsNumber, width, height);
        //int[,] biomeMap = BiomeGeneration.generateBiomeMap(worldMap, width, height);
        //worldMap = WaterBodiesGeneration.generateWaterBodiesOnMap(worldMap, continentSizes, continentShapes, continentsNumber, width, height);
        if (height > width)
        {
            Debug.Log("ROTATION");
        //    biomeMap = rotateBiomeMap(biomeMap);
            worldMap = rotateMap(worldMap);
        }
        FileIO.WriteStringToFile("debug.txt", "pret a imprimer", true);
        printMapToScreen(worldMap/*, biomeMap*/);
    }

    int[,]    rotateBiomeMap(int[,] biomeMap)
    {
        int[,] newBiomeMap = new int[height, width];
        int newY = 0;

        for (int x = width - 1; x > - 1; x--)
        {
            int newX = 0;
            for (int y = 0; y < height - 1; y++)
                newBiomeMap[newX++, newY] = biomeMap[x, y];
            newY++;
        }
        return (newBiomeMap);
    }

    int[,]    rotateMap(int[,] worldMap)
    {
        int[,] newWorldMap = new int[height, width];
        int newY = 0;

        for (int x = width - 1; x > -1; x--)
        {
            int newX = 0;
            for (int y = 0; y < height - 1; y++)
            {
                newWorldMap[newX++, newY] = worldMap[x, y];
            }
            newY++;
        }
        int tmp = width;
        width = height;
        height = tmp;
        return (newWorldMap);
    }

    void    removeInsideWaters(int[,] worldMap, int[] continentSizes, int[,] continentShapes)
    {
        for (int tmpY = 0; tmpY < height; tmpY++)
        {
            for (int tmpX = 0; tmpX < width; tmpX++)
            {
                if (worldMap[tmpX, tmpY] > 1 && worldMap[tmpX, tmpY] < 11)
                    worldMap[tmpX, tmpY] = 1;
                else if (worldMap[tmpX, tmpY] == 0)
                {
                    worldMap[tmpX, tmpY] = 1;
                    addLandCountToContinentSize(continentSizes, continentShapes, tmpX, tmpY);
                }
                else if (worldMap[tmpX, tmpY] == -1)
                    worldMap[tmpX, tmpY] = 0;
            }
        }
    }

    void    addLandCountToContinentSize(int[] continentSizes, int[,] continentShapes, int x, int y)
    {
        for (int i = 0; i < continentsNumber; i++)
        {
            if (x >= continentShapes[i, 0] && x <= continentShapes[i, 1] && y >= continentShapes[i, 2] && y <= continentShapes[i, 3])
            {
                continentSizes[i]++;
                return ;
            }
        }
    }

    void    iterativeHomeMadeFloodFill(int[,] worldMap, Vector2 tile)
    {
        worldMap[(int)tile.x, (int)tile.y] = -1;
        List<Vector2> list = new List<Vector2>();
        if (worldMap[(int)tile.x + 1, (int)tile.y] == 0)
        {
            worldMap[(int)tile.x + 1, (int)tile.y] = -2;
            list.Add(new Vector2(tile.x + 1, tile.y));
        }
        if (worldMap[(int)tile.x, (int)tile.y + 1] == 0)
        {
            worldMap[(int)tile.x, (int)tile.y + 1] = -2;
            list.Add(new Vector2(tile.x, tile.y + 1));
        }
        for (int i = 0; i < list.Count; i++)
        {
            for (Vector2 tmp = list[i]; tmp.x < width; tmp.x++)
            {
                if (worldMap[(int)tmp.x, (int)tmp.y] == -1 || worldMap[(int)tmp.x, (int)tmp.y] > 0)
                    break ;
                if (worldMap[(int)tmp.x, (int)tmp.y] == -2 || worldMap[(int)tmp.x, (int)tmp.y] == 0)
                {
                    worldMap[(int)tmp.x, (int)tmp.y] = -1;
                    list = checkTwoSurroundingTiles(worldMap, (int)tmp.x, (int)tmp.y, list);
                }

            }
            for (Vector2 tmp = list[i]; tmp.x > -1; tmp.x--)
            {
                if (worldMap[(int)tmp.x, (int)tmp.y] == -1 || worldMap[(int)tmp.x, (int)tmp.y] > 0)
                    break ;
                if (worldMap[(int)tmp.x, (int)tmp.y] == -2 || worldMap[(int)tmp.x, (int)tmp.y] == 0)
                {
                    worldMap[(int)tmp.x, (int)tmp.y] = -1;
                    list = checkTwoSurroundingTiles(worldMap, (int)tmp.x, (int)tmp.y, list);
                }
            }
        }
    }

    List<Vector2>    checkTwoSurroundingTiles(int[,] worldMap, int x, int y, List<Vector2> list)
    {
        if (y > 0 && worldMap[x, y - 1] == 0)
        {
            worldMap[x, y - 1] = -2;
            list.Add(new Vector2(x, y - 1));
        }
        if (y < height - 1 && worldMap[x, y + 1] == 0)
        {
            worldMap[x, y + 1] = -2;
            list.Add(new Vector2(x, y + 1));
        }
        if (x > 0 && worldMap[x -1, y] == 0)
        {
            worldMap[x - 1, y] = -2;
            list.Add(new Vector2(x - 1, y));
        }
        return (list);
    }

    void    floodFill(int[,] worldMap, int x, int y)
    {
        if (worldMap[x, y] == 0 && worldMap[x, y] != -1)
        {
            worldMap[x, y] = -1;
            if (x < width - 1)
                floodFill(worldMap, x + 1, y);
            if (y < height - 1)
                floodFill(worldMap, x, y + 1);
            if (x > 0)
                floodFill(worldMap, x - 1, y);
            if (y > 0)
                floodFill(worldMap, x, y - 1);
        }
    }

    int[,] shrinkWorldMapToSize(int[,] tmpWorldMap, int[,] continentShapes)
    {
        int startX = findStartX(tmpWorldMap);
        int startY = findStartY(tmpWorldMap);
        int endY = findEndY(tmpWorldMap);
        int endX = findEndX(tmpWorldMap);

        width = endX - startX + 9;
        height = endY - startY + 9;
        int[,] worldMap = new int[width, height];
        int newY = 4;
        for (int y = startY; y <= endY; y++)
        {
            int newX = 4;
            for (int x = startX; x <= endX; x++)
            {
                worldMap[newX++, newY] = tmpWorldMap[x, y];
            }
            newY++;
        }
        continentShapes = changeContinentShapesLocations(continentShapes, startX, startY);
        return (worldMap);
    }

    int[,]  changeContinentShapesLocations(int[,] continentShapes, int startX, int startY)
    {
        for (int i = 0; i < continentsNumber; i++)
        {
            continentShapes[i, 0] -= startX;
            continentShapes[i, 1] -= startX;
            continentShapes[i, 2] -= startY;
            continentShapes[i, 3] -= startY;
        }
        return (continentShapes);
    }

    int     findStartX(int[,] tmpWorldMap)
    {
        for (int x = 0; x < width * 5 - 1; x++)
        {
            for (int y = 0; y < height * 5 - 1; y++)
            {
                if (tmpWorldMap[x, y] != 0)
                    return (x);
            }
        }
        return (-1);
    }

    int     findStartY(int[,] tmpWorldMap)
    {
        for (int y = 0; y < height * 5 - 1; y++)
        {
            for (int x = 0; x < width * 5 - 1; x++)
            {
                if (tmpWorldMap[x, y] != 0)
                    return (y);
            }
        }
        return (-1);
    }

    int findEndX(int[,] tmpWorldMap)
    {
        for (int x = width * 5 - 1; x > -1; x--)
        {
            for (int y = height * 5 - 1; y > -1; y--)
            {
                if (tmpWorldMap[x, y] != 0)
                    return (x);
            }
        }
        return (-1);
    }

    int     findEndY(int[,] tmpWorldMap)
    {
        for (int y = height * 5 - 1; y > - 1; y--)
        {
            for (int x = width * 5 - 1; x > -1; x--)
            {
                if (tmpWorldMap[x, y] != 0)
                    return (y);
            }
        }
        return (-1);
    }


    public static int isNotPrevSpawn(Vector2[] prevSpawns, Vector2 spawnPoint, int randX, int randY, int spawnsNb)
    {
        for (int i = 0; i < spawnsNb; i++)
        {
            if (prevSpawns[i].x == spawnPoint.x + randX && prevSpawns[i].y == spawnPoint.y + randY)
                return (0);
        }
        return (1);
    }

    void printMapToScreen(int[,] worldMap/*, int[,] biomeMap*/)
    {
        FileIO.WriteStringToFile("debug.txt", "width = " + width + " height = " + height, true);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (worldMap[x, y] == 0f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), waterTile);
                if (worldMap[x, y] > 0f && worldMap[x, y] < 11f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grassTile);
                if (worldMap[x, y] < 0f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grass3Tile);
                if (worldMap[x, y] == 11f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grass2Tile);
              /*  FileIO.WriteStringToFile("debug.txt", "x = " + x + " y = " + y, true);
                if (biomeMap[x, y] == 1)
                    biomeTileMap.SetTile(new Vector3Int(x, height - y, 0), equatorial);
                if (biomeMap[x, y] == 2)
                    biomeTileMap.SetTile(new Vector3Int(x, height - y, 0), tropical);
                if (biomeMap[x, y] == 3)
                    biomeTileMap.SetTile(new Vector3Int(x, height - y, 0), continental);
                if (biomeMap[x, y] == 4)
                    biomeTileMap.SetTile(new Vector3Int(x, height - y, 0), desertic);
                if (biomeMap[x, y] == 5)
                    biomeTileMap.SetTile(new Vector3Int(x, height - y, 0), polar);
                */
       
            }
        }
    }

    void initContinentMap(int[,]continentMap, int sizeX, int sizeY)
    {
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
                continentMap[x, y] = 0;
        }
    }

    void initWorldMap(int[,] worldMap)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                worldMap[x, y] = 0;
        }
    }

public class FileIO {

/// <summary>
/// Write a string to a file
/// </summary>
/// <param name=”fileName”>File to write to.</param>
/// <param name=”content”>String to write.</param>
/// <param name=”append”>If set to <c>true</c>, append. If set to <c>false<c>, overwrite file.</param>

public static void WriteStringToFile(string fileName, string content, bool append)
{

StreamWriter sw = new StreamWriter(fileName, append);

sw.WriteLine(content);

sw.Close();

}

}
    }

