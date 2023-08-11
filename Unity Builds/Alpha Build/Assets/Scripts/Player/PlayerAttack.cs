using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    
    public enum Skill {Fist,Fireball,Shield}
    //Object references
    private Animator animator;
    private Enemy _nearEnemy;
    public static GameObject wand;
    public static GameObject fireFist;
    public static GameObject shield;
    [SerializeField] private Transform characterCamera;
    [SerializeField] private Rigidbody playerBullet;
    //Skill parameters
    public static float _fistCooldown=2;
    public static int _fireballCooldown=2;
    private float _lastFistTime;
    private float _lastFireballTime;
    public static int fireBallManaUse = 20;
    public static float _bulletSpeed=1800;
    //Skill variables
    public static bool _hasFist = true;
    public static bool _hasFireFist = false;
    public static bool _hasFireball = true;
    public static bool _hasShield = true;
    public static Skill _currentSkill;
    public static int _minFistDamage = 20, _maxFistDamage=30;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterCamera = GameObject.Find("Main Camera").transform;
        wand = GameObject.Find("Wand");
        fireFist = GameObject.Find("fireFist");
        shield = GameObject.Find("Shield");
        wand.SetActive(false);
        fireFist.SetActive(false);
        shield.SetActive(false);
    }

    void Update()
    {
        if (!PlayerManager.IsAlive || GameManager.GameIsPaused) return;
        SkillSelection();
        InputManagement();
        FireFistEffect();
        ShieldEffect();
    }

    private void SkillSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _hasFist && _currentSkill!=Skill.Fist)
        {
            wand.SetActive(false);
            _currentSkill = Skill.Fist;
            shield.SetActive(false);
            Debug.Log("Melee attack selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && _hasFireball && _currentSkill!=Skill.Fireball)
        {
            fireFist.SetActive(false);
            wand.SetActive(true);
            shield.SetActive(false);
            _currentSkill = Skill.Fireball;
            Debug.Log("Ranged attack selected");
        }


        if (Input.GetKeyDown(KeyCode.Alpha3) && _hasShield && _currentSkill != Skill.Shield && PlayerManager.CurrentMana >= PlayerManager.ShieldSManaUse)
        {
            fireFist.SetActive(false);
            wand.SetActive(false);
            shield.SetActive(true);
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
            AudioSource audiosource = gameObject.AddComponent<AudioSource>();
            GameManager.audioManager.PlayLocal("meleeAttack", audiosource);
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
        if (Time.time - _lastFireballTime >= _fireballCooldown  &&  PlayerManager.CurrentMana >= fireBallManaUse)
        {
            animator.SetTrigger("magicAttack");
            StartCoroutine(WaitFire());
            PlayerManager.AddMana(-fireBallManaUse);
            _lastFireballTime = Time.time;        }
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

    private void FireFistEffect()
    {
        if (_currentSkill == Skill.Fist && _hasFireFist)
            fireFist.SetActive(true);
        else
            fireFist.SetActive(false);
    }

    private void ShieldEffect()
    {
        if (_currentSkill == Skill.Shield && PlayerManager.CurrentMana >= PlayerManager.ShieldSManaUse)
            shield.SetActive(true);
        else
            shield.SetActive(false);
    }

    private IEnumerator WaitFire()
    {
        yield return new WaitForSeconds(0.5f);
        Transform thisTransform = transform;
        Rigidbody bulletClone = Instantiate(playerBullet, thisTransform.position + new Vector3(0, 2f, 0) + Vector3.forward,
            thisTransform.rotation);
        Vector3 bulletDirection = characterCamera.forward;
        bulletClone.AddForce(bulletDirection.normalized * _bulletSpeed);
    }
}
