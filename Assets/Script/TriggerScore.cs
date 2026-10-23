using UnityEngine;

public class TriggerScore : MonoBehaviour
{
    public int scoreValue = 1;
    public GameObject Effect;
    public AudioClip clip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.AddScore(scoreValue);
                
                GameObject g = Instantiate(Effect,transform.position,Quaternion.identity);
                Soundmanager.instance.PlayVFX(clip);
                Destroy(g, 2);
                Destroy(gameObject);
            }
        }
    }
}
