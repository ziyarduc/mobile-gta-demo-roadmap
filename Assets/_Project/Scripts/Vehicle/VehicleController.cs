using UnityEngine;
using GTAClone.Player;

namespace GTAClone.Vehicle
{
    /// <summary>
    /// Basit ve mobil dostu arcade araç fizik kontrolcüsü.
    /// Rigidbody ve tekerlek süspansiyon raycastleri veya doğrudan tork ile çalışır.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        [Header("Vehicle Settings")]
        [SerializeField] private float motorForce = 1500f;
        [SerializeField] private float brakeForce = 3000f;
        [SerializeField] private float maxSteerAngle = 35f;
        [SerializeField] private float topSpeed = 90f; // km/h

        [Header("Wheel Colliders (Opsiyonel / Standart)")]
        [SerializeField] private WheelCollider frontLeftWheel;
        [SerializeField] private WheelCollider frontRightWheel;
        [SerializeField] private WheelCollider rearLeftWheel;
        [SerializeField] private WheelCollider rearRightWheel;

        [Header("Center of Mass")]
        [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0, -0.5f, 0);

        private Rigidbody rb;
        private bool isBeingDriven = false;

        public bool IsBeingDriven => isBeingDriven;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass += centerOfMassOffset;
        }

        public void SetDriverActive(bool active)
        {
            isBeingDriven = active;
            if (!active)
            {
                // Aracı durdur veya el freni çek
                ApplyBrake(brakeForce);
            }
        }

        private void FixedUpdate()
        {
            if (!isBeingDriven) return;

            float vertical = 0f;
            float horizontal = 0f;

            if (PlayerInputHandler.Instance != null)
            {
                vertical = PlayerInputHandler.Instance.MoveInput.y;
                horizontal = PlayerInputHandler.Instance.MoveInput.x;
            }

            HandleMotor(vertical);
            HandleSteering(horizontal);
        }

        private void HandleMotor(float input)
        {
            float speedKmh = rb.linearVelocity.magnitude * 3.6f;

            if (speedKmh < topSpeed)
            {
                if (rearLeftWheel != null) rearLeftWheel.motorTorque = input * motorForce;
                if (rearRightWheel != null) rearRightWheel.motorTorque = input * motorForce;
            }
            else
            {
                if (rearLeftWheel != null) rearLeftWheel.motorTorque = 0;
                if (rearRightWheel != null) rearRightWheel.motorTorque = 0;
            }
        }

        private void HandleSteering(float input)
        {
            float steer = input * maxSteerAngle;
            if (frontLeftWheel != null) frontLeftWheel.steerAngle = steer;
            if (frontRightWheel != null) frontRightWheel.steerAngle = steer;
        }

        private void ApplyBrake(float force)
        {
            if (frontLeftWheel != null) frontLeftWheel.brakeTorque = force;
            if (frontRightWheel != null) frontRightWheel.brakeTorque = force;
            if (rearLeftWheel != null) rearLeftWheel.brakeTorque = force;
            if (rearRightWheel != null) rearRightWheel.brakeTorque = force;
        }
    }
}
