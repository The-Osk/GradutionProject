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
    [SerializeField] Checkpoints checkpointsScript;
    [SerializeField] Checkpoints checkpointsScript1;

    public CarAgent carAgent;
    public Checkpoint nextCheckPointToReach;

    private int CurrentCheckpointIndex;
    private List<Checkpoint> Checkpoints;

    private Checkpoint lastCheckpoint;


    public event Action<Checkpoint> reachedCheckpoint;

    void Start()
    {
        Checkpoints = checkpointsScript.checkPoints;

        ResetCheckpoints();
    }

    public void ResetCheckpoints()
    {
        randomTrackNumber = UnityEngine.Random.Range(0, 2);
        if (randomTrackNumber == 0)
        {
            Checkpoints = checkpointsScript.checkPoints;
        }
        else
        {
            Checkpoints = checkpointsScript1.checkPoints;
        }
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
