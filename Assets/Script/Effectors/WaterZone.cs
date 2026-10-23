using UnityEngine;

/// <summary>
/// ควบคุม Buoyancy Effector 2D (แอ่งน้ำ / โซนของเหลวจำลองการลอยน้ำ)
/// ทำให้วัตถุที่มี Rigidbody2D ลอยและมีแรงหนืดเมื่อตกลงไปในน้ำ
/// </summary>
[RequireComponent(typeof(BuoyancyEffector2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class WaterZone : MonoBehaviour
{
    private BuoyancyEffector2D buoyancy;
    private BoxCollider2D boxCollider;

    [Header("Water Physics Settings")]
    [Tooltip("ความหนาแน่นของน้ำ ยิ่งมากยิ่งพยุงวัตถุให้ลอยสูงขึ้น")]
    public float density = 2f;

    [Tooltip("แรงต้านทานการเคลื่อนที่ในน้ำ (หนืด)")]
    public float linearDamping = 3f;

    [Tooltip("แรงต้านทานการหมุนตัวในน้ำ")]
    public float angularDamping = 3f;

    [Tooltip("ระดับผิวน้ำสัมพัทธ์ใน Collider (0 = กลาง, 0.5 = ขอบบนสุด)")]
    public float surfaceLevel = 0.5f;

    [Header("Flow / Water Current (กระแสน้ำไหล)")]
    [Tooltip("ความแรงของกระแสน้ำไหล")]
    public float flowMagnitude = 0f;
    [Tooltip("ทิศทางของกระแสน้ำ (องศา) เช่น 0 = ไหลไปทางขวา")]
    public float flowAngle = 0f;

    void Start()
    {
        buoyancy = GetComponent<BuoyancyEffector2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // สำคัญ: ต้องเปิด Is Trigger = true และ Used By Effector = true
        boxCollider.isTrigger = true;
        boxCollider.usedByEffector = true;

        ApplySettings();
    }

    public void ApplySettings()
    {
        buoyancy.density = density;
        buoyancy.linearDamping = linearDamping;
        buoyancy.angularDamping = angularDamping;
        buoyancy.surfaceLevel = surfaceLevel;
        buoyancy.flowMagnitude = flowMagnitude;
        buoyancy.flowAngle = flowAngle;
    }

    void OnValidate()
    {
        if (buoyancy != null)
        {
            ApplySettings();
        }
    }
}
