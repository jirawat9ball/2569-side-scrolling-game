using UnityEngine;

/// <summary>
/// ควบคุม Constant Force 2D เช่น บูสเตอร์ไอพ่นจรวด หรือแรงผลักคงที่อย่างต่อเนื่อง
/// </summary>
[RequireComponent(typeof(ConstantForce2D))]
public class ConstantBooster2D : MonoBehaviour
{
    private ConstantForce2D constantForce2D;

    [Header("Force Settings")]
    [Tooltip("แรงคงที่ที่จะใส่ให้วัตถุตลอดเวลา")]
    public Vector2 continuousForce = new Vector2(0, 9.8f);

    [Tooltip("แรงบิดหมุนคงที่")]
    public float continuousTorque = 0f;

    void Start()
    {
        constantForce2D = GetComponent<ConstantForce2D>();
        ApplyForce();
    }

    public void ApplyForce()
    {
        constantForce2D.force = continuousForce;
        constantForce2D.torque = continuousTorque;
    }

    public void SetBoosterActive(bool isActive)
    {
        constantForce2D.enabled = isActive;
    }
}
