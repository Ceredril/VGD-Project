using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    enum EnemyType
    {
        Melee,Ranged,Boss
    }

    [SerializeField] EnemyType enemyType;
    Animator animator;
    private NavMeshAgent _agent;
    private Transform _player;
    [SerializeField] private Rigidbody enemyBullet;
    private LayerMask _groundLayer, _playerLayer;
    
    private static bool _isAlive=true;
    public int _currentHealth;
    public int _maxHealth;
    private float _sightRange;
    private float _walkPointRange;
    private float _attackRange;
    private bool _canAttack=true;
    private float _attackCooldown;
    private readonly float _bulletSpeed=1200f;
   

    private Vector3 _walkPoint;
    private bool _walkPointSet;

    private void Awake()
    {
        //GameManager.OnGameSave += SaveProgress;
        //GameManager.OnGameLoad += LoadProgress;
        //GameManager.OnGameNew += NewSave;
    }

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        _player = GameObject.Find("Player Body").transform;
        _agent.SetDestination(_player.position);
        _groundLayer = LayerMask.GetMask("Ground");
        _playerLayer = LayerMask.GetMask("Player");
        
        switch (enemyType)
        {
            case EnemyType.Melee:
                _sightRange = 10f;
                _walkPointRange = 6f;
                _attackRange = 2f;
                _attackCooldown = 4f;
                _maxHealth = 60;
                break;
            case EnemyType.Ranged:
                _sightRange = 18f;
                _walkPointRange = 6f;
                _attackRange = 12f;
                _attackCooldown = 3f;
                _maxHealth = 90;
                break;
            case EnemyType.Boss:
                _sightRange = 18f;
                _walkPointRange = 6f;
                _attackRange = 12f;
                _attackCooldown = 3f;
                _maxHealth = 500;
                break;
        }
    }

    private void OnDestroy()
    {
        //GameManager.OnGameSave -= SaveProgress;
        //GameManager.OnGameLoad -= LoadProgress;
        //GameManager.OnGameNew -= NewSave;
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
                PlayerManager.AddHealth(Random.Range(-15,-30));
                animator.SetTrigger("swip");  
                StartCoroutine(AttackCooldown());
                break;
            case EnemyType.Ranged:
                Transform thisTransform = transform;
                Rigidbody bulletClone = Instantiate(enemyBullet, thisTransform.position+new Vector3(0,1f,0), thisTransform.rotation);
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
    
    public void ReduceHealth(int amount, Enemy enemy)
    {
        if (enemy == this)
        {
            _currentHealth -= amount;
            if (_currentHealth < 1)
            {
                _isAlive = false;
                gameObject.SetActive(false);
            }
        }
    }

    private void NewSave()
    {
        _currentHealth = _maxHealth;
        gameObject.SetActive(true);
        SaveProgress();
    }

    private void LoadProgress()
    {
        string healthString = name + "health";
        string aliveString = name + "isAlive";
        _currentHealth = PlayerPrefs.GetInt(healthString);
        _isAlive = Convert.ToBoolean(PlayerPrefs.GetInt(aliveString));
    }

    private void SaveProgress()
    {
        string healthString = name + "health";
        string aliveString = name + "isAlive";
        PlayerPrefs.SetInt(healthString,_currentHealth);
        PlayerPrefs.SetInt(aliveString, Convert.ToInt32(_isAlive));
        PlayerPrefs.Save();
    }
}
