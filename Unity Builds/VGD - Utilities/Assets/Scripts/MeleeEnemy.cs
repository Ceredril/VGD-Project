using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _player;

    private readonly float _sightRange = 10f;
    private readonly float _walkPointRange = 6f;
    private readonly float _attackRange = 2f;
    private bool _canAttack=true;
    private readonly float _attackCooldown = 4f;

    private LayerMask _groundLayer, _playerLayer;

    private Vector3 _walkPoint;
    private bool _walkPointSet;

    private void Awake()
    {
        _agent = FindObjectOfType<NavMeshAgent>();
        _player = GameObject.Find("Player Body").transform;
        _agent.SetDestination(_player.position);
        _groundLayer = LayerMask.GetMask("Ground");
        _playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (Physics.CheckSphere(transform.position, _attackRange, _playerLayer) && _canAttack)AttackPlayer();
        if (Physics.CheckSphere(transform.position, _sightRange, _playerLayer)) ChasePlayer();
        else Patrolling();
        

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) _walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = Random.Range(-_walkPointRange, _walkPointRange);

        Vector3 transformPosition = transform.position;
        _walkPoint = new Vector3(transformPosition.x + randomX, transformPosition.y, transformPosition.z + randomZ);

        if (Physics.Raycast(_walkPoint, -transform.up, 2f, _groundLayer)) _walkPointSet = true;
    }

    void Patrolling()
    {
        if (!_walkPointSet) SearchWalkPoint();
        else _agent.SetDestination(_walkPoint);
    }

    void ChasePlayer() => _agent.SetDestination(_player.position);

    private void AttackPlayer()
    {
        GameManager.PlayerAttackedMelee(Random.Range(-15, -30));
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }
}