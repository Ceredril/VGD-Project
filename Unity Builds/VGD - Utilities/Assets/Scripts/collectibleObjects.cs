using UnityEngine;


public class collectibleObjects : MonoBehaviour
{
    private int _amount;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CompareTag("manaCollectible")) {
            GameManager.ManaCollected(_amount=Random.Range(10,20));
            GameManager.OnManaCollected -= playerStatsManager.AddMana;
            Debug.Log("User has collected " + _amount + " mana");
        }
        else if (other.CompareTag("Player") && CompareTag("healthCollectible")){
            GameManager.HealthCollected(_amount=Random.Range(10,20));
            GameManager.OnHealthCollected -= playerStatsManager.AddHealth;
            Debug.Log("User has collected " + _amount + " health");
        }
        Destroy(gameObject);
    }
}
