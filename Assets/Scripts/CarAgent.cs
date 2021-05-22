using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarAgent : Agent
{
    [SerializeField] WallHitTracker wallTracker;
    [SerializeField] CarAnalytics carAnalytics;

    [SerializeField] int timeSteps = 0;
    [SerializeField] int epCount = 0;
    [SerializeField] float maxSpeed = 0f;
    [SerializeField] double avgSpeed = 0f;
    [SerializeField] public ulong endedDueToTime = 0;

    Vector3 oldPos;
    [SerializeField] public float distanceCovered = 0;


    string collidedWith = "";
    private double speedSum = 0f;


    public TrackGenerator trackGenerator;
    public CheckpointManager _checkpointManager;
    private CarController _carController;
    Rigidbody rigidbody;
    //public ulong wallHits = 0;

    public float rewardFromWall = 0;
    public float rewardFromTime = 0;
    public float rewardFromSpeed = 0;
    public float rewardFromBrakes = 0;

    [SerializeField] public EpisodeData epData = new EpisodeData();

    //called once at the start
    public override void Initialize()
    {
         oldPos = transform.position;
        _carController = GetComponent<CarController>();
        rigidbody = GetComponent<Rigidbody>();
        wallTracker = FindObjectOfType<WallHitTracker>();
        carAnalytics = FindObjectOfType<CarAnalytics>();
    }


    

    //Called each time it has timed-out or has reached the goal
    public override void OnEpisodeBegin()
    {
        
        epCount = carAnalytics.epCount;
        carAnalytics.epCount++;

            RecordEpData();
            if (!carAnalytics)
            {
                Debug.LogWarning("Analytics object or script is missing");
            }
            else
            {
                carAnalytics.AddEpDataTolist(epData);
            }
            ClearEpData();
        Debug.Log(epCount);

        if (trackGenerator)
        {
            trackGenerator.DestroyTrack();
            trackGenerator.GenerateTrack();
        }
        

        //Basic colliosion rate
/*        Debug.Log(epCount);
        if (epCount > 0)
            Debug.Log((wallTracker.WallHitCount / epCount) * 100 + "%");*/

        if (!_checkpointManager.testing)
            _checkpointManager.ResetCheckpoints();

        oldPos = transform.position;
        oldPos.y = 0;
        distanceCovered = 0;
        _carController.Respawn();



    }

    private void ClearEpData()
    {
        speedSum = 0;
        epData = new EpisodeData();
        timeSteps = 0;
        _checkpointManager.epTime = 0;

        epData.wallHit = 0;
        epData.collidedWith = "";

        distanceCovered = 0;

        avgSpeed = 0;
        maxSpeed = 0;

        epData.totalReward = 0;

        _checkpointManager.rewardFromCheckpoints = 0;
         _checkpointManager.rewardFromFinaCheckpoint = 0;
        _checkpointManager.rewardFromTimeLimit = 0;

        rewardFromWall = 0;

        rewardFromSpeed = 0;
        rewardFromTime = 0;
        rewardFromBrakes = 0;
    }

    private void RecordEpData()
    {
        epData.epNo = carAnalytics ? carAnalytics.epCount : epCount;
        epData.timesteps = timeSteps;
        epData.time = _checkpointManager.epTime;

        epData.wallHit = rewardFromWall == -13 ? 1 : 0;
        epData.collidedWith = rewardFromWall == -13 ? collidedWith : "";

        epData.distanceCovered = distanceCovered;

        epData.avgSpeed = avgSpeed;
        epData.maxSpeed = maxSpeed;

        epData.totalReward = _checkpointManager.rewardFromCheckpoints + _checkpointManager.rewardFromFinaCheckpoint +
            _checkpointManager.rewardFromTimeLimit + rewardFromWall + rewardFromSpeed + rewardFromTime + rewardFromBrakes;

        epData.rewardFromCheckpoints = _checkpointManager.rewardFromCheckpoints;
        epData.rewardFromFinish = _checkpointManager.rewardFromFinaCheckpoint;
        epData.rewardFromTimeRunOut = _checkpointManager.rewardFromTimeLimit;

        epData.rewardFromWall = rewardFromWall;

        epData.rewardFromSpeed = rewardFromSpeed;
        epData.rewardFromTime = rewardFromTime;
        epData.rewardFromBrakes = rewardFromBrakes;
    }



    //Collecting extra Information that isn't picked up by the RaycastSensors

    public override void CollectObservations(VectorSensor sensor)
    {
        //Vector3 diffFromCheckpoint = _checkpointManager.nextCheckPointToReach.transform.position - transform.position;
        //base.CollectObservations(sensor);


        var directionToCheckpoint = (this.transform.position - _checkpointManager.nextCheckPointToReach.transform.position).normalized;
        sensor.AddObservation(directionToCheckpoint.x);
        sensor.AddObservation(directionToCheckpoint.z);
        sensor.AddObservation(rigidbody.velocity.magnitude);
       sensor.AddObservation(_carController.steeringAngle);
        //   sensor.AddObservation(diffFromCheckpoint / 20);
        rewardFromTime += (-0.001f);
       AddReward(-0.001f);

        if (rigidbody.velocity.magnitude > maxSpeed)
        {
            maxSpeed = rigidbody.velocity.magnitude;
        }
        speedSum += rigidbody.velocity.magnitude;
        avgSpeed = speedSum / timeSteps;

        //Debug.Log(GetCumulativeReward());
    }

    //Processing the actions received
    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.ContinuousActions;

        if(rigidbody.velocity.magnitude < 4)
        {
            rewardFromSpeed += (-0.01f);
            AddReward(-0.01f);
        }
        //_carController.HandleMotor(Mathf.Clamp(input[1], 0, 1));
        _carController.HandleMotor(input[1] + 1, actions.DiscreteActions[0]);
        _carController.HandleSteering(input[0]);

        //Brakes
        bool brake = (int)actions.DiscreteActions[0] > 0;
        if (brake)
        {
            rewardFromBrakes += (-0.1f);
            AddReward(-0.1f);
        }
        timeSteps++;

        if (timeSteps < 20)
            distanceCovered = 0;
        Vector3 distanceVector = transform.position - oldPos;
        distanceVector.y = 0;
        float distanceThisFrame = distanceVector.magnitude;
        distanceCovered += distanceThisFrame;
        oldPos = transform.position;

        _carController.UpdateWheels();

    }

    //For manual testing with human input, the actionsOut defined here will be sent to OnActionRecieved
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Mathf.Clamp(Input.GetAxis("Vertical"),0,1)*2 - 1;


        //discrete brakes 
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;


    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if (wallTracker)
            {
                wallTracker.WallHitCount++;
                //Debug.Log(wallTracker.WallHitCount);
            }
            rewardFromWall = -13;
            //last100EpCollision[epCount % 100] = true;
            collidedWith = other.transform.name;
            AddReward(-13f);
            EndEpisode();
        }
    }

}
