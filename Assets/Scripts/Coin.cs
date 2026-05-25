using UnityEngine;
using Sample; // Чтобы скрипт видел RewardSystem

public class Coin : MonoBehaviour
{
    private RewardSystem _rewardSystem;
    [SerializeField] private float _rotateCoin = 200f;

    void Start()
    {
        // Ищем RewardSystem в объекте-родителе (Coins)
        _rewardSystem = GetComponentInParent<RewardSystem>();
    }

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * _rotateCoin);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег игрока
        if (other.CompareTag("Player"))
        {
            if (_rewardSystem != null)
            {
                _rewardSystem.AddCoin();
            }
            Destroy(gameObject);
        }
    }
}