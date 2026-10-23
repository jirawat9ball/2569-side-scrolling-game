using UnityEngine;

/// <summary>
/// ปรับแต่ง Spring Joint 2D สำหรับสร้างแท่นกระโดดเด้งดึ๋ง หรือวัตถุติดสปริง
/// </summary>
[RequireComponent(typeof(SpringJoint2D))]
public class SpringPad : MonoBehaviour
{
    private SpringJoint2D spring;

    [Header("Spring Settings")]
    [Tooltip("ความแข็ง/ความถี่ของสปริง (Frequency) ยิ่งมากยิ่งดึงกลับไวและแข็ง")]
    public float frequency = 3f;

    [Tooltip("ความหน่วง (Damping Ratio): 0 = เด้งไม่หยุด, 1 = หยุดนิ่งทันทีไม่กระเด้ง")]
    [Range(0f, 1f)]
    public float dampingRatio = 0.2f;

    void Start()
    {
        spring = GetComponent<SpringJoint2D>();
        spring.frequency = frequency;
        spring.dampingRatio = dampingRatio;
    }

    /// <summary>
    /// เพิ่มแรงกระตุ้นให้สปริงเด้งทันที (เช่น เมื่อลูกบอลตกใส่)
    /// </summary>
    public void Bounce(Vector2 impulseForce)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(impulseForce, ForceMode2D.Impulse);
        }
    }
}
