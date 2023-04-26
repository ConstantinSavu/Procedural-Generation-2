using System;
using System.Collections.Generic;
using UnityEngine;

public static class DataProcessing {

    public static List<Vector2Int> directions = new List<Vector2Int>{
        new Vector2Int(0, 1), //N
        new Vector2Int(1, 1), //NE
        new Vector2Int(1, 0), //E
        new Vector2Int(-1, 1), //E
        new Vector2Int(-1, 0), //S
        new Vector2Int(-1, -1), //SW
        new Vector2Int(0, -1), //W
        new Vector2Int(1, -1) //NW
    };

    private static bool CheckNeighbours(float[,] noiseData, Vector2Int localPos, Func<float, bool> succesCondition){

        Vector2Int length =  new Vector2Int(noiseData.GetLength(0), noiseData.GetLength(1));

        foreach(Vector2Int dir in directions){
            Vector2Int newPos = localPos + dir;

            //check boundires

            if(newPos.x < 0 || newPos.x >= length.x){
                continue;
            }

            if(newPos.y < 0 || newPos.y >= length.y){
                continue;
            }

            if(!succesCondition(noiseData[newPos.x, newPos.y])){
                return false;
            }

        }

        return true;
    }
    public static List<Vector3Int> FindLocalMaxima2D(float[,] noiseData, Vector3Int chunkWorldPosition) {
        Vector2Int length =  new Vector2Int(noiseData.GetLength(0), noiseData.GetLength(1));
        List<Vector3Int> maximas = new List<Vector3Int>();

        for(int x = 0; x < length.x; x++){
            for(int y = 0; y < length.y; y++){
                
                float currentNoise = noiseData[x, y];

                if(CheckNeighbours(noiseData, new Vector2Int(x, y), (neighborNoise) => neighborNoise < currentNoise)){

                    maximas.Add(new Vector3Int(x, 0, y));

                }

            }
        }


        return maximas;

    }
}