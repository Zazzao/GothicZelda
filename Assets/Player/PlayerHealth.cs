using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int curHealth;
    [SerializeField] private int maxHealth;

    public void ChangeHealth(int amount) {
        curHealth += amount;

        if (curHealth > maxHealth) { 
            curHealth = maxHealth;
        }
        if (curHealth <= 0) {
            curHealth = 0;
            gameObject.SetActive(false); //temp "dying"
        }
    }
}
