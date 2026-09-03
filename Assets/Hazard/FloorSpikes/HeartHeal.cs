using UnityEngine;

public class HeartHeal : MonoBehaviour
{


    [SerializeField] int healAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMotor.Instance.Heal(healAmount);
            GameObject.Destroy(this.gameObject);
            

        }
    }
}
