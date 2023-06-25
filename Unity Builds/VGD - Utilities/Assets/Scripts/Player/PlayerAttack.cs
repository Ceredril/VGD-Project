using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    
    public enum Skill {Fist,Fireball,Shield}
    //Object references
    private Animator animator;
    private Enemy _nearEnemy;
    public static GameObject wand;
    [SerializeField] private Transform characterCamera;
    [SerializeField] private Rigidbody playerBullet;
    //Skill parameters
    private float _fistCooldown=2;
    private int _fireballCooldown=2;
    private float _lastFistTime;
    private float _lastFireballTime;
    private float _bulletSpeed=1800;
    //Skill variables
    public static bool _hasFist = true;
    public static bool _hasFireball = true;
    public static bool _hasShield = false;
    public static Skill _currentSkill;
    private readonly int _minFistDamage = 20, _maxFistDamage=30;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterCamera = GameObject.Find("Main Camera").transform;
        wand = GameObject.Find("Wand");
        wand.SetActive(false);
    }

    void Update()
    {
        SkillSelection();
        InputManagement();
    }

    private void SkillSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _hasFist && _currentSkill!=Skill.Fist)
        {
            wand.SetActive(false);
            _currentSkill = Skill.Fist;
            Debug.Log("Melee attack selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && _hasFireball && _currentSkill!=Skill.Fireball)
        {
            wand.SetActive(true);
            _currentSkill = Skill.Fireball;
            Debug.Log("Ranged attack selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && _hasShield && _currentSkill != Skill.Shield)
        {
            wand.SetActive(false);
            _currentSkill = Skill.Shield;
            Debug.Log("Shield attack selected");
        }
    }

    private void InputManagement()
    {
        if (Input.GetMouseButton(0))
        {
            switch (_currentSkill)
            {
                case Skill.Fist:
                    Fist(_nearEnemy);
                    break;
                case Skill.Fireball:
                    Fireball();
                    break;
                case Skill.Shield:
                    Shield();
                    break;
            }
        }
    }

    private void Fist(Enemy enemy)
    {
        if (Time.time - _lastFistTime >= _fistCooldown)
        {
            animator.SetTrigger("hook");
            if (_nearEnemy != null)
            {
                int damage = Random.Range(_minFistDamage, _maxFistDamage);
                enemy.ReduceHealth(damage,enemy);
            }
            _lastFistTime = Time.time;
        }
    }

    private void Fireball()
    {
        if (Time.time - _lastFireballTime >= _fireballCooldown)
        {
            animator.SetTrigger("magicAttack");
            Transform thisTransform = transform;
            Rigidbody bulletClone = Instantiate(playerBullet, thisTransform.position + new Vector3(0, 2f, 0) + Vector3.forward,
                thisTransform.rotation);
            Vector3 bulletDirection = characterCamera.forward;
            bulletClone.AddForce(bulletDirection.normalized * _bulletSpeed);
            _lastFireballTime = Time.time;
        }
    }

    private void Shield()
    {
        //TBD
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("HasMeleeAttack", Convert.ToInt32(_hasFist));
        PlayerPrefs.SetInt("HasRangedAttack", Convert.ToInt32(_hasFireball));
        PlayerPrefs.SetInt("HasShield", Convert.ToInt32(_hasShield));
        PlayerPrefs.SetInt("CurrentAttack", (int)_currentSkill);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1)
        {
            _hasFist = Convert.ToBoolean(PlayerPrefs.GetInt("HasMeleeAttack"));
            _hasFireball = Convert.ToBoolean(PlayerPrefs.GetInt("hasRangedAttack"));
            _hasShield = Convert.ToBoolean(PlayerPrefs.GetInt("HasShield"));
            _currentSkill = (Skill)PlayerPrefs.GetInt("CurrentAttack");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) _nearEnemy = other.GetComponent<Enemy>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) _nearEnemy = null;
    }
}
