using UnityEngine;

/// <summary>
/// ควบคุม Distance Joint 2D เช่น เชือกแขวนลูกตุ้มหนาม หรือโซ่ดึงวัตถุ
/// สามารถปรับยืด-หดความยาวเชือกได้ในเกม (Climbing/Lowering Rope)
/// </summary>
[RequireComponent(typeof(DistanceJoint2D))]
public class PendulumRope : MonoBehaviour
{
    private DistanceJoint2D distanceJoint;

    [Header("Rope Settings")]
    [Tooltip("ความยาวเชือก")]
    public float currentDistance = 3f;

    [Tooltip("ใช้เป็นเชือกหย่อนได้หรือไม่ (ถ้าติ๊กถูก จะขยับเข้ามาใกล้ได้แต่ห้ามเกินระยะ)")]
    public bool maxDistanceOnly = true;

    [Header("Rope Adjustment")]
    [Tooltip("ความเร็วในการยืด/หดเชือก")]
    public float adjustSpeed = 2f;

    void Start()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.maxDistanceOnly = maxDistanceOnly;
        distanceJoint.distance = currentDistance;
    }

    /// <summary>
    /// ปรับความยาวเชือก (ค่าบวก = ยืดยาวขึ้น, ค่าลบ = หดสั้นลง)
    /// </summary>
    public void ChangeRopeLength(float delta)
    {
        currentDistance = Mathf.Max(0.5f, currentDistance + (delta * adjustSpeed * Time.deltaTime));
        distanceJoint.distance = currentDistance;
    }
}
