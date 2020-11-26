using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBodiesGeneration : MonoBehaviour
{
    
    public static int[,]    generateWaterBodiesOnMap(int[,] worldMap, int[,] biomeMap, int[] continentSizes, int[,] continentShapes, int continentsNumber, int width, int height)
    {
        for (int z = 0; z < continentsNumber; z++)
        {
            int startX = continentShapes[z, 0];
            int endX = continentShapes[z, 1];
            int startY = continentShapes[z, 2];
            int endY = continentShapes[z, 3];
            if (continentSizes[z] > 50)
                addLakesTocontinent(worldMap, biomeMap, continentSizes[z], startX, endX, startY, endY, width, height);
        }
        return (worldMap);
    }

    private static void addLakesTocontinent(int[,] worldMap, int[,] biomeMap, int continentSize, int startX, int endX, int startY, int endY, int width, int height)
    {
        (int continentalTile, int tropicalTile, int equatorialTile) = calculateBiomeTilesOnContinent(worldMap, biomeMap, startX, endX, startY, endY);
        int lakeTiles = continentalTile / 20 + tropicalTile / 30 + equatorialTile / 40;
        int lakeNb = 0;
        int lakeSize = 0;

        if (lakeTiles / 30 > 1)
            lakeNb = Random.Range(1, lakeTiles / 30);
        else
            lakeNb = Random.Range(1, 3);        
        for (int i = 0; i < lakeNb; i++)
        {
            if (i < lakeNb - 1)
            {
                lakeSize = (int)(Random.Range((lakeTiles / lakeNb - (i + 1)) / 0.5f, (lakeTiles/ lakeNb - (i + 1)) * 0.5f));
                lakeTiles -= lakeSize;
            }
            else
                lakeSize = lakeTiles;
            if (lakeSize < 1)
                break ;
            int spawnsNb = 0;
            Vector2[] prevSpawns = new Vector2[lakeSize * 10];
            for (int j = 0; j < lakeSize * 10; j++)
                prevSpawns[j] = new Vector2(-1, -1);
            Vector2 spawnPoint = findLakeSpawnPoint(worldMap, startX, endX, startY, endY, width, height);
            if (spawnPoint.x == -1)
                break ;
            worldMap[(int)spawnPoint.x, (int)spawnPoint.y] = 3;
            int lakeAdded = 1;
            while (lakeAdded < lakeSize)
            {
                if (spawnsNb >= lakeSize * 10 - 1)
                    break ;
                if ((int)spawnPoint.y > 0 && worldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == 1 && isShoreTile(worldMap, (int)spawnPoint.x, (int)spawnPoint.y - 1, width, height) != 1 &&
                 Random.Range(0f, 1f) > 0.2f)
                {
                    worldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] = 3;
                    lakeAdded++;
                }
                if ((int)spawnPoint.x > 0 && worldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == 1  && isShoreTile(worldMap, (int)spawnPoint.x - 1, (int)spawnPoint.y, width, height) != 1 &&
                 Random.Range(0f, 1f) > 0.2f)
                {
                    worldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] = 3;
                    lakeAdded++;
                }
                if ((int)spawnPoint.x < width - 1 && worldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == 1 && isShoreTile(worldMap, (int)spawnPoint.x + 1, (int)spawnPoint.y, width, height) != 1
                 && Random.Range(0f, 1f) > 0.2f)
                {
                    worldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] = 3;
                    lakeAdded++;
                }
                if ((int)spawnPoint.y < height - 1 && worldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == 1 && isShoreTile(worldMap, (int)spawnPoint.x, (int)spawnPoint.y + 1, width, height) != 1 && 
                Random.Range(0f, 1f) > 0.2f)
                {
                    worldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] = 3;
                    lakeAdded++;
                }
                spawnPoint = determineNewLakeSpawnPoint(worldMap, spawnPoint, prevSpawns, spawnsNb, startX, endX, startY, endY, width, height);
                spawnsNb++;
            }
        }
    }

    private static Vector2 determineNewLakeSpawnPoint(int[,] worldMap, Vector2 spawnPoint, Vector2[] prevSpawns, int spawnsNb, int startX, int endX, int startY, int endY, int width, int height)
    {
        int randX = -2;
        int randY = -2;
        int loop = 0;

        while (randX == -2 || (worldMap[(int)spawnPoint.x + randX, (int)spawnPoint.y + randY] != 1 && worldMap[(int)spawnPoint.x + randX, (int)spawnPoint.y + randX] != 3) /*|| WorldGenerator.isNotPrevSpawn(prevSpawns, spawnPoint, randX, randY, spawnsNb) == 0 */||
        isShoreTile(worldMap, (int)spawnPoint.x + randX, (int)spawnPoint.y + randY, width, height) == 1)
        {
            if (Random.Range(0, 2) == 0)
            {
                randX = 0;
                while (randX == 0)
                {
                    if (spawnPoint.x == startX)
                        randX = Random.Range(0, 2);
                    else if (spawnPoint.x == endX)
                        randX = Random.Range(-1, 1);
                    else
                        randX = Random.Range(-1, 2);
                    randY = 0;
                }
            }
            else
            {
               randY = 0;
               while (randY == 0)
               {
                    if (spawnPoint.y == startY)
                        randY = Random.Range(0, 2);
                    else if (spawnPoint.y == endY)
                        randY = Random.Range(-1, 1);
                    else
                        randY = Random.Range(-1, 2);
                    randX = 0;
               } 
            }
            loop++;
            if (loop > 100)
            {
                randX = -2;
                randY = -2;
                while (randX == -2 || worldMap[randX, randY] != 1 || isShoreTile(worldMap, randX, randY, width, height) == 1)
                {
                    randX = Random.Range(startX, endX);
                    randY = Random.Range(startY, endY);
                    return (new Vector2(randX, randY));
                }
            }
        }
        return (new Vector2(spawnPoint.x + randX, spawnPoint.y + randY));
    }

    static private Vector2  findLakeSpawnPoint(int[,] worldMap, int startX, int endX, int startY, int endY, int width, int height)
    {
        Vector2 spawnPoint = new Vector2(-1, -1);
        int loop = 0;
        while (spawnPoint.x == -1 || worldMap[(int)spawnPoint.x, (int)spawnPoint.y] != 1 || isShoreTile(worldMap, (int)spawnPoint.x, (int)spawnPoint.y, width, height) == 1)
        {
            spawnPoint.x = Random.Range(startX, endX + 1);
            spawnPoint.y = Random.Range(startY, endY + 1);
            if (loop > 500)
                return (new Vector2(-1, -1));
        }
        return (spawnPoint);
    }

    static private int  isShoreTile(int[,] worldMap, int x, int y, int width, int height)
    {
        for (int tmpY = y - 1; tmpY < y + 2; tmpY++)
        {
            for (int tmpX = x - 1; tmpX < x + 2; tmpX++)
            {
                if (tmpY > 0 && tmpY < height && tmpX > 0 && tmpX < width && worldMap[tmpX, tmpY] == 0)
                    return (1);
            }
        }
        return (0);
    }

    static private (int, int, int)  calculateBiomeTilesOnContinent(int[,] worldMap, int[,] biomeMap, int startX, int endX, int startY, int endY)
    {
        int continentalTile = 0;
        int tropicalTile = 0;
        int equatorialTile = 0;

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                if (worldMap[x, y] == 1 && biomeMap[x, y] == 2)
                    continentalTile++;
                else if (worldMap[x, y] == 1 && biomeMap[x, y] == 3)
                    tropicalTile++;
                else if (worldMap[x, y] == 1 && biomeMap[x, y] == 4)
                    equatorialTile++;
            }
        }
        return (continentalTile, tropicalTile, equatorialTile);
    }
    
}
