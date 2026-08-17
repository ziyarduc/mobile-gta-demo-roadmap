using UnityEngine;
using GTAClone.Core;
using GTAClone.Player;

namespace GTAClone.Vehicle
{
    /// <summary>
    /// GTA 3 tarzı araca binme / araçtan inme tetikleyicisi ve etkileşim yöneticisi.
    /// </summary>
    public class VehicleInteraction : MonoBehaviour
    {
        [Header("Seat & Exit Position")]
        [SerializeField] private Transform driverSeatPoint;
        [SerializeField] private Transform exitSpawnPoint;

        [Header("Settings")]
        [SerializeField] private float enterDistanceThreshold = 3.0f;

        private VehicleController vehicleController;
        private GameObject currentPlayer;
        private bool isPlayerInside = false;

        private void Awake()
        {
            vehicleController = GetComponent<VehicleController>();
        }

        private void Update()
        {
            if (PlayerInputHandler.Instance == null) return;

            // Araca Binme Kontrolü
            if (!isPlayerInside && PlayerInputHandler.Instance.IsInteractPressed)
            {
                CheckAndEnterVehicle();
            }
            // Araçtan İnme Kontrolü
            else if (isPlayerInside && PlayerInputHandler.Instance.IsInteractPressed)
            {
                ExitVehicle();
            }
        }

        private void CheckAndEnterVehicle()
        {
            if (currentPlayer == null)
            {
                currentPlayer = GameObject.FindWithTag("Player");
            }

            if (currentPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, currentPlayer.transform.position);
                if (distance <= enterDistanceThreshold)
                {
                    EnterVehicle(currentPlayer);
                }
            }
        }

        public void EnterVehicle(GameObject player)
        {
            isPlayerInside = true;
            currentPlayer = player;

            // Oyun durumunu değiştir
            GameManager.Instance?.SetGameState(GameState.InVehicle);

            // Aracı aktif et
            vehicleController.SetDriverActive(true);

            // Olayı tetikle
            EventManager.TriggerPlayerEnteredVehicle(gameObject);
        }

        public void ExitVehicle()
        {
            if (!isPlayerInside) return;

            isPlayerInside = false;

            // Aracı devre dışı bırak
            vehicleController.SetDriverActive(false);

            // Oyuncu karakterini araç yanına yerleştir ve aktif et
            Vector3 exitPos = exitSpawnPoint != null ? exitSpawnPoint.position : transform.position + (transform.right * -2f);
            if (currentPlayer != null)
            {
                currentPlayer.transform.position = exitPos;
                currentPlayer.SetActive(true);
            }

            // Oyun durumunu değiştir
            GameManager.Instance?.SetGameState(GameState.OnFoot);

            // Olayı tetikle
            EventManager.TriggerPlayerExitedVehicle(gameObject);
        }
    }
}
