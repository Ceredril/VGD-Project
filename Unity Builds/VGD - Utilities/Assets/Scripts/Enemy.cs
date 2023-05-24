using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    enum EnemyType
    {
        Melee,Ranged,Boss
    }

    [SerializeField] EnemyType enemyType;
    
    private NavMeshAgent _agent;
    private Transform _player;
    [SerializeField] private Rigidbody _bullet;
    
    private float _sightRange;
    private float _walkPointRange;
    private float _attackRange;
    private bool _canAttack=true;
    private float _attackCooldown;
    private float _bulletSpeed=1200f;
    
    private LayerMask _groundLayer, _playerLayer;

    private Vector3 _walkPoint;
    private bool _walkPointSet;
    // Start is called before the first frame update
    void Start()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                _sightRange = 10f;
                _walkPointRange = 6f;
                _attackRange = 2f;
                _attackCooldown = 2f;
                break;
            case EnemyType.Ranged:
                _sightRange = 18f;
                _walkPointRange = 6f;
                _attackRange = 12f;
                _attackCooldown = 3f;
                break;
        }
        
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player Body").transform;
        _agent.SetDestination(_player.position);
        _groundLayer = LayerMask.GetMask("Ground");
        _playerLayer = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(transform.position, _attackRange, _playerLayer) && _canAttack && GameManager.PlayerIsAlive) AttackPlayer(enemyType);
        if (Physics.CheckSphere(transform.position, _sightRange, _playerLayer) && GameManager.PlayerIsAlive) ChasePlayer();
        else Patrolling();
        Vector3 distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 5f) _walkPointSet = false;
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

    void ChasePlayer()
    {
        _agent.SetDestination(_player.position);
    }

    private void AttackPlayer(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                GameManager.MeleeEnemyAttacks(Random.Range(-15, -30));
                StartCoroutine(AttackCooldown());
                break;
            case EnemyType.Ranged:
                Transform thisTransform = transform;
                Rigidbody bulletClone = Instantiate(_bullet, thisTransform.position+new Vector3(0,1f,0), thisTransform.rotation);
                Vector3 bulletDirection = _player.position - transform.position;
                bulletClone.AddForce(bulletDirection.normalized * _bulletSpeed);
                StartCoroutine(AttackCooldown());
                break;
        }

    }

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }
}
