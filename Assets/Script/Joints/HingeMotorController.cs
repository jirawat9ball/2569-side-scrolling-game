using UnityEngine;

/// <summary>
/// ควบคุม Hinge Joint 2D (บานพับ / มอเตอร์หมุน)
/// ตัวอย่างการใช้งาน: ใบพัดหมุน, ประตูเปิด-ปิด, คานแกว่งสลับทิศทาง
/// </summary>
[RequireComponent(typeof(HingeJoint2D))]
public class HingeMotorController : MonoBehaviour
{
    private HingeJoint2D hinge;

    [Header("Motor Settings")]
    [Tooltip("ความเร็วในการหมุนของมอเตอร์")]
    public float targetSpeed = 100f;

    [Tooltip("กลับทิศทางมอเตอร์อัตโนมัติเมื่อหมุนชนขอบ Limit")]
    public bool autoReverseOnLimits = true;

    void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        hinge.useMotor = true;
        SetMotorSpeed(targetSpeed);
    }

    void Update()
    {
        // หากเปิดใช้ Limits และต้องการให้หมุนสลับไปมาเมื่อชนขอบ
        if (autoReverseOnLimits && hinge.useLimits)
        {
            float currentAngle = hinge.jointAngle;
            JointAngleLimits2D limits = hinge.limits;

            // เมื่อมุมเข้าใกล้ min limit และกำลังหมุนไปทางลบ ให้กลับทิศทาง
            if (currentAngle <= limits.min + 2f && hinge.motor.motorSpeed < 0)
            {
                SetMotorSpeed(Mathf.Abs(targetSpeed));
            }
            // เมื่อมุมเข้าใกล้ max limit และกำลังหมุนไปทางบวก ให้กลับทิศทาง
            else if (currentAngle >= limits.max - 2f && hinge.motor.motorSpeed > 0)
            {
                SetMotorSpeed(-Mathf.Abs(targetSpeed));
            }
        }
    }

    public void SetMotorSpeed(float speed)
    {
        JointMotor2D motor = hinge.motor;
        motor.motorSpeed = speed;
        hinge.motor = motor; // JointMotor2D เป็น struct ต้องกำหนดค่ากลับคืนเข้า joint
    }

    public void ToggleMotor(bool enable)
    {
        hinge.useMotor = enable;
    }
}
