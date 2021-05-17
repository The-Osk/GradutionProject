using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public LevelChunkData[] levelChunkData;
    public LevelChunkData firstChunk;

    private LevelChunkData previousChunk;

    public GameObject checkpointPrefab;
    public int CheckpointEveryNSegments = 4;

    public List<GameObject> SavedCheckpoints = new List<GameObject>();

    public GameObject FinaleTemplate;


    public Vector3 spawnOrigin;

    private Vector3 spawnPosition;
    public int trackLength = 10;

    int tracksTillCheckpoint = 0;

    Checkpoints checkpointsScript;

    LevelChunkData.Direction nextRequiredDirection = LevelChunkData.Direction.North;


    void Start()
    {
        checkpointsScript = gameObject.GetComponent<Checkpoints>();
    }

    public void GenerateTrack()
    {
        SavedCheckpoints.Clear();
        previousChunk = firstChunk;

        spawnPosition = gameObject.transform.position;

        for (int i = 0; i < trackLength; i++)
        {
            PickAndSpawnChunk();
            /*            if (tracksTillCheckpoint == CheckpointEveryNSegments && i != trackLength - 1)
                        {
                            CreateCheckpoint();
                            tracksTillCheckpoint = 0;
                        }*/
            CreateFinishGate(spawnPosition, i);
        }
    }
    public void DestroyTrack()
    {
        checkpointsScript.checkPoints.Clear();
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    LevelChunkData PickNextChunk()
    {
        List<LevelChunkData> allowedChunkList = new List<LevelChunkData>();
        LevelChunkData nextChunk = null;

        UpdateSpawnPoint();


        //spawnPosition = spawnPosition + previousChunk.levelChunks[0].transform.TransformPoint(previousChunk.levelChunks[0].transform.Find("OutputPoint").position);


        for (int i = 0; i < levelChunkData.Length; i++)
        {
           
            if (levelChunkData[i].entryDirection == nextRequiredDirection)
            {
                
                allowedChunkList.Add(levelChunkData[i]);
            }
        }
        var aloo = Random.Range(0, allowedChunkList.Count);
        nextChunk = allowedChunkList[aloo];

        return nextChunk;

    }

    void PickAndSpawnChunk()
    {

        LevelChunkData chunkToSpawn = PickNextChunk();

        Quaternion trackRotation = chunkToSpawn.trackRoation;


        
        GameObject objectFromChunk = chunkToSpawn.levelChunks[Random.Range(0, chunkToSpawn.levelChunks.Length)];

        //if(objectFr

        // var chinkInputPoint = objectFromChunk.transform.Find("InputPoint").position;

        tracksTillCheckpoint += (int)chunkToSpawn.chunkSize.y / 10;

        //Debug.Log(chunkToSpawn.name + tracksTillCheckpoint);

        if (tracksTillCheckpoint >= CheckpointEveryNSegments)
        {
            CreateCheckpoint();
            tracksTillCheckpoint = 0;
        }



        previousChunk = chunkToSpawn;
        var trackPiece = Instantiate(objectFromChunk, spawnPosition + spawnOrigin , trackRotation);

        trackPiece.transform.parent = gameObject.transform;

        

    }

    private int CreateCheckpoint()
    {
        int tillCheckpointCounter;
        GameObject checkpoint = Instantiate(checkpointPrefab, this.transform);
        checkpoint.transform.position = spawnPosition + new Vector3(0, 1.5f, 0);
        if (previousChunk.exitDirection == LevelChunkData.Direction.East || previousChunk.exitDirection == LevelChunkData.Direction.West) 
            checkpoint.transform.rotation = Quaternion.Euler(0, 90, 0);

        SavedCheckpoints.Add(checkpoint);
        //checkpoint.transform.position = checkpoint.transform.position;// + new Vector3(0, 5, 0);
        tillCheckpointCounter = 0;
        return tillCheckpointCounter;
    }

    private void CreateFinishGate(Vector3 CurrentNextPoint, int i)
    {
        if (i == trackLength - 1)
        {
            GameObject checkpoint = Instantiate(FinaleTemplate, this.transform.position, Quaternion.identity);
            UpdateSpawnPoint();
            SavedCheckpoints.Add(checkpoint);
            checkpoint.transform.position = CurrentNextPoint + new Vector3(previousChunk.chunkSize.x, 0, previousChunk.chunkSize.y);
            if (previousChunk.exitDirection == LevelChunkData.Direction.East)
            {
                checkpoint.transform.rotation = Quaternion.Euler(0, 90, 0);
            }else if(previousChunk.exitDirection == LevelChunkData.Direction.West)
            {
                checkpoint.transform.rotation = Quaternion.Euler(0, -90, 0);
            }

            for (int j = 0; j < SavedCheckpoints.Count; j++)
            {
                checkpointsScript.checkPoints.Add(SavedCheckpoints[j].GetComponent<Checkpoint>());
            }
            checkpoint.transform.parent = gameObject.transform;
            //checkpoint.transform.rotation = previousChunk.trackRoation;
            //checkpoint.transform.position = checkpoint.transform.position;// + new Vector3(0, 5, 0);
        }
    }

    private void UpdateSpawnPoint()
    {
        switch (previousChunk.exitDirection)
        {
            case LevelChunkData.Direction.North:
                nextRequiredDirection = LevelChunkData.Direction.South;
                spawnPosition = spawnPosition + new Vector3(previousChunk.chunkSize.x, 0, previousChunk.chunkSize.y);

                break;
            case LevelChunkData.Direction.West:
                nextRequiredDirection = LevelChunkData.Direction.West;
                spawnPosition = spawnPosition + new Vector3(previousChunk.chunkSize.x, 0, previousChunk.chunkSize.y);
                break;
            case LevelChunkData.Direction.South:
                nextRequiredDirection = LevelChunkData.Direction.North;
                spawnPosition = spawnPosition + new Vector3(0, 0, previousChunk.chunkSize.y);
                break;
            case LevelChunkData.Direction.East:
                nextRequiredDirection = LevelChunkData.Direction.East;
                spawnPosition = spawnPosition + new Vector3(previousChunk.chunkSize.x, 0, previousChunk.chunkSize.y);

                break;

            case LevelChunkData.Direction.EastNorth:
                nextRequiredDirection = LevelChunkData.Direction.South;
                spawnPosition = spawnPosition + new Vector3(previousChunk.chunkSize.x, 0, previousChunk.chunkSize.y);

                break;

            case LevelChunkData.Direction.WestNorth:
                nextRequiredDirection = LevelChunkData.Direction.South;
                spawnPosition = spawnPosition + new Vector3(previousChunk.chunkSize.x, 0, previousChunk.chunkSize.y);

                break;
            default:
                break;
        }
    }

    public void UpdateSpawnOrigin(Vector3 originDelta)
    {
        spawnOrigin = spawnOrigin + originDelta;
    }
}
