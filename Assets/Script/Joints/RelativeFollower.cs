using UnityEngine;

/// <summary>
/// ควบคุม Relative Joint 2D (วัตถุติดตามที่รักษาระยะและมุมสัมพัทธ์)
/// เช่น วัตถุบริวารลอยตามตัวผู้เล่น หรือแท่นคู่ขนาน
/// </summary>
[RequireComponent(typeof(RelativeJoint2D))]
public class RelativeFollower : MonoBehaviour
{
    private RelativeJoint2D relativeJoint;

    [Header("Relative Settings")]
    [Tooltip("ระยะห่างสัมพัทธ์จากวัตถุหลัก (Linear Offset)")]
    public Vector2 linearOffset = new Vector2(0, 1.5f);

    [Tooltip("มุมเอียงสัมพัทธ์ (Angular Offset)")]
    public float angularOffset = 0f;

    [Tooltip("แรงดึงสูงสุดเพื่อรักษาระยะ")]
    public float maxForce = 500f;

    void Start()
    {
        relativeJoint = GetComponent<RelativeJoint2D>();
        ApplySettings();
    }

    public void ApplySettings()
    {
        relativeJoint.autoConfigureOffset = false;
        relativeJoint.linearOffset = linearOffset;
        relativeJoint.angularOffset = angularOffset;
        relativeJoint.maxForce = maxForce;
    }

    void OnValidate()
    {
        if (relativeJoint != null)
        {
            ApplySettings();
        }
    }
}
