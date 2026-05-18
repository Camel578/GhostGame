using UnityEngine;
using UnityEngine.AI; // Обязательно для работы ИИ-агента
using Sample;         // Подключаем пространство имен твоего призрака

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки обзора")]
    [SerializeField] private float _viewRadius = 8f;       // Дистанция, на которой враг видит
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 90f;       // Угол обзора конусом (например, 90 градусов перед собой)
    [SerializeField] private LayerMask _obstacleMask;     // Слой стен (чтобы не видел сквозь стены)

    [Header("Патрулирование")]
    [SerializeField] private Transform[] _waypoints;      // Точки, между которыми враг ходит, пока не видит игрока

    private NavMeshAgent _agent;
    private Transform _player;
    private GhostScript _ghostScript;
    private int _currentWaypointIndex;
    private bool _isChasing;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Автоматически находим призрака на сцене
        _ghostScript = FindObjectOfType<GhostScript>();
        if (_ghostScript != null)
        {
            _player = _ghostScript.transform;
        }

        // БЕЗОПАСНАЯ ПРОВЕРКА: Идем к точке только если агент успешно встал на сетку NavMesh
        if (_agent != null && _agent.isOnNavMesh)
        {
            GoToNextWaypoint();
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] Внимание! {gameObject.name} не установлен на NavMesh при старте!");
        }
    }
    void Update()
    {
        if (_player == null) return;

        // Защита от зависания: если НПС сошел с сетки NavMesh, возвращаем его насильно
        if (!_agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
            }
            return;
        }

        // Каждый кадр проверяем, видит ли враг призрака
        if (CanSeePlayer())
        {
            _isChasing = true;
            ChasePlayer();
        }
        else
        {
            // Если потерял из виду — возвращается к патрулированию
            if (_isChasing)
            {
                _isChasing = false;
                GoToNextWaypoint();
            }
            Patrol();
        }
    }

    bool CanSeePlayer()
    {
        // 1. Проверяем расстояние
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer <= _viewRadius)
        {
            // 2. Проверяем угол обзора (смотрит ли враг в сторону игрока)
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < _viewAngle / 2)
            {
                // 3. Проверяем, нет ли между ними стены (кастуем линию-луч)
                // В Physics.Linecast мы передаем позицию врага и игрока, приподнятые чуть вверх (+ Vector3.up)
                if (!Physics.Linecast(transform.position + Vector3.up, _player.position + Vector3.up, _obstacleMask))
                {
                    return true; // Препятствий нет, игрок замечен!
                }
            }
        }
        return false;
    }

    void ChasePlayer()
    {
        _agent.SetDestination(_player.position); // Приказываем бежать прямо к игроку
    }

    void Patrol()
    {
        if (_waypoints.Length == 0) return;

        // Если почти дошли до текущей точки патруля — выбираем следующую
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (_waypoints.Length == 0) return;

        _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
    }

    // Убийство призрака при касании
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (_ghostScript != null)
        {
            _ghostScript.Damage(); // Вызываем возрождение призрака на спавне
        }
    }

    // Отрисовка конуса зрения в окне Scene для твоего удобства
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 viewAngleA = Quaternion.AngleAxis(-_viewAngle / 2, Vector3.up) * transform.forward;
        Vector3 viewAngleB = Quaternion.AngleAxis(_viewAngle / 2, Vector3.up) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _viewRadius);
    }
}