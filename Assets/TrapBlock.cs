using UnityEngine;
using Sample; // Подключаем пространство имен вашего призрака

public class TrapBlock : MonoBehaviour
{
    [Header("Настройки таймингов (в секундах)")]
    [SerializeField] private float _groundDuration = 3f; // Сколько секунд это земля
    [SerializeField] private float _lavaDuration = 2f;   // Сколько секунд это лава
    [SerializeField] private float _startDelay = 0f;     // Задержка на старте (чтобы блоки работали несинхронно)

    [Header("Ссылки на визуал")]
    [SerializeField] private GameObject _groundVisual;   // Объект обычной земли (куб земли)
    [SerializeField] private GameObject _lavaVisual;     // Объект лавы (куб лавы)

    private float _timer;
    private bool _isLava;

    void Start()
    {
        _isLava = false;
        UpdateVisuals();

        // Задаем начальное время с учетом задержки
        _timer = _groundDuration - _startDelay;
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            // Переключаем состояние (если была лава — станет землей, и наоборот)
            _isLava = !_isLava;

            // Задаем таймер для нового состояния
            _timer = _isLava ? _lavaDuration : _groundDuration;

            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (_groundVisual != null) _groundVisual.SetActive(!_isLava);
        if (_lavaVisual != null) _lavaVisual.SetActive(_isLava);
    }

    private void OnTriggerStay(Collider other)
    {
        // Используем OnTriggerStay, чтобы урон наносился, даже если игрок просто стоит на блоке в момент превращения
        if (_isLava && other.CompareTag("Player"))
        {
            GhostScript ghost = other.GetComponent<GhostScript>();
            if (ghost != null)
            {
                ghost.Damage(); // Вызываем ваш метод получения урона/смерти
            }
        }
    }
}