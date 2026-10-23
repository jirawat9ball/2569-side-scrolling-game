using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ตรวจจับการแตกหักของ Joint2D (เช่น Fixed Joint 2D หรือ Distance Joint 2D)
/// เมื่อรับแรงกระแทกเกิน Break Force หรือ Break Torque
/// เหมาะสำหรับทำ สะพานพัง, เชือกขาด, แท่นหินหล่นเมื่อถูกชน
/// </summary>
[RequireComponent(typeof(Joint2D))]
public class BreakableJoint : MonoBehaviour
{
    [Header("Break Event Effects")]
    [Tooltip("เสียงหรือฟังก์ชันที่จะทำงานเมื่อ Joint หลุด/พัง")]
    public UnityEvent onJointBroken;

    [Tooltip("Particle Effect Prefab ที่จะสร้างขึ้นเมื่อ Joint หลุด (ถ้ามี)")]
    public GameObject breakEffectPrefab;

    // ฟังก์ชัน Event มาตรฐานของ Unity 2D Physics เมื่อ Joint รับแรงเกินค่า Break Force / Torque
    private void OnJointBreak2D(Joint2D brokenJoint)
    {
        Debug.Log($"[BreakableJoint] Joint บน '{gameObject.name}' แตกหักแล้ว!");

        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        onJointBroken?.Invoke();
    }
}
