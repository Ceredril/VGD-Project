using System.Collections;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private readonly float _vanishingTime = 1;
    private readonly int _minDamage = 30, _maxDamage=40;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            int damage = Random.Range(_minDamage, _maxDamage);
            enemy.ReduceHealth(damage,enemy);
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(VanishingTime());
    }

    private IEnumerator VanishingTime()
    {
        yield return new WaitForSeconds(_vanishingTime);
        Destroy(gameObject);
    }
}
