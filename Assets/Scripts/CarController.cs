using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    Rigidbody rigidbody;
    CheckpointManager checkpointManager;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private IEnumerator coroutine;

    private float horizontalInput;
    private float verticalInput;

    public float steeringAngle;

    private float currentBrakeForce;
    //private bool isBraking;

    [SerializeField] List<SpawnPointManager> _spawnPointManager;

    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteering;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] GameObject BrakeLights;
    [SerializeField] bool manual = false;

    bool isBraking = false;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        checkpointManager = GetComponent<CheckpointManager>();
    }


    enum DriveModes
    {
        FrontDrive,
        BackDrive,
        QuadDrive
    };

    [SerializeField] DriveModes currentDriveMode = DriveModes.FrontDrive;

    void FixedUpdate()
    {
        int x;
        if (isBraking)
            x = 1;
        else x = 0;
        if (manual)
        {
            GetInput();
            HandleMotor(verticalInput, x);
            HandleSteering(horizontalInput);
            UpdateWheels();
        }
    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    public void HandleMotor(float verticalInput, int isBrake)
    {
        verticalInputbla = verticalInput;
        //Debug.Log(verticalInput);
        if (verticalInput >= 0)
        {
            //StopBrakes();

            if (currentDriveMode == DriveModes.FrontDrive)
            {
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
                frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            }

            else if (currentDriveMode == DriveModes.BackDrive)
            {
                rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
                rearRightWheelCollider.motorTorque = verticalInput * motorForce;
            }

            else if (currentDriveMode == DriveModes.QuadDrive)
            {
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
                frontRightWheelCollider.motorTorque = verticalInput * motorForce;
                rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
                rearRightWheelCollider.motorTorque = verticalInput * motorForce;
            }
        }
        ApplyBraking(isBrake);

/*        else if (verticalInput < 0)
        {
            ApplyBraking();
        }*/

        /*currentBrakeForce = isBraking ? brakeForce : 0;
        if (isBraking)
        {
            Debug.Log("Braking");
        }
        ApplyBraking();*/
    }


    public void StopBrakes()
    {
        
        frontLeftWheelCollider.brakeTorque = 0;
        frontRightWheelCollider.brakeTorque = 0;
        rearLeftWheelCollider.brakeTorque = 0;
        rearRightWheelCollider.brakeTorque = 0;
    }

    public void ApplyBraking(int isBrake)
    {
        if (isBrake == 1)
        {
            BrakeLights.SetActive(true);
            frontLeftWheelCollider.brakeTorque = brakeForce;
            frontRightWheelCollider.brakeTorque = brakeForce;
            rearLeftWheelCollider.brakeTorque = brakeForce;
            rearRightWheelCollider.brakeTorque = brakeForce;
        }
        else
        {
            BrakeLights.SetActive(false);
            frontLeftWheelCollider.brakeTorque = 0;
            frontRightWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
        }

    }

    public void HandleSteering(float horizontalInput)
    {
        //Debug.Log(horizontalInput);
        steeringAngle = maxSteering * horizontalInput;
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
        
    }

    public void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void ResetSpeed()
    {
        frontLeftWheelCollider.steerAngle = 0;
        frontRightWheelCollider.steerAngle = 0;

        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
        rearLeftWheelCollider.motorTorque = 0;
        rearRightWheelCollider.motorTorque = 0;



/*        frontLeftWheelCollider.brakeTorque = Mathf.Infinity;
        frontRightWheelCollider.brakeTorque = Mathf.Infinity;
        rearLeftWheelCollider.brakeTorque = Mathf.Infinity;
        rearRightWheelCollider.brakeTorque = Mathf.Infinity;*/

        

    }


    public void Respawn()
    {
        //coroutine = BrakeTimer();
        StartCoroutine(BrakeTimer());


        Vector3 pos = _spawnPointManager[checkpointManager.randomTrackNumber].SelectRandomSpawnpoint();


        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigidbody.MovePosition(pos);

        //rigidbody.
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.rotation = Quaternion.Euler(0,0,0);
        ResetSpeed();


        //transform.position = pos;
    }

    IEnumerator BrakeTimer()
    {
        frontLeftWheelCollider.brakeTorque = Mathf.Infinity;
        frontRightWheelCollider.brakeTorque = Mathf.Infinity;
        rearLeftWheelCollider.brakeTorque = Mathf.Infinity;
        rearRightWheelCollider.brakeTorque = Mathf.Infinity;


        yield return new WaitForSeconds(0.05f);
        //DebugRespawn();
        frontLeftWheelCollider.brakeTorque = 0;
        frontRightWheelCollider.brakeTorque = 0;
        rearLeftWheelCollider.brakeTorque = 0;
        rearRightWheelCollider.brakeTorque = 0;
    }


    [SerializeField] float verticalInputbla;
    [SerializeField] float frontLeftWheelColliderbrakeTorque;
    [SerializeField] float frontRighttWheelColliderbrakeTorque;
    [SerializeField] float backLeftWheelColliderbrakeTorque;
    [SerializeField] float backRighttWheelColliderbrakeTorque;

    [SerializeField] bool frontLeftWheelColliderisGrounded;
    [SerializeField] bool frontRightWheelColliderisGrounded;
    [SerializeField] bool rearLeftWheelColliderisGrounded;
    [SerializeField] bool rearRightWheelColliderisGrounded;

    [SerializeField] float frontLeftWheelColliderrpm;
    [SerializeField] float frontRightWheelColliderrpm;
    [SerializeField] float rearLeftWheelColliderrpm;
    [SerializeField] float rearRightWheelColliderrpm;

    [SerializeField] float frontLeftWheelCollidermotorTorque;
    [SerializeField] float frontRightWheelCollidermotorTorque;
    [SerializeField] float rearLeftWheelCollidermotorTorque;
    [SerializeField] float rearRightWheelCollidermotorTorque;

    [SerializeField] float frontLeftWheelCollidersteerAngle;
    [SerializeField] float frontrightWheelCollidersteerAngle;

    void DebugRespawn()
    {
        Debug.Log("#########################################\n#########################################");

        Debug.Log(frontLeftWheelCollider.brakeTorque);

        Debug.Log("frontLeftWheelCollider Grounded: " + frontLeftWheelCollider.isGrounded);
        Debug.Log("frontRightWheelCollider Grounded: " + frontRightWheelCollider.isGrounded);
        Debug.Log("rearLeftWheelCollider Grounded: " + rearLeftWheelCollider.isGrounded);
        Debug.Log("rearRightWheelCollider Grounded: " + rearRightWheelCollider.isGrounded);

        Debug.Log("transofrm rotation:\t" + transform.rotation);

        Debug.Log("Rigid body speed:\t" + rigidbody.velocity);
        Debug.Log("Rigid body rotation:\t" + rigidbody.rotation);

        Debug.Log("front left Wheel rpm\t" + frontLeftWheelCollider.rpm);
        Debug.Log("front Right Wheel rpm\t" + frontRightWheelCollider.rpm);
        Debug.Log("Rear left Wheel rpm\t" + rearLeftWheelCollider.rpm);
        Debug.Log("Rear right Wheel rpm\t" + rearRightWheelCollider.rpm);

        Debug.Log("Front Left Wheel motorTorque\t" + frontLeftWheelCollider.motorTorque);
        Debug.Log("Front right Wheel motorTorque\t" + frontRightWheelCollider.motorTorque);
        Debug.Log("Rear Left Wheel motorTorque\t" + rearLeftWheelCollider.motorTorque);
        Debug.Log("Rear right Wheel motorTorque\t" + rearRightWheelCollider.motorTorque);

        Debug.Log("front left Wheel Angle\t" + frontLeftWheelCollider.steerAngle);
        Debug.Log("front Right Wheel Angle\t" + frontRightWheelCollider.steerAngle);
    }
}
