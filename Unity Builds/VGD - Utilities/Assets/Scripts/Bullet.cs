using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    private readonly float _vanishingTime = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            GameManager.RangedEnemyAttacks(Random.Range(-20,-40));
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(vanishingTime());
    }

    private IEnumerator vanishingTime()
    {
        yield return new WaitForSeconds(_vanishingTime);
        Destroy(gameObject);
    }
}
