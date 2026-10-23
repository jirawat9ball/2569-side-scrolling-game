using UnityEngine;

public class TriggerCheckPoint : MonoBehaviour
{
    public GameObject Effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.RespawnPostion = transform.position;
                GameObject g = Instantiate(Effect, transform.position, Quaternion.identity);
                Destroy(g, 1f);
            }
        }
    }
}
