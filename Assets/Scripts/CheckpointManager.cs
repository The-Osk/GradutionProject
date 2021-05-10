using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public float MaxTimeToReachNextCheckpoint = 30f;
    public float TimeLeft = 30f;

    public int randomTrackNumber = 0;

    [SerializeField] List<Checkpoints> checkpointsScript;
    [SerializeField] TrackGenerator generator;
    

    public CarAgent carAgent;

    public Checkpoint nextCheckPointToReach;
    private int CurrentCheckpointIndex;
    private List<Checkpoint> Checkpoints;
    private Checkpoint lastCheckpoint;

    public bool testing = false;
    public bool procedural = false;

    [SerializeField] public float epTime = 0f;

    [SerializeField] public float rewardFromTimeLimit = 0;
    [SerializeField] public float rewardFromFinaCheckpoint = 0;
    [SerializeField] public float rewardFromCheckpoints = 0;


    public event Action<Checkpoint> reachedCheckpoint;

    void Start()
    {
        Checkpoints = checkpointsScript[0].checkPoints;
        ResetCheckpoints();
    }


    public void ResetCheckpoints()
    {
        rewardFromTimeLimit = 0;
        rewardFromFinaCheckpoint = 0;
        rewardFromCheckpoints = 0;
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
        epTime += Time.deltaTime;

        if (TimeLeft < 0f)
        {

            rewardFromTimeLimit = -100f;
            carAgent.endedDueToTime++;
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
            rewardFromFinaCheckpoint = 50f;
            carAgent.AddReward(50f);
            Debug.Log("<color=green>Final checkpoint Reached</color>"  + carAgent.GetCumulativeReward(), this);
            carAgent.EndEpisode();
        }
        else
        {
            rewardFromCheckpoints += (100f / Checkpoints.Count);
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
