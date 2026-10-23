using System.Collections;
using UnityEngine;

/// <summary>
/// ควบคุม Platform Effector 2D (แท่นเหยียบแบบทะลุขึ้นไปได้ One-Way Platform)
/// มีระบบกดปุ่มลง (S หรือ Down Arrow) เพื่อทิ้งตัวทะลุลงด้านล่าง
/// </summary>
[RequireComponent(typeof(PlatformEffector2D))]
[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private Collider2D platformCollider;

    [Header("Drop Down Settings")]
    [Tooltip("ระยะเวลาที่ปิดการชนเพื่อให้ตัวละครหล่นลงมา (วินาที)")]
    public float waitTimeToRestore = 0.4f;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        platformCollider = GetComponent<Collider2D>();

        // ข้อสำคัญ: Collider2D ต้องติ๊ก Used by Effector จึงจะทำงาน
        platformCollider.usedByEffector = true;
        effector.useOneWay = true;
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่มลง (S หรือ ลูกศรลง) เพื่อทิ้งตัวลงจากแท่น
        float v = Input.GetAxisRaw("Vertical");
        if (v < -0.5f)
        {
            StartCoroutine(DropDownRoutine());
        }
    }

    private IEnumerator DropDownRoutine()
    {
        // สลับองศาของ OneWay เพื่อให้ทะลุลงได้
        effector.rotationalOffset = 180f;
        yield return new WaitForSeconds(waitTimeToRestore);
        effector.rotationalOffset = 0f;
    }
}
