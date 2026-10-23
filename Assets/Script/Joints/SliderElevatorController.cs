using UnityEngine;

/// <summary>
/// ควบคุม Slider Joint 2D (แท่นเลื่อน / ลิฟต์ / ลูกสูบ)
/// ตัวอย่างการใช้งาน: ลิฟต์เลื่อนขึ้น-ลง หรือแท่นเลื่อนซ้าย-ขวาตามแนวแกน Slider
/// </summary>
[RequireComponent(typeof(SliderJoint2D))]
public class SliderElevatorController : MonoBehaviour
{
    private SliderJoint2D slider;

    [Header("Elevator Settings")]
    [Tooltip("ความเร็วในการเลื่อน")]
    public float moveSpeed = 3f;

    [Tooltip("สลับทิศทางอัตโนมัติเมื่อเลื่อนถึงขอบ Limit")]
    public bool autoReverse = true;

    [Tooltip("เวลาหยุดรอก่อนเปลี่ยนทิศทาง (วินาที)")]
    public float waitTimeAtLimits = 0.5f;

    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        slider = GetComponent<SliderJoint2D>();
        slider.useMotor = true;
        SetMotorSpeed(moveSpeed);
    }

    void Update()
    {
        if (!autoReverse || !slider.useLimits) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        float currentTranslation = slider.jointTranslation;
        JointTranslationLimits2D limits = slider.limits;

        // ถึงขอบล่าง/จุดเริ่มต้น
        if (currentTranslation <= limits.min + 0.05f && slider.motor.motorSpeed < 0)
        {
            TriggerReverse(Mathf.Abs(moveSpeed));
        }
        // ถึงขอบบน/จุดสิ้นสุด
        else if (currentTranslation >= limits.max - 0.05f && slider.motor.motorSpeed > 0)
        {
            TriggerReverse(-Mathf.Abs(moveSpeed));
        }
    }

    private void TriggerReverse(float newSpeed)
    {
        if (waitTimeAtLimits > 0f)
        {
            isWaiting = true;
            waitTimer = waitTimeAtLimits;
            SetMotorSpeed(0f);
            Invoke(nameof(ResumeAfterWait), waitTimeAtLimits);
        }
        else
        {
            SetMotorSpeed(newSpeed);
        }
    }

    private void ResumeAfterWait()
    {
        float currentTranslation = slider.jointTranslation;
        JointTranslationLimits2D limits = slider.limits;
        if (currentTranslation <= limits.min + 0.1f)
            SetMotorSpeed(Mathf.Abs(moveSpeed));
        else
            SetMotorSpeed(-Mathf.Abs(moveSpeed));
    }

    public void SetMotorSpeed(float speed)
    {
        JointMotor2D motor = slider.motor;
        motor.motorSpeed = speed;
        slider.motor = motor; // กำหนดค่ากลับเข้า SliderJoint2D
    }
}
