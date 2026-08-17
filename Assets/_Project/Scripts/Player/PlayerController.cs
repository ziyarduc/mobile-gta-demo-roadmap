using UnityEngine;
using GTAClone.Core;

namespace GTAClone.Player
{
    /// <summary>
    /// GTA 3 tarzı üçüncü şahıs karakter hareket ve durum kontrolcüsü.
    /// CharacterController bileşeni ile çalışır.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 7.0f;
        [SerializeField] private float rotationSmoothTime = 0.1f;
        
        [Header("Physics")]
        [SerializeField] private float gravity = -18f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("References")]
        [SerializeField] private Transform mainCameraTransform;

        private CharacterController characterController;
        private Vector3 velocity;
        private bool isGrounded;
        private float turnSmoothVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (mainCameraTransform == null && UnityEngine.Camera.main != null)
            {
                mainCameraTransform = UnityEngine.Camera.main.transform;
            }
        }

        private void OnEnable()
        {
            EventManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            EventManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void Update()
        {
            // Sadece yaya modundaysak hareket et
            if (GameManager.Instance != null && GameManager.Instance.CurrentGameState != GameState.OnFoot)
                return;

            HandleGroundAndGravity();
            HandleMovement();
        }

        private void HandleGroundAndGravity()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Zemine tam oturmasını sağla
            }

            // Zıplama
            if (isGrounded && PlayerInputHandler.Instance != null && PlayerInputHandler.Instance.IsJumpPressed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Yerçekimi uygula
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleMovement()
        {
            if (PlayerInputHandler.Instance == null) return;

            Vector2 input = PlayerInputHandler.Instance.MoveInput;
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            if (direction.magnitude >= 0.1f)
            {
                // Kameranın baktığı açıya göre hareket yönü hesaplama
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                float currentSpeed = PlayerInputHandler.Instance.IsSprintPressed ? runSpeed : walkSpeed;

                characterController.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
            }
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.InVehicle)
            {
                // Karakteri gizle ve hareketini devre dışı bırak
                characterController.enabled = false;
                gameObject.SetActive(false);
            }
            else if (newState == GameState.OnFoot)
            {
                // Karakteri tekrar aktif et
                gameObject.SetActive(true);
                characterController.enabled = true;
            }
        }
    }
}
