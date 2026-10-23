using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float speed = 5f;
    public int Hp = 3;
    public int Score = 0;

    public Vector3 RespawnPostion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        RespawnPostion = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Debug.Log("Horizontal: " + h + ", Vertical: " + v);

        //rb2d.linearVelocity = new Vector2(h, v) * 5f;
        rb2d.AddTorque(-h * speed,ForceMode2D.Force);

    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log("Player took " + damage + " damage. Remaining HP: " + Hp);
        Respwan();
        Gamemanager.instance.UpdateHealth(Hp, 3);
        if (Hp <= 0)
        {
            Debug.Log("Player is dead.");
            Destroy(gameObject);
        }
    }

    public void Respwan()
    {
        transform.position = RespawnPostion;
        rb2d.Sleep();
        Debug.Log("Player respawned at: " + RespawnPostion);
    }
    public void AddScore(int score)
    {
        Score += score;
        Gamemanager.instance.UpdateScore(Score);
        Debug.Log("Player gained " 
            + score + " points. Total Score: " + Score);
    }
}
