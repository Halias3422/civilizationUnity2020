using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] int        width = 0;
    [SerializeField] int        height = 0;
    [SerializeField] int        continentsNumber = 0;
    [SerializeField] TileBase   grassTile = null;
    [SerializeField] TileBase   grass2Tile = null;
    [SerializeField] TileBase   grass3Tile = null;
    [SerializeField] TileBase   waterTile = null;
    [SerializeField] Tilemap    mapTileMap = null;

    // Start is called before the first frame update
    void Start()
    {
        File.Delete("debug.txt");
        int[,] tmpWorldMap = new int[width * 5, height * 5];
        int totalLandSize = (int)(width * height * 0.6f);
        int[] continentSizes = determineContinentSizes(totalLandSize);
        tmpWorldMap = GenerateContinents(tmpWorldMap, continentSizes);
        int[,] worldMap = shrinkWorldMapToSize(tmpWorldMap);
        floodFill(worldMap, 0, 0);
        removeInsideWaters(worldMap);
        if (height > width)
            worldMap = rotateMap(worldMap);
        //worldMap = dispatchContinentOnWorldMap(continentMaps, continentMapSizes);
        printMapToScreen(worldMap);
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
        Debug.Log("MAP ROTATED");
        return (newWorldMap);
    }

    void    removeInsideWaters(int[,] worldMap)
    {
        for (int tmpY = 0; tmpY < height; tmpY++)
        {
            for (int tmpX = 0; tmpX < width; tmpX++)
            {
                if (worldMap[tmpX, tmpY] > 1)
                    worldMap[tmpX, tmpY] = 1;
                else if (worldMap[tmpX, tmpY] == 0)
                    worldMap[tmpX, tmpY] = 1;
                else if (worldMap[tmpX, tmpY] == -1)
                    worldMap[tmpX, tmpY] = 0;
            }
        }
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

    int[,] shrinkWorldMapToSize(int[,] tmpWorldMap)
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
        return (worldMap);
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

    int[]   determineContinentSizes(int totalLandSize)
    {
        int[] continentSizes = new int[continentsNumber];
        for (int i = 0; i < continentsNumber; i++)
        {
            if (i < continentsNumber - 1)
            {
                int possibleSize = totalLandSize / (continentsNumber - i);
                continentSizes[i] = (int)(Random.Range(possibleSize * 0.5f, possibleSize / 0.5f));
                totalLandSize -= continentSizes[i];
            }
            else
                continentSizes[i] = totalLandSize;
        }
        return (continentSizes);
    }

    int[,] GenerateContinents(int[,] tmpWorldMap, int[] continentSizes)
    {
        int currContinent = 0;
        while (currContinent < continentsNumber)
        {
            FileIO.WriteStringToFile("debug.txt", "currContinent = " + currContinent + " NOUVEAU CONTINENT", true);
            Vector2 spawnPoint = findSpawnPointContinent(tmpWorldMap, currContinent);
            if (spawnPoint.x == -1)
                break ;
            int landAdded = 0;
            Vector2[] prevSpawns = new Vector2[continentSizes[currContinent] * 10];
            int spawnsNb = 0;
            for (int i = 0; i < continentSizes[currContinent] * 10; i++)
                prevSpawns[i] = new Vector2(-1, -1); 

            while (landAdded < continentSizes[currContinent])
            {
                int prevAdded = landAdded;
                if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] == 0)
                    landAdded++;
                tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] = currContinent + 1;
                FileIO.WriteStringToFile("debug.txt", "initialSpawn = [" + spawnPoint.x + ", " + spawnPoint.y + "]", true);
                if (spawnsNb == continentSizes[currContinent] * 10 - 1)
                {
                    //Debug.Log("JAI DU BREAKER PREVSPAWNS FULL");
                    break ;
                }
                prevSpawns[spawnsNb++] = spawnPoint;

                if (spawnPoint.y > 0)
                {
                    if (spawnPoint.x > 0 && tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y - 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x - 1, (int)spawnPoint.y - 1) == 0 && Random.Range(0f, 1f) > 0.5f)
                    {
                        tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y - 1] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "HAUT GAUCHE coord [" + (spawnPoint.x - 1) + ", " + (spawnPoint.y - 1) + "]", true);
                        landAdded++;
                    }
                    if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == 0f && 
                    checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x, (int)spawnPoint.y - 1) == 0 && Random.Range(0f, 1f) > 0.5f)
                    {
                        tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "HAUT CENTRE coord [" + spawnPoint.x + ", " + (spawnPoint.y - 1) + "]", true);
                        landAdded++;
                    }
                    if (spawnPoint.x < width * 5 - 1 && tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y - 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x + 1, (int)spawnPoint.y - 1) == 0 && Random.Range(0f, 1f) > 0.5f)
                    {
                        tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y - 1] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "HAUT DROITE coord [" + (spawnPoint.x + 1) + ", " + (spawnPoint.y - 1) + "]", true);
                        landAdded++;
                    }
                }
                if (spawnPoint.x > 0 && tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x - 1, (int)spawnPoint.y) == 0 && Random.Range(0f, 1f) > 0.5f)
                {
                    tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "CENTRE GAUCHE coord [" + (spawnPoint.x - 1) + ", " + spawnPoint.y + "]", true);
                    landAdded++;
                }
                if (spawnPoint.x < width * 5 - 1 && tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x + 1, (int)spawnPoint.y) == 0 && Random.Range(0f, 1f) > 0.5f)
                {
                    tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "CENTRE DROIT coord [" + (spawnPoint.x + 1) + ", " + spawnPoint.y + "]", true);
                    landAdded++;
                }
                if (spawnPoint.y < height * 5 - 1)
                {
                    if (spawnPoint.x > 0 && tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y + 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x - 1, (int)spawnPoint.y + 1) == 0 && Random.Range(0f, 1f) > 0.5f)
                    {
                        tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y + 1] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "BAS GAUCHE coord [" + (spawnPoint.x - 1) + ", " + (spawnPoint.y + 1) + "]", true);
                        landAdded++;
                    }
                    if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == 0f && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x, (int)spawnPoint.y + 1) == 0 && Random.Range(0f, 1f) > 0.5f)
                    {
                        tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "BAS CENTRE coord [" + spawnPoint.x + ", " + (spawnPoint.y + 1) + "]", true);
                        landAdded++;
                    }
                    if (spawnPoint.x < width * 5 - 1 && tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y + 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x + 1, (int)spawnPoint.y + 1) == 0 && Random.Range(0f, 1f) > 0.5f)
                    {
                        tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y + 1] = currContinent + 1;
                        FileIO.WriteStringToFile("debug.txt", "BAS DROITE coord [" + (spawnPoint.x + 1) + ", " + (spawnPoint.y + 1) + "]", true);
                        landAdded++;
                    }
                } 
                spawnPoint = determineNewSpawnPoint(tmpWorldMap, spawnPoint, prevSpawns, spawnsNb, currContinent + 1);
                if (spawnPoint.x == -1)
                    break ;
            }
            currContinent++;
        }
        return (tmpWorldMap);
    }

    Vector2 findSpawnPointContinent(int[,] tmpWorldMap, int currContinent)
    {
        Vector2 spawnPoint = new Vector2(-1, -1);
        int loop = 0;
        while (spawnPoint.x == -1 || tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] != 0 || checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x, (int)spawnPoint.y) == 1)
        {
            loop++;
            spawnPoint = new Vector2(Random.Range(width * 2, width * 3 + 1), Random.Range(height * 2, height * 3 + 1));
            if (loop > 1000 && checkIfThereIsValidSpawnPoint(tmpWorldMap, currContinent + 1) == 0)
            {
                spawnPoint.x = -1;
                return (spawnPoint);
            }
        }
        return (spawnPoint);
    }

    int     checkIfThereIsValidSpawnPoint(int[,] worldMap, int currContinent)
    {
        for (int y = height * 2; y < height * 3 - 1; y++)
        {
            for (int x = width * 2; x < width * 3 - 1; x++)
            {
                if (checkIfOtherContinentIsClose(worldMap, currContinent, x, y) == 0)
                    return (1);
            }
        }
        return (0); 
    }
        
    int     checkIfOtherContinentIsClose(int[,] tmpWorldMap, int currContinent, int checkX, int checkY)
    {
        for (int y = checkY - 4; y < checkY + 5; y++)
        {
            for (int x = checkX - 4; x < checkX + 5; x++)
            {
                if ((y > 0 && y < height * 5 - 1 && x > 0 && x < width * 5 - 1) && tmpWorldMap[x, y] != 0 && tmpWorldMap[x, y] != currContinent)
                {
                    FileIO.WriteStringToFile("debug.txt", "CHECKIFOTHER RETURN 1", true);
                    return (1);
                }
            }
        }
        FileIO.WriteStringToFile("debug.txt", "CHECKIFOTHER RETURN 0", true);
        return (0);  
    }

    Vector2 determineNewSpawnPoint(int[,] worldMap, Vector2 spawnPoint, Vector2[] prevSpawns, int spawnsNb, int currContinent)
    {
        int randX = -2;
        int randY = -2;
        int loop = 0;

        while (randX == -2 || (randX == 0 && randY == 0) || isNotPrevSpawn(prevSpawns, spawnPoint, randX, randY, spawnsNb) == 0 || nothingToFill(worldMap, spawnPoint, randX, randY) == 1 ||
        checkIfOtherContinentIsClose(worldMap, currContinent, (int)spawnPoint.x + randX, (int)spawnPoint.y + randY) == 1)
        {
            if (spawnPoint.x == 0)
                randX = Random.Range(0, 2);
            else if (spawnPoint.x ==  width * 5 - 1)
                randX = Random.Range(-1, 1);
            else
                randX = Random.Range(-1, 2);
            if (spawnPoint.y == 0)
                randY = Random.Range(0, 2);
            else if (spawnPoint.y == height * 5 - 1)
                randY = Random.Range(-1, 1);
            else
                randY = Random.Range(-1, 2);
            if (spawnPoint.x > width * 5 - 1)
                spawnPoint.x = width * 5 - 1;
            if (spawnPoint.y > height * 5 - 1)
                spawnPoint.y = height * 5 - 1;
            if (spawnPoint.x < 0)
                spawnPoint.x = 0;
            if (spawnPoint.y < 0)
                spawnPoint.y = 0;
            loop++;
            if (loop > 20)
            {
                int loop2 = 0;
                randX = randY = -2;
                while (randX == -2 || worldMap[randX, randY] != currContinent || nothingToFill(worldMap, new Vector2(0, 0), randX, randY) == 1 ||
                checkIfOtherContinentIsClose(worldMap, currContinent, randX, randY) == 1)
                {
                    randX = Random.Range(width * 2, width * 3);
                    randY = Random.Range(height * 2, height * 3);
                    loop2++;
                    if (loop2 > 1000)
                    {
                        FileIO.WriteStringToFile("debug.txt", "boucle inf dans la loop" , true);
                        Debug.Log("boucle inf dans la loop");
                        randX = -1;
                        break ;
                    }
                }
                FileIO.WriteStringToFile("debug.txt", "NOUVEAU SPAWN DANS LOOP = [" + randX + ", " + randY + "]", true);
                return (spawnPoint = new Vector2(randX, randY));
            }
        }
                FileIO.WriteStringToFile("debug.txt", "NOUVEAU SPAWN NORMAL = [" + (spawnPoint.x + randX) + ", " + (spawnPoint.y + randY) + "]", true);
        return (spawnPoint = new Vector2(spawnPoint.x + randX, spawnPoint.y + randY));
    }
    
    int nothingToFill(int[,] worldMap, Vector2 spawnPoint, int randX, int randY)
    {
        Vector2 newSpawn = new Vector2(spawnPoint.x + randX, spawnPoint.y + randY);

        if (newSpawn.y > 0)
        {
            if (newSpawn.x > 0 && worldMap[(int)newSpawn.x - 1, (int)newSpawn.y - 1] == 0)
                return (0);
            if (worldMap[(int)newSpawn.x, (int)newSpawn.y - 1] == 0)
                return (0);
            if (newSpawn.x < width * 5 - 1 && worldMap[(int)newSpawn.x + 1, (int)newSpawn.y - 1] == 0)
                return (0);
        }
        if (newSpawn.x > 0 && worldMap[(int)newSpawn.x - 1, (int)newSpawn.y] == 0)
            return (0);
        if (newSpawn.x < width * 5 - 1 && worldMap[(int)newSpawn.x + 1, (int)newSpawn.y] == 0)
            return (0);
        if (newSpawn.y < height * 5 - 1)
        {
            if (newSpawn.x > 0 && worldMap[(int)newSpawn.x - 1, (int)newSpawn.y + 1] == 0)
                return (0);
            if (worldMap[(int)newSpawn.x, (int)newSpawn.y + 1] == 0)
                return (0);
            if (newSpawn.x < width * 5 - 1 && worldMap[(int)newSpawn.x + 1, (int)newSpawn.y + 1] == 0)
                return (0);
        }
        return (1);
    }

    int isNotPrevSpawn(Vector2[] prevSpawns, Vector2 spawnPoint, int randX, int randY, int spawnsNb)
    {
        for (int i = 0; i < spawnsNb; i++)
        {
            if (prevSpawns[i].x == spawnPoint.x + randX && prevSpawns[i].y == spawnPoint.y + randY)
                return (0);
        }
        return (1);
    }

    void printMapToScreen(int[,] worldMap)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (worldMap[x, y] == 0f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), waterTile);
                if (worldMap[x, y] > 0f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grassTile);
                if (worldMap[x, y] < 0f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grass2Tile);
                    /*
                if (worldMap[x, y] == 1f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grassTile);
                if (worldMap[x, y] == 2f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grass2Tile);
                if (worldMap[x, y] == 3f)
                    mapTileMap.SetTile(new Vector3Int(x, height - y, 0), grass3Tile);
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

