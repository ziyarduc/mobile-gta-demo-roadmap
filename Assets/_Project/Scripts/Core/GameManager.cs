using UnityEngine;

namespace GTAClone.Core
{
    /// <summary>
    /// Oyunun ana kontrolünü ve durum yönetimini üstlenen Singleton sınıfı.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentGameState = GameState.OnFoot;
        public GameState CurrentGameState => currentGameState;

        [Header("Economy & Stats")]
        [SerializeField] private int playerMoney = 5000;
        public int PlayerMoney => playerMoney;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SetGameState(GameState.OnFoot);
        }

        /// <summary>
        /// Oyun durumunu değiştirir ve ilgili tüm sistemleri Event üzerinden haberdar eder.
        /// </summary>
        public void SetGameState(GameState newState)
        {
            currentGameState = newState;
            EventManager.TriggerGameStateChanged(newState);
            Debug.Log($"[GameManager] Oyun Durumu Değişti: {newState}");
        }

        /// <summary>
        /// Oyuncuya para ekler veya çıkartır.
        /// </summary>
        public bool TrySpendMoney(int amount)
        {
            if (playerMoney >= amount)
            {
                playerMoney -= amount;
                EventManager.TriggerMoneyChanged(playerMoney);
                return true;
            }
            return false;
        }

        public void AddMoney(int amount)
        {
            playerMoney += amount;
            EventManager.TriggerMoneyChanged(playerMoney);
        }
    }
}
