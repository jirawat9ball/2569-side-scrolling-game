using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private CircleCollider2D circleCol;

    [Header("Movement Settings")]
    public float speed = 5f;

    [Header("Jump Settings")]
    [Tooltip("แรงกระโดด")]
    public float jumpForce = 8f;
    [Tooltip("จำนวนครั้งที่กระโดดได้ (1 = ปกติ, 2 = Double Jump)")]
    public int maxJumps = 1;
    [Tooltip("เสียงกระโดด")]
    public AudioClip jumpSound;

    [Header("Status")]
    public int Hp = 3;
    public int Score = 0;
    public Vector3 RespawnPostion;

    [Header("Ground Check")]
    [Tooltip("สถานะแตะพื้น")]
    public bool isGrounded = false;
    private int remainingJumps;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        circleCol = GetComponent<CircleCollider2D>();
        RespawnPostion = transform.position;
        remainingJumps = maxJumps;
    }

    void Update()
    {
        // การเคลื่อนที่แนวนอน (กลิ้งด้วย Torque)
        float h = Input.GetAxis("Horizontal");
        rb2d.AddTorque(-h * speed, ForceMode2D.Force);

        // ตรวจสอบว่าแตะพื้นอยู่หรือไม่
        if (CheckGrounded())
        {
            isGrounded = true;
            remainingJumps = maxJumps;
        }

        // ตรวจจับการกดปุ่มกระโดด (Spacebar, W, ลูกศรขึ้น, หรือปุ่ม Jump)
        bool jumpPressed = Input.GetButtonDown("Jump") || 
                           Input.GetKeyDown(KeyCode.Space) || 
                           Input.GetKeyDown(KeyCode.W) || 
                           Input.GetKeyDown(KeyCode.UpArrow);

        if (jumpPressed && (isGrounded || remainingJumps > 0))
        {
            Jump();
        }
    }

    /// <summary>
    /// ฟังก์ชันสั่งให้ผู้เล่นกระโดด
    /// </summary>
    public void Jump()
    {
        // รีเซ็ตความเร็วแนวดิ่งก่อนใส่แรงกระโดด เพื่อให้ได้ระยะกระโดดที่สม่ำเสมอ
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        isGrounded = false;
        remainingJumps--;

        // เล่นเสียงกระโดด
        if (jumpSound != null && Soundmanager.instance != null)
        {
            Soundmanager.instance.PlayVFX(jumpSound);
        }

        Debug.Log("Player Jumped! Remaining jumps: " + remainingJumps);
    }

    /// <summary>
    /// ตรวจจับพื้นใต้ลูกบอลโดยใช้ CircleCast
    /// </summary>
    private bool CheckGrounded()
    {
        if (circleCol != null)
        {
            float radius = circleCol.radius * transform.lossyScale.y * 0.9f;
            // ยิง CircleCast ลงไปข้างล่างเล็กน้อยจากจุดศูนย์กลาง
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.down, 0.15f);
            if (hit.collider != null && hit.collider.gameObject != gameObject && !hit.collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // ตรวจจับจุดสัมผัสพื้นผิว เพื่อให้กระโดดบนทางลาดหรือแท่นได้ลื่นไหล
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.4f)
            {
                isGrounded = true;
                remainingJumps = maxJumps;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = CheckGrounded();
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log("Player took " + damage + " damage. Remaining HP: " + Hp);
        Respwan();
        if (Gamemanager.instance != null)
        {
            Gamemanager.instance.UpdateHealth(Hp, 3);
        }
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
        if (Gamemanager.instance != null)
        {
            Gamemanager.instance.UpdateScore(Score);
        }
        Debug.Log("Player gained " + score + " points. Total Score: " + Score);
    }
}
