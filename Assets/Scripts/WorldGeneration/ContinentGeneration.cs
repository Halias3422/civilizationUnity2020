using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentGeneration : MonoBehaviour
{

    public static int[]   determineContinentSizes(int totalLandSize, int continentsNumber)
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
            {
                continentSizes[i] = totalLandSize;
            }
        }
        return (continentSizes);
    }

    public static int[,] GenerateContinents(int[,] tmpWorldMap, int[] continentSizes, int continentsNumber, int width, int height)
    {
        int currContinent = 0;
        int startX, endX, startY, endY;
        startX = endX = startY = endY = -1;
        while (currContinent < continentsNumber)
        {
            int shoreDistance = Random.Range(10, 40);
            Vector2 spawnPoint = findSpawnPointContinent(tmpWorldMap, currContinent, shoreDistance, width, height);
            if (spawnPoint.x == -1)
                break ;
            int landAdded = 0;
            Vector2[] prevSpawns = new Vector2[continentSizes[currContinent] * 10];
            int spawnsNb = 0;
            for (int i = 0; i < continentSizes[currContinent] * 10; i++)
                prevSpawns[i] = new Vector2(-1, -1); 

            while (landAdded < continentSizes[currContinent])
            {
                //int randomLeft, randomRight, randomUp, randomDown;
                //randomLeft = randomRight = randomUp = randomDown = 0.4f;
                //int shape = Random.Range(0, 3);
                //if (shape == 0)
                 //   randomLeft = randomRight = 0.3f;
                int prevAdded = landAdded;
                if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] == 0)
                    landAdded++;
                tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] = currContinent + 1;
                if (startX == -1)
                {
                    startX = endX = (int)spawnPoint.x;
                    startY = endY = (int)spawnPoint.y;
                }
                if ((int)spawnPoint.x < startX)
                    startX = (int)spawnPoint.x;
                if ((int)spawnPoint.x > endX)
                    endX = (int)spawnPoint.x;
                if ((int)spawnPoint.y < startY)
                    startY = (int)spawnPoint.y;
                if ((int)spawnPoint.y > endY)
                    endY = (int)spawnPoint.y;
                if (spawnsNb == continentSizes[currContinent] * 10 - 1)
                    break ;
                prevSpawns[spawnsNb++] = spawnPoint;

                if (spawnPoint.y > 0)
                {
                    if (spawnPoint.x > 0 && tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y - 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x - 1, (int)spawnPoint.y - 1, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                    {
                        tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y - 1] = currContinent + 1;
                        if ((int)spawnPoint.x - 1 < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x - 1 > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y - 1 < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y - 1 > endY)
                            endY = (int)spawnPoint.y;
                        landAdded++;
                    }
                    if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == 0f && 
                    checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x, (int)spawnPoint.y - 1, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                    {
                        tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] = currContinent + 1;
                        if ((int)spawnPoint.x < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y - 1 < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y - 1 > endY)
                            endY = (int)spawnPoint.y;
                        landAdded++;
                    }
                    if (spawnPoint.x < width * 5 - 1 && tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y - 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x + 1, (int)spawnPoint.y - 1, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                    {
                        tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y - 1] = currContinent + 1;
                        if ((int)spawnPoint.x + 1 < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x + 1 > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y - 1 < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y - 1 > endY)
                            endY = (int)spawnPoint.y;
                        landAdded++;
                    }
                }
                if (spawnPoint.x > 0 && tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x - 1, (int)spawnPoint.y, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.6f)
                {
                    tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] = currContinent + 1;
                        if ((int)spawnPoint.x - 1 < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x - 1 > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y > endY)
                            endY = (int)spawnPoint.y;
                    landAdded++;
                }
                if (spawnPoint.x < width * 5 - 1 && tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x + 1, (int)spawnPoint.y, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                {
                    tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] = currContinent + 1;
                        if ((int)spawnPoint.x + 1 < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x + 1 > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y > endY)
                            endY = (int)spawnPoint.y;
                    landAdded++;
                }
                if (spawnPoint.y < height * 5 - 1)
                {
                    if (spawnPoint.x > 0 && tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y + 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x - 1, (int)spawnPoint.y + 1, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                    {
                        tmpWorldMap[(int)spawnPoint.x - 1, (int)spawnPoint.y + 1] = currContinent + 1;
                        if ((int)spawnPoint.x - 1 < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x - 1 > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y + 1 < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y + 1 > endY)
                            endY = (int)spawnPoint.y;
                        landAdded++;
                    }
                    if (tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == 0f && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x, (int)spawnPoint.y + 1, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                    {
                        tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] = currContinent + 1;
                        if ((int)spawnPoint.x < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y + 1 < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y + 1 > endY)
                            endY = (int)spawnPoint.y;
                        landAdded++;
                    }
                    if (spawnPoint.x < width * 5 - 1 && tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y + 1] == 0 && 
                     checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x + 1, (int)spawnPoint.y + 1, shoreDistance, width, height) == 0 && Random.Range(0f, 1f) > 0.8f)
                    {
                        tmpWorldMap[(int)spawnPoint.x + 1, (int)spawnPoint.y + 1] = currContinent + 1;
                        if ((int)spawnPoint.x + 1 < startX)
                            startX = (int)spawnPoint.x;
                        if ((int)spawnPoint.x + 1 > endX)
                            endX = (int)spawnPoint.x;
                        if ((int)spawnPoint.y + 1 < startY)
                            startY = (int)spawnPoint.y;
                        if ((int)spawnPoint.y + 1 > endY)
                            endY = (int)spawnPoint.y;
                        landAdded++;
                    }
                } 
                spawnPoint = determineNewSpawnPoint(tmpWorldMap, spawnPoint, prevSpawns, spawnsNb, currContinent + 1, startX, endX, startY, endY, shoreDistance, width, height);
                if (spawnPoint.x == -1)
                    break ;
            }
            tmpWorldMap = MountainGeneration.generateMountainsOnContinent(tmpWorldMap, landAdded, startX, endX, startY, endY, currContinent + 1);
            currContinent++;
        }
        return (tmpWorldMap);
    }

    private static Vector2 findSpawnPointContinent(int[,] tmpWorldMap, int currContinent, int shoreDistance, int width, int height)
    {
        Vector2 spawnPoint = new Vector2(-1, -1);
        int loop = 0;
        Debug.Log("width = " + width + " height = " + height);
        while (spawnPoint.x == -1 || tmpWorldMap[(int)spawnPoint.x, (int)spawnPoint.y] != 0 || checkIfOtherContinentIsClose(tmpWorldMap, currContinent + 1, (int)spawnPoint.x, (int)spawnPoint.y, shoreDistance, width, height) == 1)
        {
            loop++;
            spawnPoint = new Vector2(Random.Range(width * 2, width * 3 + 1), Random.Range(height * 2, height * 3 + 1));
            if (loop > 1000 && checkIfThereIsValidSpawnPoint(tmpWorldMap, currContinent + 1, shoreDistance, width, height) == 0)
            {
                spawnPoint.x = -1;
                return (spawnPoint);
            }
        }
        return (spawnPoint);
    }

    private static int     checkIfThereIsValidSpawnPoint(int[,] worldMap, int currContinent, int shoreDistance, int width, int height)
    {
        for (int y = height * 2; y < height * 3 - 1; y++)
        {
            for (int x = width * 2; x < width * 3 - 1; x++)
            {
                if (checkIfOtherContinentIsClose(worldMap, currContinent, x, y, shoreDistance, width, height) == 0)
                    return (1);
            }
        }
        return (0); 
    }
        
    private static int     checkIfOtherContinentIsClose(int[,] tmpWorldMap, int currContinent, int checkX, int checkY, int shoreDistance, int width, int height)
    {
        for (int y = checkY - shoreDistance; y < checkY + shoreDistance + 1; y++)
        {
            for (int x = checkX - shoreDistance; x < checkX + shoreDistance + 1; x++)
            {
                if ((y > 0 && y < height * 5 - 1 && x > 0 && x < width * 5 - 1) && tmpWorldMap[x, y] != 0 && tmpWorldMap[x, y] != currContinent)
                    return (1);
            }
        }
        return (0);  
    }

    private static Vector2 determineNewSpawnPoint(int[,] worldMap, Vector2 spawnPoint, Vector2[] prevSpawns, int spawnsNb, int currContinent, int startX, int endX, int startY, int endY, int shoreDistance, int width, int height)
    {
        int randX = -2;
        int randY = -2;
        int loop = 0;

        while (randX == -2 || (randX == 0 && randY == 0) || WorldGenerator.isNotPrevSpawn(prevSpawns, spawnPoint, randX, randY, spawnsNb) == 0 || nothingToFill(worldMap, spawnPoint, randX, randY, width, height) == 1 ||
        checkIfOtherContinentIsClose(worldMap, currContinent, (int)spawnPoint.x + randX, (int)spawnPoint.y + randY, shoreDistance, width, height) == 1)
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
            if (loop > 50 || nothingToFill(worldMap, new Vector2(0, 0), (int)spawnPoint.x, (int)spawnPoint.y, width, height) == 1)
            {
                int loop2 = 0;
                randX = randY = -2;
                while (randX == -2 || worldMap[randX, randY] != currContinent || nothingToFill(worldMap, new Vector2(0, 0), randX, randY, width, height) == 1 ||
                checkIfOtherContinentIsClose(worldMap, currContinent, randX, randY, shoreDistance, width, height) == 1)
                {
                    randX = Random.Range(startX, endX + 1);
                    randY = Random.Range(startY, endY + 1);
                    loop2++;
                    if (loop2 > (endX - startX) * (endY - startY))
                    {
                        randX = -1;
                        break ;
                    }
                }
                return (spawnPoint = new Vector2(randX, randY));
            }
        }
        return (spawnPoint = new Vector2(spawnPoint.x + randX, spawnPoint.y + randY));
    }

    private static int nothingToFill(int[,] worldMap, Vector2 spawnPoint, int randX, int randY, int width, int height)
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
}