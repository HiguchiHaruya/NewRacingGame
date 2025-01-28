using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AICarController : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform[] wayPoints;
    private int _currentIndex = 0;
    private Rigidbody _rb;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetNextDestination();
        _agent.updateRotation = false;
    }

    private void FixedUpdate()
    {
        // WayPoint‚É“ž’B‚µ‚½‚çŽŸ‚Ì–Ú“I’n‚Ö
        Circulate();
        Vector3 targetDir = (wayPoints[_currentIndex].position - transform.position).normalized;
        SetRotate(targetDir);
    }

    private void Circulate()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 5.0f)
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
    private void SetRotate(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        if (_rb.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 50 * Time.fixedDeltaTime));
        }
    }
}
