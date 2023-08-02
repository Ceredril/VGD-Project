using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,Ranged,Boss
    }
    
    [SerializeField] public EnemyType enemyType;
    Animator animator;
    EnemyHealthBar healthBar;
    private NavMeshAgent _agent;
    private Transform _player;
    [SerializeField] private Rigidbody enemyBullet;
    private LayerMask _groundLayer, _playerLayer;
    
    public bool isAlive=true;
    public int currentHealth;
    
    public int maxHealth;
    private float _sightRange;
    private float _walkPointRange;
    private float _attackRange;
    private float _attackCooldown;
    private readonly float _bulletSpeed=1200f;
    private float _lastAttackTime;

    private Vector3 _walkPoint;
    private bool _walkPointSet;
    

    private void OnDestroy()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        SetStats();
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        _player = GameObject.Find("Player Body").transform;
        _agent.SetDestination(_player.position);
        _groundLayer = LayerMask.GetMask("Ground");
        _playerLayer = LayerMask.GetMask("Player");
        EnemyManager.Instance.RegisterEnemy(this);  
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(transform.position, _attackRange, _playerLayer) && PlayerManager.IsAlive) AttackPlayer(enemyType);
        if (Physics.CheckSphere(transform.position, _sightRange, _playerLayer) && PlayerManager.IsAlive) ChasePlayer();
        else Patrolling();
        Vector3 distanceToWalkPoint = transform.position - _walkPoint;
        if (distanceToWalkPoint.magnitude < 5f) _walkPointSet = false;
        if (currentHealth < 0) isAlive = false;
        if (!isAlive)
        {
            GameManager.EnemyKilled(gameObject);
            gameObject.SetActive(false);
        }
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
        if(Time.time - _lastAttackTime <= _attackCooldown)return;
        switch (enemyType)
        {
            case EnemyType.Melee:
                PlayerManager.AddHealth(Random.Range(-15, -30));
                animator.SetTrigger("swip");
                _lastAttackTime = Time.time;
                AudioSource audiosource = gameObject.AddComponent<AudioSource>();
                GameManager.audioManager.Play("meleeAttack", audiosource);
                break;
            case EnemyType.Ranged:
                Transform thisTransform = transform;
                Rigidbody bulletClone = Instantiate(enemyBullet, thisTransform.position + new Vector3(0, 1f, 0), thisTransform.rotation); 
                Vector3 bulletDirection = _player.position - transform.position;
                bulletClone.AddForce(bulletDirection.normalized * _bulletSpeed);
                _lastAttackTime = Time.time;
                AudioSource audiosource2 = gameObject.AddComponent<AudioSource>();
                GameManager.audioManager.Play("rangeAttack", audiosource2);
                break;
        }
    }

    public void ReduceHealth(int amount, Enemy enemy)
    {
        if (enemy == this)
        {
            currentHealth -= amount;
        }
        healthBar.UpdateHealthBar();
    }

    private void SetStats()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                _sightRange = 10f;
                _walkPointRange = 6f;
                _attackRange = 2f;
                _attackCooldown = 4f;
                currentHealth=maxHealth = 60;
                break;
            case EnemyType.Ranged:
                _sightRange = 18f;
                _walkPointRange = 6f;
                _attackRange = 12f;
                _attackCooldown = 3f;
                currentHealth=maxHealth = 90;
                break;
            case EnemyType.Boss:
                _sightRange = 18f;
                _walkPointRange = 6f;
                _attackRange = 12f;
                _attackCooldown = 3f;
                currentHealth=maxHealth = 500;
                break;
        }
    }



}
