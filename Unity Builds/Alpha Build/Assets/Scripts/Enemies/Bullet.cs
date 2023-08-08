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
            PlayerManager playerManager  = other.GetComponent<PlayerManager>();
            PlayerManager.AddHealth(Random.Range(-10,-20));
            Destroy(gameObject);
        }
        
        }

    private void OnEnable()
    {
        AudioSource audiosource = gameObject.AddComponent<AudioSource>();
        GameManager.audioManager.PlayLocal("Fireball", audiosource);
        StartCoroutine(vanishingTime());
    }

    private IEnumerator vanishingTime()
    {
        yield return new WaitForSeconds(_vanishingTime);
        Destroy(gameObject);
    }
}
