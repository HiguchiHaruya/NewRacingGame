using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AICarController : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private GameObject _player;
    [SerializeField] private float _baseSpeed = 25;
    private int _currentIndex = 0;
    private Rigidbody _rb;

    private void Start()
    {
        NavMeshSetUp();
        SetNextDestination();
    }

    private void NavMeshSetUp()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
    }

    private void FixedUpdate()
    {
        // WayPoint‚É“ž’B‚µ‚½‚çŽŸ‚Ì–Ú“I’n‚Ö
        Circulate();
        Vector3 targetDir = (_agent.steeringTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);

        float targetSpeed = _player.GetComponent<Rigidbody>().velocity.magnitude * 2;
        _agent.speed = Mathf.Lerp(_baseSpeed, targetSpeed, Time.fixedDeltaTime * 2);
    }
    private void Circulate()
    {
        if (wayPoints.Length < 0) return;
        if (!_agent.pathPending && _agent.remainingDistance < 15f)
        {
            _currentIndex = (_currentIndex + 1) % wayPoints.Length;
            SetNextDestination();
        }
    }

    private void SetNextDestination()
    {
        if (wayPoints.Length == 0) return;
        Vector3 randomOffset = new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-6f, 6f));
        Vector3 targetPosition = wayPoints[_currentIndex].position + randomOffset;
        _agent.SetDestination(targetPosition);
    }
}
