using UnityEngine;

/// <summary>
/// ควบคุม Surface Effector 2D (สายพานลำเลียง Conveyor Belt)
/// สามารถปรับความเร็ว สลับทิศทาง และเลื่อน Texture อัตโนมัติให้ดูสมจริง
/// </summary>
[RequireComponent(typeof(SurfaceEffector2D))]
[RequireComponent(typeof(Collider2D))]
public class ConveyorBelt : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector;
    private Collider2D beltCollider;
    private Renderer beltRenderer;

    [Header("Conveyor Settings")]
    [Tooltip("ความเร็วของสายพาน (ค่าบวก = ไปข้างหน้า, ค่าลบ = ถอยหลัง)")]
    public float speed = 5f;

    [Tooltip("สลับทิศทางการวิ่งของสายพาน")]
    public bool reverseDirection = false;

    [Header("Visual Animation (Optional)")]
    [Tooltip("เลื่อนลาย Texture ตามความเร็วสายพานหรือไม่")]
    public bool animateTexture = true;
    public float textureScrollScale = 0.2f;

    void Start()
    {
        surfaceEffector = GetComponent<SurfaceEffector2D>();
        beltCollider = GetComponent<Collider2D>();
        beltRenderer = GetComponent<Renderer>();

        // สำคัญ: ต้องเปิด Used by Effector
        beltCollider.usedByEffector = true;
        UpdateEffectorSpeed();
    }

    void Update()
    {
        UpdateEffectorSpeed();

        // เลื่อน Texture ให้เห็นว่าสายพานกำลังวิ่ง
        if (animateTexture && beltRenderer != null && beltRenderer.material != null)
        {
            float currentSpeed = reverseDirection ? -speed : speed;
            float offset = Time.time * currentSpeed * textureScrollScale;
            beltRenderer.material.mainTextureOffset = new Vector2(offset, 0);
        }
    }

    public void UpdateEffectorSpeed()
    {
        surfaceEffector.speed = reverseDirection ? -Mathf.Abs(speed) : Mathf.Abs(speed);
    }

    public void ToggleDirection()
    {
        reverseDirection = !reverseDirection;
        UpdateEffectorSpeed();
    }
}
