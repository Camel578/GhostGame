using UnityEngine;
using Sample;

namespace Sample
{
    public class RewardSystem : MonoBehaviour
    {
        [SerializeField] private Coin[] _coins;
        [SerializeField] private Transform _nextLevelPoint;
        [SerializeField] private GameObject _winWindow; // Перетащи сюда свое окно победы в инспекторе
        [SerializeField] private bool _isLastLevel;     // Поставь эту галочку в инспекторе на последнем уровне

        private GhostScript _ghostScript;

        void Start()
        {
            _ghostScript = FindObjectOfType<GhostScript>();
            if (_winWindow != null) _winWindow.SetActive(false);
        }

        public void AddCoin()
        {
            DataContainer._coins++;

            if (DataContainer._coins >= _coins.Length)
            {
                if (_isLastLevel)
                {
                    ShowWinWindow();
                }
                else if (_nextLevelPoint != null)
                {
                    TeleportPlayer();
                }
            }
        }

        private void ShowWinWindow()
        {
            if (_winWindow != null)
            {
                _winWindow.SetActive(true);
                Time.timeScale = 0f; // Пауза игры
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void TeleportPlayer()
        {
            if (_ghostScript == null) return;

            CharacterController cc = _ghostScript.GetComponent<CharacterController>();
            cc.enabled = false;

            _ghostScript.transform.position = _nextLevelPoint.position;
            _ghostScript.transform.rotation = _nextLevelPoint.rotation;
            _ghostScript.SetNewRespawnPoint(_nextLevelPoint);

            // 1. ВКЛЮЧАЕМ ЗАМОРОЗКУ ИГРОКА (чтобы не улетал от зажатых кнопок)
            _ghostScript.FreezeOnNextLevel(0.3f);

            // 2. ОГЛУШАЕМ ВСЕХ НПС НА СЦЕНЕ: находим их и сбрасываем агрессию
            EnemyAI[] allEnemies = FindObjectsOfType<EnemyAI>();
            foreach (EnemyAI enemy in allEnemies)
            {
                if (enemy != null)
                {
                    enemy.ResetEnemyOnTeleport();
                }
            }

            DataContainer._coins = 0;
            cc.enabled = true;
        }
    }
}