using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelChunkData")]
public class LevelChunkData : ScriptableObject
{
    public enum Direction
    {
        North, East, South, West, WestNorth, EastNorth
    }

    public Vector2 chunkSize = new Vector2(10f, 10f);

    public GameObject[] levelChunks;
    public Direction entryDirection;
    public Direction exitDirection;
    public Quaternion trackRoation;
    public Vector3 inputPoint;
    public Vector3 outputPoint;
    public bool RequireLargeCurve;


}
