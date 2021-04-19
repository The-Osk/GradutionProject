using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public float MaxTimeToReachNextCheckpoint = 30f;
    public float TimeLeft = 30f;
    public int randomTrackNumber = 0;


    // Consider making it a list for multi tracks
    [SerializeField] List<Checkpoints> checkpointsScript;
    

    public CarAgent carAgent;
    public Checkpoint nextCheckPointToReach;

    private int CurrentCheckpointIndex;
    private List<Checkpoint> Checkpoints;

    private Checkpoint lastCheckpoint;


    public event Action<Checkpoint> reachedCheckpoint;

    void Start()
    {
        Checkpoints = checkpointsScript[0].checkPoints;

        ResetCheckpoints();
    }


    public void ResetCheckpoints()
    {
        randomTrackNumber = UnityEngine.Random.Range(0, checkpointsScript.Count);
        Checkpoints = checkpointsScript[randomTrackNumber].checkPoints;
        
        foreach (Checkpoint c in Checkpoints)
        {
            c.gameObject.SetActive(false);
        }

        CurrentCheckpointIndex = 0;
        TimeLeft = MaxTimeToReachNextCheckpoint;

        SetNextCheckpoint();
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;

        if (TimeLeft < 0f)
        {
            carAgent.AddReward(-100f);
            carAgent.EndEpisode();
        }
    }

    public void CheckPointReached(Checkpoint checkpoint)
    {
        if (nextCheckPointToReach != checkpoint) return;


        lastCheckpoint = Checkpoints[CurrentCheckpointIndex];
        reachedCheckpoint?.Invoke(checkpoint);
        CurrentCheckpointIndex++;
        

        if (CurrentCheckpointIndex >= Checkpoints.Count)
        {
            carAgent.AddReward(50f);
            Debug.Log("<color=green>Final checkpoint Reached</color>"  + carAgent.GetCumulativeReward(), this);
            carAgent.EndEpisode();
        }
        else
        {
            carAgent.AddReward((100f/ Checkpoints.Count));
            checkpoint.gameObject.SetActive(false);
            SetNextCheckpoint();
        }
    }

    private void SetNextCheckpoint()
    {
        if (Checkpoints.Count > 0)
        {
            TimeLeft = MaxTimeToReachNextCheckpoint;
            Checkpoints[CurrentCheckpointIndex].gameObject.SetActive(true);
            nextCheckPointToReach = Checkpoints[CurrentCheckpointIndex];

        }
    }
}
