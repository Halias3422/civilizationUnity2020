using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGeneration : MonoBehaviour
{

    /*
        BIOME CODE:

        1 - POLAR
        2 - CONTINENTAL
        3 - TROPICAL
        4 - EQUATORIAL
        5 - DESERTIC
    */
    public static int[,]  generateBiomeMap(int[,] worldMap, int width, int height)
    {
        int[,] biomeMap = new int[width, height];
        biomeMap = initBiomeMap(biomeMap, width, height);
        addContinentalToBiomeMap(biomeMap, width, height);
        int tropicalTiles = addTropicalToBiomeMap(biomeMap, width, height);
        addEquatorToBiomeMap(biomeMap, width, height);
        addDesertsToBiomeMap(biomeMap, width, height, tropicalTiles);
        return (biomeMap);
    }

    private static int   addContinentalToBiomeMap(int[,] biomeMap, int width, int height)
    {
        int minStartY = (int)(height * 0.1f);
        int maxStartY = (int)(height * 0.3f);
        int minEndY = (int)(height * 0.7f);
        int maxEndY = (int)(height * 0.9f);
        int startY = Random.Range(minStartY, maxStartY);
        int endY = Random.Range(minEndY, maxEndY);
        int biomeTiles = addNewBiomeToMap(biomeMap, width, height, startY, endY, minStartY, maxStartY, minEndY, maxEndY, 2); 
        return (biomeTiles);
    }

    private static int   addTropicalToBiomeMap(int[,] biomeMap, int width, int height)
    {
        int minStartY = (int)(height * 0.3f);
        int maxStartY = (int)(height * 0.4f);
        int minEndY = (int)(height * 0.6f);
        int maxEndY = (int)(height * 0.7f);
        int startY = Random.Range(minStartY, maxStartY);
        int endY = Random.Range(minEndY, maxEndY);
        int biomeTiles = addNewBiomeToMap(biomeMap, width, height, startY, endY, minStartY, maxStartY, minEndY, maxEndY, 3); 
        return (biomeTiles);
    }

    private static int     addEquatorToBiomeMap(int[,] biomeMap ,int width, int height)
    {
        int minStartY = (int)(height * 0.4f);
        int maxStartY = (int)(height * 0.5f);
        int minEndY = (int)(height * 0.5f);
        int maxEndY = (int)(height * 0.6f);
        int startY = Random.Range(minStartY, maxStartY);
        int endY = Random.Range(minEndY, maxEndY);
        int biomeTiles = addNewBiomeToMap(biomeMap, width, height, startY, endY, minStartY, maxStartY, minEndY, maxEndY, 4);
        return (biomeTiles);
    }

    private static int   addDesertsToBiomeMap(int[,] biomeMap, int width, int height, int tropicalTiles)
    {
        int biomeTiles = 0;
        int desertsNb = Random.Range(1, 5);
        int desertTilesNb = tropicalTiles / 10;
        int desertSize = 0;
        for (int i = 0; i < desertsNb; i++)
        {
            int spawnsNb = 0;
            if (i < desertsNb - 1)
                desertSize = (int)(Random.Range(desertTilesNb / 1.5f, desertTilesNb / desertsNb));
            else
                desertSize = desertTilesNb;
            desertTilesNb -= desertSize;
            Vector2[] prevSpawns = new Vector2[desertSize * 10];
            for (int h = 0; h < desertSize * 10; h++)
                prevSpawns[h] = new Vector2(-1, -1);
            Vector2 spawnPoint = findDesertSpawnPoint(biomeMap, width, height);
            biomeMap[(int)spawnPoint.x, (int)spawnPoint.y] = 5;
            int desertAdded = 1;
            while (desertAdded < desertSize)
            {
                if (spawnsNb >= desertSize * 10 - 1)
                    break ;
                prevSpawns[spawnsNb++] = spawnPoint;
                if ((int)spawnPoint.y > 0 && (biomeMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == 3 || biomeMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] == 4) &&
                Random.Range(0f, 1f) >= 0.3f)
                {
                    biomeMap[(int)spawnPoint.x, (int)spawnPoint.y - 1] = 5;
                    desertAdded++;
                }
                if ((int)spawnPoint.x > 0 && (biomeMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == 3 || biomeMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] == 4) &&
                Random.Range(0f, 1f) >= 0.3f)
                {
                    biomeMap[(int)spawnPoint.x - 1, (int)spawnPoint.y] = 5;
                    desertAdded++;
                }
                if ((int)spawnPoint.x < width - 1 && (biomeMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == 3 || biomeMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] == 4) &&
                Random.Range(0f, 1f) >= 0.3f)
                {
                    biomeMap[(int)spawnPoint.x + 1, (int)spawnPoint.y] = 5;
                    desertAdded++;
                }
                if ((int)spawnPoint.y < height - 1 && (biomeMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == 3 || biomeMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] == 4) &&
                Random.Range(0f, 1f) >= 0.3f)
                {
                    biomeMap[(int)spawnPoint.x, (int)spawnPoint.y + 1] = 5;
                    desertAdded++;
                }
                spawnPoint = determineNewDesertSpawnPoint(biomeMap, spawnPoint, prevSpawns, spawnsNb, width, height);
                spawnsNb++;
            }
        }
        return (biomeTiles);
    }

    private static Vector2  determineNewDesertSpawnPoint(int[,] biomeMap, Vector2 spawnPoint, Vector2[] prevSpawns, int spawnsNb, int width, int height)
    {            
        int randX = -2;
        int randY = -2;
        int minY = (int)(height * 0.3f);
        int maxY = (int)(height * 0.7f);
        int loop = 0;
        while (randX == -2 || (biomeMap[(int)spawnPoint.x + randX, (int)spawnPoint.y + randY] != 3 && biomeMap[(int)spawnPoint.x + randX, (int)spawnPoint.y + randY] != 4) ||
        WorldGenerator.isNotPrevSpawn(prevSpawns, spawnPoint, randX, randY, spawnsNb) == 0)
        {
            if (spawnPoint.x == 0)
                randX = Random.Range(0, 2);
            else if (spawnPoint.x == width - 1)
                randX = Random.Range(-1, 1);
            else
                randX = Random.Range(-1, 2);
            if (spawnPoint.y == minY)
                randY = Random.Range(0, 2);
            else if (spawnPoint.y == maxY)
                randY = Random.Range(-1, 1);
            else
                randY = Random.Range(-1, 2);
            loop++;
            if (loop > 50)
            {
                randX = randY = -2;
                while (randX == -2 || (biomeMap[randX, randY] != 3 && biomeMap[randX, randY] != 4))
                {
                    randX = Random.Range(0, width - 1);
                    randY = Random.Range(minY, maxY);
                    return (new Vector2(randX, randY));
                } 
            }
        }
        return (new Vector2(spawnPoint.x + randX, spawnPoint.y + randY));
    }

    private static Vector2  findDesertSpawnPoint(int[,] biomeMap, int width, int height)
    {
        int minY = (int)(height * 0.3f);
        int maxY = (int)(height * 0.7f);
        int randX = -2;
        int randY = -2;
        while (randX == -2 || (biomeMap[randX, randY] != 3 && biomeMap[randX, randY] != 4))
        {
            randX = Random.Range(0, width - 1);
            randY = Random.Range(minY, maxY);
        } 
        return (new Vector2(randX, randY));
    }

    private static int  addNewBiomeToMap(int[,] biomeMap, int width, int height, int startY, int endY, int minStartY, int maxStartY, int minEndY, int maxEndY, int biomeNb)
    {
        int biomeTiles = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                biomeMap[x, y] = biomeNb;
                biomeTiles++;
            }
            if (Random.Range(0f, 1f) >= 0.5f)
                startY += 1;
            else
                startY -= 1;
            if (Random.Range(0f, 1f) >= 0.5f)
                endY += 1;
            else      
                endY -= 1;
            if (startY < minStartY)
                startY = minStartY;
            else if (startY > maxStartY)
                startY = maxStartY;
            if (endY < minEndY)
                endY = minEndY;
            else if (endY > maxEndY)
                endY = maxEndY;
        }
        return (biomeTiles);

    }


    private static int[,]     initBiomeMap(int[,] biomeMap, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                biomeMap[x, y] = 1;
            }   
        }
        return (biomeMap);
    }
}
