using System.Collections;
using UnityEngine;

/// <summary>
/// ควบคุม Area Effector 2D (อุโมงค์ลม / กระแสลมผลักวัตถุ)
/// สามารถตั้งค่าให้ลมพัดตลอดเวลา หรือพัดเป็นจังหวะ เปิด-ปิด (Pulsing Wind)
/// </summary>
[RequireComponent(typeof(AreaEffector2D))]
[RequireComponent(typeof(Collider2D))]
public class WindZoneArea : MonoBehaviour
{
    private AreaEffector2D areaEffector;
    private Collider2D areaCollider;

    [Header("Wind Settings")]
    [Tooltip("ความแรงของลม")]
    public float forceMagnitude = 15f;

    [Tooltip("มุมทิศทางของลม (90 = พัดขึ้น, 0 = พัดไปทางขวา, 180 = พัดไปทางซ้าย)")]
    public float forceAngle = 90f;

    [Header("Pulsing Wind / Intermittent (ทางเลือกลมเป็นจังหวะ)")]
    [Tooltip("เปิดระบบลมพัดเป็นจังหวะ")]
    public bool isPulsing = false;
    [Tooltip("เวลาที่ลมพัด (วินาที)")]
    public float blowDuration = 2f;
    [Tooltip("เวลาที่ลมดับ (วินาที)")]
    public float restDuration = 1.5f;

    [Header("Visual Effects (Optional)")]
    [Tooltip("Particle หรือเอฟเฟกต์ลมที่จะเปิด/ปิดตามจังหวะลม")]
    public GameObject windEffectParticles;

    void Start()
    {
        areaEffector = GetComponent<AreaEffector2D>();
        areaCollider = GetComponent<Collider2D>();

        // สำคัญ: Area Effector ต้องใช้ Is Trigger = true และ Used By Effector = true
        areaCollider.isTrigger = true;
        areaCollider.usedByEffector = true;

        areaEffector.forceMagnitude = forceMagnitude;
        areaEffector.forceAngle = forceAngle;

        if (isPulsing)
        {
            StartCoroutine(PulseWindRoutine());
        }
    }

    private IEnumerator PulseWindRoutine()
    {
        while (true)
        {
            SetWindActive(true);
            yield return new WaitForSeconds(blowDuration);

            SetWindActive(false);
            yield return new WaitForSeconds(restDuration);
        }
    }

    public void SetWindActive(bool active)
    {
        areaEffector.enabled = active;
        if (windEffectParticles != null)
        {
            windEffectParticles.SetActive(active);
        }
    }
}
