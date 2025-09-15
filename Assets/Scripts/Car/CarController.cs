using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Drift))]
[RequireComponent(typeof(PhotonView))]
public class CarController : MonoBehaviourPun, IPunObservable
{
    public enum Axel { Front, Rear }

    [Serializable]
    public struct Wheel
    {
        public Transform wheelTransform;
        public WheelCollider wheelCollider;
        public TrailRenderer trailRenderer;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    [Header("Car Setup")]
    [SerializeField] private Vector3 _centerOfMass;
    [SerializeField] private List<Wheel> wheels;
    [SerializeField] private float maxSteerAngle = 30f;
    
    [SerializeField] private float torqueMultiplier = 40f;
    [SerializeField] private float steerMultiplier = 0.1f;
    
    [Tooltip("Autosteering multiplier in drift")]
    [SerializeField] private float driftSteerMultiplier = 2f;
    
    [Tooltip("How quickly does the steering wheel adjust")]
    [SerializeField] private float steerLerp = 0.6f;
    
    private float acceleration;
    private float braking;
    private float handling;
    
    private float moveInput;
    private float steerInput;

    private MyButton gasPedal;
    private MyButton brakePedal;
    private MyButton leftButton;
    private MyButton rightButton;
    
    private Rigidbody rb;
    private Drift drift;

    private bool isMine;
    private float networkSteerInput;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = _centerOfMass;
        drift = GetComponent<Drift>();

        isMine = photonView.IsMine;
    }

    private void Update()
    {
        if (!isMine) return;

        GetInputs();
        AnimateWheels();
        WheelEffects();
    }

    private void LateUpdate()
    {
        if (isMine)
        {
            Move();
            Steer();
            Brake();
        }
        else
        {
            SyncRotationWheels();
        }
    }

    private void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        if (gasPedal.GetIsPressed()) moveInput += gasPedal.GetDampenPress();
        
        steerInput = Input.GetAxis("Horizontal");
        if (rightButton.GetIsPressed()) steerInput += rightButton.GetDampenPress();
        if (leftButton.GetIsPressed()) steerInput -= leftButton.GetDampenPress();
    }

    private void Move()
    {
        float targetTorque = moveInput * torqueMultiplier * acceleration;
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = targetTorque;
        }
    }

    private void Steer()
    {
        foreach(var wheel in wheels)
        {
            if (wheel.axel != Axel.Front) continue;
            
            float steerAngle = steerInput * handling * steerMultiplier * maxSteerAngle;

            // Auto steer
            if (drift.GetIsDrifting())
            {
                Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
                float driftSteer = Mathf.Clamp(localVel.x * driftSteerMultiplier, -maxSteerAngle, maxSteerAngle);
                steerAngle += driftSteer;
            }

            wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, steerLerp);
        }
    }

    private void Brake()
    {
        foreach(var wheel in wheels)
        {
            if(brakePedal.GetIsPressed()) wheel.wheelCollider.brakeTorque = torqueMultiplier * braking;
            else wheel.wheelCollider.brakeTorque = 0;
        }
    }

    private void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelTransform.position = pos;
            wheel.wheelTransform.rotation = rot;
        }
    }

    private void WheelEffects()
    {
        foreach(var wheel in wheels)
        {
            if (wheel.axel != Axel.Rear) continue;

            if(brakePedal.GetIsPressed() || drift.GetIsDrifting())
            {
                wheel.trailRenderer.emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.trailRenderer.emitting = false;
            }
        }
    }

    private void SyncRotationWheels()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                float steerAngle = networkSteerInput * handling * steerMultiplier * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, steerLerp);
            }
            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelTransform.position = pos;
            wheel.wheelTransform.rotation = rot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(steerInput);
        else networkSteerInput = (float)stream.ReceiveNext();
    }

    public void SetAcceleration(float value) => acceleration = value;
    public void SetBraking(float value) => braking = value;
    public void SetHandling(float value) => handling = value;
    public void SetGasPedal(MyButton myButton) => gasPedal = myButton;
    public void SetBrakePedal(MyButton mybutton) => brakePedal = mybutton;
    public void SetLeftButton(MyButton mybutton) => leftButton = mybutton;
    public void SetRightButton(MyButton mybutton) => rightButton = mybutton;
}
