using UnityEngine;

namespace GTAClone.Player
{
    /// <summary>
    /// Hem PC (Klavye/Mouse) hem de Mobil (Sanal Joystick / Touch) girdilerini ortak bir arayüze dönüştüren sınıf.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        public static PlayerInputHandler Instance { get; private set; }

        [Header("Movement Input")]
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        
        [Header("Action Buttons")]
        public bool IsSprintPressed { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsInteractPressed { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // Unity Editor / PC Test Kontrolleri
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            MoveInput = new Vector2(h, v).normalized;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            LookInput = new Vector2(mouseX, mouseY);

            IsSprintPressed = Input.GetKey(KeyCode.LeftShift);
            IsJumpPressed = Input.GetKeyDown(KeyCode.Space);
            IsInteractPressed = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E);
        }

        // Mobil Sanal Joystick ve Butonlar için Dışarıdan Çağrılacak Fonksiyonlar
        public void SetMobileMoveInput(Vector2 input) => MoveInput = input;
        public void SetMobileLookInput(Vector2 delta) => LookInput = delta;
        public void SetMobileJump(bool state) => IsJumpPressed = state;
        public void SetMobileSprint(bool state) => IsSprintPressed = state;
        public void SetMobileInteract(bool state) => IsInteractPressed = state;
    }
}
