using UnityEngine;
using GTAClone.Core;

namespace GTAClone.Camera
{
    /// <summary>
    /// GTA 3 tarzı üçüncü şahıs kamera. Hem yaya hem de araç modunu destekler,
    /// duvarlara çarpınca yakınlaşır (Camera Collision).
    /// </summary>
    public class ThirdPersonOrbitCamera : MonoBehaviour
    {
        [Header("Target & Following")]
        [SerializeField] private Transform currentTarget;
        [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);

        [Header("Distance & Limits")]
        [SerializeField] private float defaultDistance = 4.5f;
        [SerializeField] private float minDistance = 1.0f;
        [SerializeField] private float maxDistance = 7.0f;
        [SerializeField] private float vehicleDistance = 6.0f;

        [Header("Rotation Speeds")]
        [SerializeField] private float xSpeed = 120.0f;
        [SerializeField] private float ySpeed = 80.0f;
        [SerializeField] private float yMinLimit = -10f;
        [SerializeField] private float yMaxLimit = 65f;

        [Header("Collision Detection")]
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float collisionRadius = 0.25f;

        private float x = 0.0f;
        private float y = 20.0f;
        private float currentDistance;
        private float targetDistance;

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            currentDistance = defaultDistance;
            targetDistance = defaultDistance;
        }

        private void OnEnable()
        {
            EventManager.OnPlayerEnteredVehicle += HandleEnteredVehicle;
            EventManager.OnPlayerExitedVehicle += HandleExitedVehicle;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerEnteredVehicle -= HandleEnteredVehicle;
            EventManager.OnPlayerExitedVehicle -= HandleExitedVehicle;
        }

        public void SetTarget(Transform newTarget)
        {
            currentTarget = newTarget;
        }

        private void LateUpdate()
        {
            if (!currentTarget) return;

            // PC Mouse veya Mobil Touch Delta
            float inputX = Input.GetAxis("Mouse X");
            float inputY = Input.GetAxis("Mouse Y");

            x += inputX * xSpeed * Time.deltaTime;
            y -= inputY * ySpeed * Time.deltaTime;
            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 focusPoint = currentTarget.position + targetOffset;

            // Kamera Duvar Çarpışma Kontrolü (SphereCast)
            Vector3 desiredCameraPos = focusPoint - (rotation * Vector3.forward * targetDistance);
            RaycastHit hit;
            if (Physics.SphereCast(focusPoint, collisionRadius, (desiredCameraPos - focusPoint).normalized, out hit, targetDistance, collisionLayers))
            {
                currentDistance = Mathf.Clamp(hit.distance, minDistance, targetDistance);
            }
            else
            {
                currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * 5f);
            }

            Vector3 finalPosition = focusPoint - (rotation * Vector3.forward * currentDistance);
            transform.rotation = rotation;
            transform.position = finalPosition;
        }

        private void HandleEnteredVehicle(GameObject vehicle)
        {
            currentTarget = vehicle.transform;
            targetDistance = vehicleDistance;
            targetOffset = new Vector3(0, 2.0f, 0);
        }

        private void HandleExitedVehicle(GameObject vehicle)
        {
            // Oyuncu karakterini tekrar bul ve hedef yap
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                currentTarget = player.transform;
                targetDistance = defaultDistance;
                targetOffset = new Vector3(0, 1.8f, 0);
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F) angle += 360F;
            if (angle > 360F) angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
