using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGeneration : MonoBehaviour
{

    /*
        BIOME CODE:

        1 - EQUATORIAL
        2 - TROPICAL
        3 - CONTINENTAL
        4 - DESERTIC
        5 - POLAR
    */
    public static int[,]  generateBiomeMap(int[,] worldMap, int width, int height)
    {
        WorldGenerator.FileIO.WriteStringToFile("debug.txt", "je genere biomeMap", true);
        int[,] biomeMap = new int[width, height];
        biomeMap = initBiomeMap(biomeMap, width, height);
        biomeMap = addEquatorToBiomeMap(biomeMap, width, height);
        WorldGenerator.FileIO.WriteStringToFile("debug.txt", "biomeMap genere", true);
        return (biomeMap);
    }

    private static int[,]     addEquatorToBiomeMap(int[,] biomeMap ,int width, int height)
    {
        int lattitude = (int)(height * 0.2f);
        int startY = height / 2 - lattitude / 2;
        int endY = height / 2 + lattitude / 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = startY; y <= endY; y++)
                biomeMap[x, y] = 1;
            if (Random.Range(0f, 1f) >= 0.5f)
                startY += 1;
            else
                startY -= 1;
            if (Random.Range(0f, 1f) >= 0.5f)
                endY += 1;
            else
                endY -= 1;
            if (startY < height / 2 - lattitude / 2)
                startY = height / 2 - lattitude / 2;
            if (endY > height / 2 + lattitude / 2)
                endY = height / 2 + lattitude / 2;
        }
        return (biomeMap);
    }

    private static int[,]     initBiomeMap(int[,] biomeMap, int width, int height)
    {
        Debug.Log("width  = " + width + " height = " + height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                biomeMap[x, y] = 0;
            }   
        }
        Debug.Log("biomeMap[0, 0] = " + biomeMap[0, 0]);
        return (biomeMap);
    }
}
