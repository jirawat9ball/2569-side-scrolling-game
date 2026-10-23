using UnityEngine;

/// <summary>
/// ควบคุมยานพาหนะ 2D (รถยนต์ / รถเข็น) โดยใช้ Wheel Joint 2D
/// หมุนล้อหน้าและล้อหลังพร้อมกันตามการกดปุ่ม Horizontal (A/D หรือซ้าย/ขวา)
/// </summary>
public class Vehicle2DController : MonoBehaviour
{
    [Header("Wheels Setup")]
    [Tooltip("Wheel Joint ของล้อหลัง (ขับเคลื่อนหลัก)")]
    public WheelJoint2D backWheelJoint;

    [Tooltip("Wheel Joint ของล้อหน้า (ถ้าต้องการระบบขับเคลื่อน 4 ล้อ)")]
    public WheelJoint2D frontWheelJoint;

    [Header("Vehicle Settings")]
    [Tooltip("ความเร็วในการหมุนของล้อ (Motor Speed)")]
    public float motorSpeed = 800f;

    [Tooltip("แรงบิดสูงสุดของมอเตอร์")]
    public float maxMotorTorque = 1000f;

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            ApplyMotor(moveInput);
        }
        else
        {
            StopMotor();
        }
    }

    private void ApplyMotor(float input)
    {
        // ค่า motorSpeed ติดลบ = วิ่งไปข้างหน้าตามทิศทางหมุนตามเข็มนาฬิกา
        float targetSpeed = -input * motorSpeed;

        if (backWheelJoint != null)
        {
            backWheelJoint.useMotor = true;
            JointMotor2D motor = backWheelJoint.motor;
            motor.motorSpeed = targetSpeed;
            motor.maxMotorTorque = maxMotorTorque;
            backWheelJoint.motor = motor;
        }

        if (frontWheelJoint != null)
        {
            frontWheelJoint.useMotor = true;
            JointMotor2D motor = frontWheelJoint.motor;
            motor.motorSpeed = targetSpeed;
            motor.maxMotorTorque = maxMotorTorque;
            frontWheelJoint.motor = motor;
        }
    }

    private void StopMotor()
    {
        if (backWheelJoint != null) backWheelJoint.useMotor = false;
        if (frontWheelJoint != null) frontWheelJoint.useMotor = false;
    }
}
