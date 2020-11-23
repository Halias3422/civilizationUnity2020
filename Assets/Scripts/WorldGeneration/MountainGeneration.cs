using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGeneration : MonoBehaviour
{
    public static int[,]  generateMountainsOnContinent(int[,] tmpWorldMap, int continentSize, int startX, int endX, int startY, int endY, int currContinent)
    {
        int mountainTiles = Random.Range(continentSize / 5, continentSize / 15);
        int mountainBodies;
        if (mountainTiles >= 6)
            mountainBodies = Random.Range(1, 6);
        else
            mountainBodies = 1;
        int[] mountainSizes = determineMountainSizes(mountainBodies, mountainTiles);

        for (int i = 0; i < mountainBodies; i++)
        {
            int spawnsNb = 0;
            Vector2[] prevSpawns = new Vector2[mountainSizes[i] * 10];
            for (int j = 0; j < mountainSizes[i] * 10; j++)
                prevSpawns[j] = new Vector2(-1, -1);
            Vector2 spawnPoint = findMountainSpawnPoint(tmpWorldMap, startX, endX, startY, endY);
            tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] = 11;
            int mountainAdded = 0;
            while (mountainAdded < mountainSizes[i])
            {
                if (spawnsNb >= mountainSizes[i] * 10 - 1)
                    break ;
                prevSpawns[spawnsNb++] = spawnPoint;
                if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == currContinent && Random.Range(0f, 1f) > 0.4f)
                {
                    tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] = 11;
                    mountainAdded++;
                }
                if (tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == currContinent && Random.Range(0f, 1f) > 0.4f)
                {
                    tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] = 11;
                    mountainAdded++;
                }
                if (tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == currContinent && Random.Range(0f, 1f) > 0.4f)
                {
                    tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] = 11;
                    mountainAdded++;
                }
                if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == currContinent && Random.Range(0f, 1f) > 0.4f)
                {
                    tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] = 11;
                    mountainAdded++;
                }
                spawnPoint = determineNewMountainSpawnPoint(tmpWorldMap, spawnPoint, prevSpawns, spawnsNb, startX, endX, startY, endY);
                spawnsNb++;
            }

        }
        return (tmpWorldMap);
    }

    private static Vector2 determineNewMountainSpawnPoint(int[,] tmpWorldMap, Vector2 spawnPoint, Vector2[] prevSpawns, int spawnsNb, int startX, int endX, int startY, int endY)
    {
        int randX = -2;
        int randY = -2;
        int loop = 0;

        while (randX == -2 || tmpWorldMap[(int)spawnPoint.x + randX, (int)spawnPoint.y + randY] <= 0 || WorldGenerator.isNotPrevSpawn(prevSpawns, spawnPoint, randX, randY, spawnsNb) == 0)
        {
            randX = Random.Range(-1, 2);
            randY = Random.Range(-1, 2);
            loop++;
            if (loop > 50)
            {
                randX = -2;
                randY = -2;
                while (randX == -2 || tmpWorldMap[randX, randY] <= 0)
                {
                    randX = Random.Range(startX, endX);
                    randY = Random.Range(startY, endY);
                    return (new Vector2(randX, randY));
                }
            }
        }
       return (new Vector2(spawnPoint.x + randX, spawnPoint.y + randY));
    }

    private static int[]   determineMountainSizes(int mountainBodies, int mountainTiles)
    {
        int[] mountainSizes = new int[mountainBodies];

        for (int i = 0; i < mountainBodies; i++)
        {
            if (i < mountainBodies - 1)
            {
                int possibleSize = mountainTiles / (mountainBodies - i);
                mountainSizes[i] = (int)(Random.Range(possibleSize * 0.5f, possibleSize / 0.5f));
                mountainTiles -= mountainSizes[i];
            }
            else
                mountainSizes[i] = mountainTiles;
        }
        return (mountainSizes);
    }

    private static Vector2 findMountainSpawnPoint(int[,] tmpWorldMap, int startX, int endX, int startY, int endY)
    {
        Vector2 spawnPoint = new Vector2(-1, -1);
        while (spawnPoint.x == -1 || tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] <= 0)
        {
            spawnPoint.x = Random.Range(startX, endX + 1);
            spawnPoint.y = Random.Range(startY, endY + 1);
        }
        return (spawnPoint);
    }

}
