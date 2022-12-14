using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.PlayerAttackedRanged(Random.Range(-20,-40));
            Destroy(gameObject);
        }
    }
}
