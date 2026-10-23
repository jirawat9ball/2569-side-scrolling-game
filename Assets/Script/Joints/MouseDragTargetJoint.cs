using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ควบคุม Target Joint 2D เพื่อลากวัตถุฟิสิกส์ด้วยเมาส์ (Drag and Drop Physics)
/// สามารถใส่ไว้ที่วัตถุที่มี Rigidbody2D และ TargetJoint2D ได้ทันที
/// รองรับทั้ง New Input System และ Legacy Input
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MouseDragTargetJoint : MonoBehaviour
{
    private TargetJoint2D targetJoint;
    private Camera mainCamera;
    private bool isDragging = false;

    [Header("Joint Settings")]
    [Tooltip("ความถี่ของสปริงที่ดึงเข้าหาเมาส์ ยิ่งมากยิ่งดึงเร็ว")]
    public float frequency = 5f;

    [Tooltip("แรงต้านเพื่อไม่ให้วัตถุสั่นหรือแกว่งหลุดการควบคุม")]
    [Range(0f, 1f)]
    public float dampingRatio = 0.7f;

    void Start()
    {
        mainCamera = Camera.main;
        targetJoint = GetComponent<TargetJoint2D>();
        
        // ถ้ายังไม่มี TargetJoint2D ให้เพิ่มอัตโนมัติ
        if (targetJoint == null)
        {
            targetJoint = gameObject.AddComponent<TargetJoint2D>();
        }

        targetJoint.frequency = frequency;
        targetJoint.dampingRatio = dampingRatio;
        targetJoint.enabled = false; // ปิดไว้ก่อน จะเปิดเมื่อเริ่มคลิกลาก
    }

    void Update()
    {
        Vector2 mouseWorldPos = GetMouseWorldPosition();

        // ตรวจสอบการคลิกเมาส์ซ้าย (New Input System และ Fallback สำหรับ Legacy)
        bool isPointerDown = Mouse.current != null 
            ? Mouse.current.leftButton.isPressed 
            : Input.GetMouseButton(0);

        bool wasPointerPressedThisFrame = Mouse.current != null 
            ? Mouse.current.leftButton.wasPressedThisFrame 
            : Input.GetMouseButtonDown(0);

        if (wasPointerPressedThisFrame)
        {
            // ตรวจสอบว่าคลิกโดนวัตถุนี้หรือไม่
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                targetJoint.enabled = true;
                targetJoint.target = mouseWorldPos;
            }
        }

        if (isDragging)
        {
            if (isPointerDown)
            {
                targetJoint.target = mouseWorldPos;
            }
            else
            {
                // ปล่อยเมาส์
                isDragging = false;
                targetJoint.enabled = false;
            }
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return Vector2.zero;
        }

        Vector3 screenPos = Mouse.current != null 
            ? (Vector3)Mouse.current.position.ReadValue() 
            : Input.mousePosition;

        screenPos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(screenPos);
    }
}
