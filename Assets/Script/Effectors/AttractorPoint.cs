using UnityEngine;

/// <summary>
/// ควบคุม Point Effector 2D (หลุมดำดูดวัตถุ / แม่เหล็กผลักวัตถุ)
/// ดูดหรือผลักวัตถุออกจากจุดศูนย์กลาง
/// </summary>
[RequireComponent(typeof(PointEffector2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class AttractorPoint : MonoBehaviour
{
    private PointEffector2D pointEffector;
    private CircleCollider2D circleCollider;

    [Header("Magnet / Gravity Settings")]
    [Tooltip("ความแรงของแรง (ค่าลบ = ดูดเข้าหาศูนย์กลาง, ค่าบวก = ผลักกระจายออก)")]
    public float forceMagnitude = -20f;

    [Tooltip("รัศมีการส่งผลของแรง")]
    public float effectRadius = 5f;

    [Tooltip("รูปแบบการลดทอนของแรงตามระยะห่าง (None, Linear, InverseSquared)")]
    public EffectorSelection2D distanceScale = EffectorSelection2D.Rigidbody;

    void Start()
    {
        pointEffector = GetComponent<PointEffector2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        // สำคัญ: ต้องเปิด Is Trigger = true และ Used By Effector = true
        circleCollider.isTrigger = true;
        circleCollider.usedByEffector = true;
        circleCollider.radius = effectRadius;

        pointEffector.forceMagnitude = forceMagnitude;
    }

    void OnValidate()
    {
        if (circleCollider != null)
        {
            circleCollider.radius = effectRadius;
        }
        if (pointEffector != null)
        {
            pointEffector.forceMagnitude = forceMagnitude;
        }
    }
}
