using UnityEngine;

public class TriggerGrivity : MonoBehaviour
{
    public float ForceSpeed = 3;
    public Vector2 ForceDirection = Vector2.up;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.AddForce(ForceDirection.normalized * ForceSpeed, ForceMode2D.Force);
        }
    }
}
