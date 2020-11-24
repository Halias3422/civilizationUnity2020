using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGeneration : MonoBehaviour
{

    public static int[,]    generateMountainsOnMap(int[,] worldMap, int[] continentSizes, int[,] continentShapes, int continentNumber, int width, int height)
    {
        for (int z = 0; z < continentNumber; z++)
        {
            int startX = continentShapes[z, 0];
            int endX = continentShapes[z, 1];
            int startY = continentShapes[z, 2];
            int endY = continentShapes[z, 3];
            int mountainTiles = Random.Range(continentSizes[z] / 8, continentSizes[z] / 15);
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
                Vector2 spawnPoint = findMountainSpawnPoint(worldMap, startX, endX, startY, endY);
                worldMap[(int)spawnPoint.x, (int)spawnPoint.y] = 11;
                int mountainAdded = 1;
                while (mountainAdded < mountainSizes[i])
                {
                    if (spawnsNb >= mountainSizes[i] * 10 - 1)
                        break ;
                    prevSpawns[spawnsNb++] = spawnPoint;
                    if ((int)spawnPoint.y > 0 && worldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == 1 && Random.Range(0f, 1f) > 0.4f)
                    {
                        worldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] = 11;
                        mountainAdded++;
                    }
                    if ((int)spawnPoint.x > 0 && worldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == 1 && Random.Range(0f, 1f) > 0.4f)
                    {
                        worldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] = 11;
                        mountainAdded++;
                    }
                    if ((int)spawnPoint.x < width - 1 && worldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == 1 && Random.Range(0f, 1f) > 0.4f)
                    {
                        worldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] = 11;
                        mountainAdded++;
                    }
                    if ((int)spawnPoint.y < height - 1 && worldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == 1 && Random.Range(0f, 1f) > 0.4f)
                    {
                        worldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] = 11;
                        mountainAdded++;
                    }
                    spawnPoint = determineNewMountainSpawnPoint(worldMap, spawnPoint, prevSpawns, spawnsNb, startX, endX, startY, endY, width, height);
                    spawnsNb++;
                }
            }
        }
        return (worldMap);
    }

    private static Vector2 determineNewMountainSpawnPoint(int[,] tmpWorldMap, Vector2 spawnPoint, Vector2[] prevSpawns, int spawnsNb, int startX, int endX, int startY, int endY, int width, int height)
    {
        int randX = -2;
        int randY = -2;
        int loop = 0;

        while (randX == -2 || tmpWorldMap[(int)spawnPoint.x + randX, (int)spawnPoint.y + randY] <= 0 || WorldGenerator.isNotPrevSpawn(prevSpawns, spawnPoint, randX, randY, spawnsNb) == 0)
        {
            if (spawnPoint.x == 0)
                randX = Random.Range(0, 2);
            else if (spawnPoint.x == width - 1)
                randX = Random.Range(-1, 1);
            else
                randX = Random.Range(-1, 2);
            if (spawnPoint.y == 0)
                randY = Random.Range(0, 2);
            else if (spawnPoint.y == height - 1)
                randY = Random.Range(-1, 1);
            else
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
