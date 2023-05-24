using UnityEngine;


public class collectibleObjects : MonoBehaviour
{
    private int _amount;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody") && CompareTag("manaCollectible")) {
            GameManager.ManaCollected(_amount=Random.Range(10,20));
            GameManager.OnManaCollected -= PlayerManager.AddMana;
            Debug.Log("User has collected " + _amount + " mana");
        }
        else if (other.CompareTag("PlayerBody") && CompareTag("healthCollectible")){
            GameManager.HealthCollected(_amount=Random.Range(10,20));
            GameManager.OnHealthCollected -= PlayerManager.AddHealth;
            Debug.Log("User has collected " + _amount + " health");
        }
        Destroy(gameObject);
    }
}
