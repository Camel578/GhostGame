using UnityEngine;
using UnityEngine.AI;
using Sample;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки после убийства")]
    [SerializeField] private float _postKillCooldown = 3f; // Сколько секунд НПС «остывает»
    private float _cooldownTimer = 0f;
    [Header("Настройки обзора")]
    [SerializeField] private float _viewRadius = 8f;
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 90f;
    [SerializeField] private LayerMask _obstacleMask;     // СЮДА НУЖНО ВЫБРАТЬ ТОЛЬКО СЛОЙ СТЕН! Поставь слой "Walls" или проверь, чтобы там НЕ БЫЛО слоя игрока.

    [Header("Патрулирование")]
    [SerializeField] private Transform[] _waypoints;

    [Header("Настройки атаки")]
    [SerializeField] private float _killDistance = 1.2f;   // Дистанция, ближе которой призрак гарантированно умирает

    private NavMeshAgent _agent;
    private Transform _player;
    private GhostScript _ghostScript;
    private int _currentWaypointIndex;
    private bool _isChasing;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _ghostScript = FindObjectOfType<GhostScript>();
        if (_ghostScript != null)
        {
            _player = _ghostScript.transform;
        }

        if (_agent != null && _agent.isOnNavMesh)
        {
            GoToNextWaypoint();
        }
    }

    void Update()
    {
        if (_player == null) return;

        // Если таймер «остывания» идет — просто идем по маршруту и ничего не видим
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            Patrol(); // Просто ходим
            return; // Выходим из метода, дальше ничего не делаем
        }

        // Защита от зависания на NavMesh
        if (!_agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
            }
            return;
        }

        // 1. ЖЕСТКАЯ ПРОВЕРКА НА УБИЙСТВО (работает всегда, даже если все стоят на месте)
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer <= _killDistance)
        {
            KillPlayer();
            return;
        }

        // 2. Логика преследования и обзора
        if (CanSeePlayer(distanceToPlayer))
        {
            _isChasing = true;
            ChasePlayer();
        }
        else
        {
            if (_isChasing)
            {
                _isChasing = false;
                GoToNextWaypoint();
            }
            Patrol();
        }
    }

    bool CanSeePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= _viewRadius)
        {
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < _viewAngle / 2)
            {
                // Поднимаем точки луча на 0.5 метра вверх (на уровень глаз), чтобы луч не терся о пол
                Vector3 eyePosition = transform.position + Vector3.up * 0.5f;
                Vector3 playerEyePosition = _player.position + Vector3.up * 0.5f;

                // Пускаем луч. Если он попал в препятствие из Obstacle Mask — значит игрока не видно
                if (!Physics.Linecast(eyePosition, playerEyePosition, _obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ChasePlayer()
    {
        _agent.SetDestination(_player.position);
    }

    void Patrol()
    {
        if (_waypoints.Length == 0) return;

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (_waypoints.Length == 0 || !_agent.isOnNavMesh) return;
        _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
    }


    private void KillPlayer()
    {
        if (_ghostScript != null)
        {
            _ghostScript.Damage();

            // НПС «забывает» игрока и уходит на отдых
            _isChasing = false;
            _cooldownTimer = _postKillCooldown; // Включаем таймер невидимости

            // Сразу направляем его к следующей точке, чтобы он ушел от спавна
            GoToNextWaypoint();
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -_viewAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, _viewAngle / 2f, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, (transform.position + Vector3.up * 0.5f) + leftBoundary * _viewRadius);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, (transform.position + Vector3.up * 0.5f) + rightBoundary * _viewRadius);
    }
}