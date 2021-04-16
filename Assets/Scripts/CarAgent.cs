using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarAgent : Agent
{
    public CheckpointManager _checkpointManager;
    private CarController _carController;
    Rigidbody rigidbody;
    public ulong wallHits = 0;

    public double rewardFromTime = 0;
    public double rewardFromSpeed = 0;

    //called once at the start
    public override void Initialize()
    {
        _carController = GetComponent<CarController>();
        rigidbody = GetComponent<Rigidbody>();
    }


    

    //Called each time it has timed-out or has reached the goal
    public override void OnEpisodeBegin()
    {
        _checkpointManager.ResetCheckpoints();
        _carController.Respawn();
        
    }

    

    //Collecting extra Information that isn't picked up by the RaycastSensors
    
    public override void CollectObservations(VectorSensor sensor)
    {
       //Vector3 diffFromCheckpoint = _checkpointManager.nextCheckPointToReach.transform.position - transform.position;
        //base.CollectObservations(sensor);

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
        _carController.HandleMotor(Mathf.Clamp(input[1], 0, 1));
        _carController.HandleSteering(input[0]);
        _carController.UpdateWheels();

    }

    //For manual testing with human input, the actionsOut defined here will be sent to OnActionRecieved
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Mathf.Clamp(Input.GetAxis("Vertical"),0,1);
       // action[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;

    }

}
