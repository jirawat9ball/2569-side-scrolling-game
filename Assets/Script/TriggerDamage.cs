using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    public int DamageAmount = 1;
    public GameObject Effect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { 
            Player player = collision.GetComponent<Player>();
            Instantiate(Effect, player.transform.position, Quaternion.identity);
            player.TakeDamage(DamageAmount);
        }
    }
}
