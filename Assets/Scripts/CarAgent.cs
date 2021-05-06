using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarAgent : Agent
{
    [SerializeField] ulong timeSteps = 0;
    [SerializeField] int epCount = -1;
    [SerializeField] WallHitTracker wallTracker;
    public ulong endedDueToTime = 0;

    public TrackGenerator trackGenerator;
    public CheckpointManager _checkpointManager;
    private CarController _carController;
    Rigidbody rigidbody;
    //public ulong wallHits = 0;

    public double rewardFromTime = 0;
    public double rewardFromSpeed = 0;

    List<bool> last100EpCollision = new List<bool>();

    //called once at the start
    public override void Initialize()
    {
        _carController = GetComponent<CarController>();
        rigidbody = GetComponent<Rigidbody>();
        wallTracker = FindObjectOfType<WallHitTracker>();
    }


    

    //Called each time it has timed-out or has reached the goal
    public override void OnEpisodeBegin()
    {
        if (trackGenerator)
        {
            trackGenerator.DestroyTrack();
            trackGenerator.GenerateTrack();
        }
        epCount++;
        /*if (epCount < 100)
            last100EpCollision.Add(false);
        else
        {
            last100EpCollision[epCount % 100] = false;
        }
        if(wallTracker.WallHitCount != 0)
            Debug.Log("Collision Rate: " + wallTracker.WallHitCount / epCount * 100 + "%");

        int wallHitsInLast100Ep = 0;
        for (int i = 0; i < last100EpCollision.Count; i++)
        {
            if(last100EpCollision[i])
                wallHitsInLast100Ep++;
        }

        Debug.Log("Collision Rate for last 100 Ep: " + wallHitsInLast100Ep / last100EpCollision.Count * 100 + "%");*/

        timeSteps = 0;
        if(!_checkpointManager.testing)
            _checkpointManager.ResetCheckpoints();

        _carController.Respawn();
        
    }

    

    //Collecting extra Information that isn't picked up by the RaycastSensors
    
    public override void CollectObservations(VectorSensor sensor)
    {
        //Vector3 diffFromCheckpoint = _checkpointManager.nextCheckPointToReach.transform.position - transform.position;
        //base.CollectObservations(sensor);

        if (!_checkpointManager.testing)
        {
            var directionToCheckpoint = (this.transform.position - _checkpointManager.nextCheckPointToReach.transform.position).normalized;
            sensor.AddObservation(directionToCheckpoint.x);
            sensor.AddObservation(directionToCheckpoint.z);
        }
       sensor.AddObservation(rigidbody.velocity.magnitude);
       sensor.AddObservation(_carController.steeringAngle);
        //   sensor.AddObservation(diffFromCheckpoint / 20);
       AddReward(-0.001f);

        //Debug.Log(GetCumulativeReward());
    }

    //Processing the actions received
    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.ContinuousActions;

        if(rigidbody.velocity.magnitude < 4)
        {
            AddReward(-0.01f);
        }
        //_carController.HandleMotor(Mathf.Clamp(input[1], 0, 1));
        _carController.HandleMotor(input[1] + 1);
        _carController.HandleSteering(input[0]);

        //Brakes
        bool brake = (int)actions.DiscreteActions[0] > 0;
        if (brake)
        {
            AddReward(-0.5f);
            _carController.ApplyBraking();
        }
        timeSteps++;

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
                Debug.Log(wallTracker.WallHitCount);
            }

            //last100EpCollision[epCount % 100] = true;
            AddReward(-13f);
            EndEpisode();
        }
    }

}
