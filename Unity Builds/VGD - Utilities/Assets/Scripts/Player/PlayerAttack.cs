using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    
    private enum Skill {Fist,Fireball,Shield}
    private Animator animator;
    private Enemy _nearEnemy;


    private float _fistCooldown=2;
    private int _fireballCooldown;
    private float _lastAttackTime;

    
    
    private static bool _hasFist = true;
    private static bool _hasFireball = true;
    //private static bool _hasShield = true;
    private Skill _currentSkill;
    
    private readonly int _minFistDamage = 20, _maxFistDamage=30;
    private readonly int _minFireballDamage = 10, _maxFireballDamage = 20;


    private void Awake()
    {
        animator = GetComponent<Animator>();
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
            _currentSkill = Skill.Fist;
            Debug.Log("Melee attack selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && _hasFist && _currentSkill!=Skill.Fireball)
        {
            _currentSkill = Skill.Fireball;
            Debug.Log("Ranged attack selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && _hasFist && _currentSkill != Skill.Shield)
        {
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
        if (Time.time - _lastAttackTime >= _fistCooldown)
        {
            animator.SetTrigger("hook");
            if (_nearEnemy != null)
            {
                int damage = Random.Range(_minFistDamage, _maxFistDamage);
                enemy._currentHealth -= damage;
            }

            _lastAttackTime = Time.time;
        }
    }

    private void Fireball()
    {
        int damage = Random.Range(_minFireballDamage, _maxFireballDamage);
        //Throws a fireball prefab towards where the main camera is pointing at.
        //If the prefab hits a object of tag "Enemy", it reduces the enemy's parameter _currentHealth
    }

    private void Shield()
    {
        //TBD
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("HasMeleeAttack", Convert.ToInt32(_hasFist));
        PlayerPrefs.SetInt("HasRangedAttack", Convert.ToInt32(_hasFireball));
        PlayerPrefs.SetInt("CurrentAttack", (int)_currentSkill);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        _hasFist = Convert.ToBoolean(PlayerPrefs.GetInt("HasMeleeAttack"));
        _hasFireball = Convert.ToBoolean(PlayerPrefs.GetInt("hasRangedAttack"));
        _currentSkill = (Skill)PlayerPrefs.GetInt("CurrentAttack");
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
