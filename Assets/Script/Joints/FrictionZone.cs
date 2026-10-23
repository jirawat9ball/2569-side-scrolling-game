using UnityEngine;

/// <summary>
/// ควบคุม Friction Joint 2D (โซนแรงเสียดทาน / ชะลอความเร็ว)
/// เช่น พื้นโคลนหนืด, ผิวสัมผัสชะลอการหมุนหรือการเลื่อนของวัตถุ
/// </summary>
[RequireComponent(typeof(FrictionJoint2D))]
public class FrictionZone : MonoBehaviour
{
    private FrictionJoint2D frictionJoint;

    [Header("Friction Settings")]
    [Tooltip("แรงต้านทานการเคลื่อนที่เชิงเส้น (Linear Friction)")]
    public float maxForce = 50f;

    [Tooltip("แรงต้านทานการหมุน (Angular Friction)")]
    public float maxTorque = 50f;

    void Start()
    {
        frictionJoint = GetComponent<FrictionJoint2D>();
        ApplyFriction();
    }

    public void ApplyFriction()
    {
        frictionJoint.maxForce = maxForce;
        frictionJoint.maxTorque = maxTorque;
    }

    void OnValidate()
    {
        if (frictionJoint != null)
        {
            ApplyFriction();
        }
    }
}
