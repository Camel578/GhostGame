using UnityEngine;
using Sample;

namespace Sample
{
    public class RewardSystem : MonoBehaviour
    {
        [SerializeField] private Coin[] _coins;
        [SerializeField] private Transform _nextLevelPoint;

        private GhostScript _ghostScript;

        void Start()
        {
            // Ищем призрака на сцене
            _ghostScript = FindObjectOfType<GhostScript>();
        }

        public void AddCoin()
        {
            DataContainer._coins++;

            if (DataContainer._coins >= _coins.Length)
            {
                if (_nextLevelPoint != null)
                {
                    TeleportPlayer();
                }
                else
                {
                    Debug.Log("Все монеты собраны, но точка телепортации не задана!");
                }
            }
        }

        private void TeleportPlayer()
        {
            if (_ghostScript == null) return;

            CharacterController cc = _ghostScript.GetComponent<CharacterController>();

            cc.enabled = false; // Выключаем для перемещения

            _ghostScript.transform.position = _nextLevelPoint.position;
            _ghostScript.transform.rotation = _nextLevelPoint.rotation;

            // Метод в GhostScript, который мы добавим ниже
            _ghostScript.SetNewRespawnPoint(_nextLevelPoint);

            DataContainer._coins = 0; // Сброс для новой зоны

            cc.enabled = true;
        }
    }
}