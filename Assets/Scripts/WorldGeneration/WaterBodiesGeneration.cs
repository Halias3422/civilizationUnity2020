using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBodiesGeneration : MonoBehaviour
{
    public static int[,]    generateWaterBodiesOnMap(int[,] worldMap, int[] continentSizes, int[,] continentShapes, int continentsNumber, int width, int height)
    {
        for (int z = 0; z < continentsNumber; z++)
        {
            int startX = continentShapes[z, 0];
            int endX = continentShapes[z, 1];
            int startY = continentShapes[z, 2];
            int endY = continentShapes[z, 3];
        }
        return (worldMap);
    }
}
