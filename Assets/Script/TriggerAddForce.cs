using UnityEngine;

public class TriggerAddForce : MonoBehaviour
{
    public float ForceSpeed = 10;
    public Vector2 ForceDirection = Vector2.right;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.AddForce(ForceDirection.normalized * ForceSpeed, ForceMode2D.Impulse);
        }
    }
}
