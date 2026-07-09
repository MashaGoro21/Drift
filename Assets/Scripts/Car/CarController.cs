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

    private bool isBraking;
    private bool isMine;
    private bool wheelEffectActive;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

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
            SyncNetworkMovement();
            SyncWheelEffects();
        }
    }

    private void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        if (gasPedal != null && gasPedal.GetIsPressed()) moveInput += gasPedal.GetDampenPress();
        if (brakePedal != null && brakePedal.GetIsPressed()) moveInput -= brakePedal.GetDampenPress();
        
        steerInput = Input.GetAxis("Horizontal");
        if (rightButton != null && rightButton.GetIsPressed()) steerInput += rightButton.GetDampenPress();
        if (leftButton != null && leftButton.GetIsPressed()) steerInput -= leftButton.GetDampenPress();
    }

    private void Move()
    {
        if (isBraking) return;

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
        float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
        isBraking = brakePedal.GetIsPressed() && forwardSpeed > 0.1f;

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.brakeTorque = isBraking ? torqueMultiplier * braking : 0f;
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
        wheelEffectActive = isBraking || drift.GetIsDrifting();
        HandleWheelEffects(wheelEffectActive);
    }

    private void SyncWheelEffects()
    {
        HandleWheelEffects(wheelEffectActive);
    }

    private void HandleWheelEffects(bool effectActive)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel != Axel.Rear) continue;

            if(wheel.trailRenderer != null)
                wheel.trailRenderer.emitting = effectActive;

            if (effectActive && wheel.smokeParticle != null)
                wheel.smokeParticle.Emit(1);
        }
    }

    private void SyncNetworkMovement()
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(wheelEffectActive);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            wheelEffectActive = (bool)stream.ReceiveNext();
        }
    }

    public void StopCarInput()
    {
        moveInput = 0f;
        steerInput = 0f;
        isBraking = false;
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0f;
            wheel.wheelCollider.brakeTorque = 0f;
            wheel.wheelCollider.steerAngle = 0f;
        }
    }

    public void SetAcceleration(float value) => acceleration = value;
    public void SetBraking(float value) => braking = value;
    public void SetHandling(float value) => handling = value;
    public void SetGasPedal(MyButton myButton) => gasPedal = myButton;
    public void SetBrakePedal(MyButton mybutton) => brakePedal = mybutton;
    public void SetLeftButton(MyButton mybutton) => leftButton = mybutton;
    public void SetRightButton(MyButton mybutton) => rightButton = mybutton;
}
