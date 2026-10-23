#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class Physics2DDemoBuilder : EditorWindow
{
    [MenuItem("Tools/Generate Physics 2D Demo Scene")]
    public static void GenerateScene()
    {
        // 1. สร้าง Scene ใหม่
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // โหลด sprite พื้นฐาน
        Sprite ballSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/rollingBall.png");
        // ค้นหา square sprite ถ้ามี หรือสร้าง Texture สีขาว
        Sprite squareSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        // โหลดฟอนต์ภาษาไทย (Thai Leelawadee SDF)
        TMP_FontAsset thaiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Thai_Leelawadee_SDF.asset");
        if (thaiFont == null)
        {
            thaiFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Leelawadee_SDF.asset");
        }
        AudioClip jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Leohpaz/RPG_Essentials_Free/12_Player_Movement_SFX/30_Jump_03.wav");

        // 2. แสงสว่าง (2D Global Light)
        GameObject lightObj = new GameObject("Global Light 2D");
        var light2D = lightObj.AddComponent<Light2D>();
        light2D.lightType = Light2D.LightType.Global;
        light2D.intensity = 1.0f;

        // 3. Main Camera พร้อม CameraFollow
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6.5f;
        cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camObj.AddComponent<AudioListener>();
        camObj.AddComponent<UniversalAdditionalCameraData>();
        CameraFollow camFollow = camObj.AddComponent<CameraFollow>();
        camObj.transform.position = new Vector3(0, 2.5f, -10);

        // 4. Player (ลูกบอลกลิ้ง)
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0, 1.5f, 0);
        var playerSr = player.AddComponent<SpriteRenderer>();
        if (ballSprite != null) playerSr.sprite = ballSprite;
        playerSr.color = new Color(0.2f, 0.8f, 1f); // สีฟ้าสดใส
        var playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var playerCol = player.AddComponent<CircleCollider2D>();
        playerCol.radius = 0.5f;
        var playerScript = player.AddComponent<Player>();
        playerScript.speed = 10f;
        playerScript.jumpForce = 9f;
        playerScript.maxJumps = 1;
        if (jumpClip != null) playerScript.jumpSound = jumpClip;
        camFollow.target = player.transform;

        // 5. Root Object เก็บด่าน
        GameObject demoRoot = new GameObject("DemoEnvironment");

        // ฟังก์ชันช่วยสร้างแท่นพื้น
        System.Func<string, Vector3, Vector2, Color, GameObject> CreatePlatform = (name, pos, size, col) => {
            GameObject p = new GameObject(name);
            p.transform.parent = demoRoot.transform;
            p.transform.position = pos;
            var sr = p.AddComponent<SpriteRenderer>();
            if (squareSprite != null) sr.sprite = squareSprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = size;
            sr.color = col;
            var box = p.AddComponent<BoxCollider2D>();
            box.size = size;
            return p;
        };

        // ฟังก์ชันช่วยสร้างป้ายข้อความ
        System.Action<string, Vector3> CreateLabel = (text, pos) => {
            GameObject textObj = new GameObject("Label_" + text.Split('\n')[0]);
            textObj.transform.parent = demoRoot.transform;
            textObj.transform.position = pos;
            var tmp = textObj.AddComponent<TextMeshPro>();
            if (thaiFont != null) tmp.font = thaiFont;
            tmp.text = text;
            tmp.fontSize = 4f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.yellow;
            tmp.rectTransform.sizeDelta = new Vector2(14, 3);
        };

        // --- พื้นหลัก (Main Ground ขยายยาวรองรับทุกสถานี) ---
        CreatePlatform("Ground_Start", new Vector3(10, -0.5f, 0), new Vector2(25, 1), new Color(0.3f, 0.35f, 0.4f));
        CreatePlatform("Ground_Mid1", new Vector3(45, -0.5f, 0), new Vector2(30, 1), new Color(0.3f, 0.35f, 0.4f));
        CreatePlatform("Ground_Mid2", new Vector3(80, -0.5f, 0), new Vector2(30, 1), new Color(0.3f, 0.35f, 0.4f));
        CreatePlatform("Ground_Extended1", new Vector3(115, -0.5f, 0), new Vector2(40, 1), new Color(0.3f, 0.35f, 0.4f));
        CreatePlatform("Ground_Extended2", new Vector3(155, -0.5f, 0), new Vector2(40, 1), new Color(0.3f, 0.35f, 0.4f));
        CreatePlatform("Ground_Final", new Vector3(190, -0.5f, 0), new Vector2(35, 1), new Color(0.3f, 0.35f, 0.4f));

        // กำแพงกันตกซ้ายสุด และขวาสุด
        CreatePlatform("Wall_Left", new Vector3(-3, 6, 0), new Vector2(1, 14), new Color(0.2f, 0.25f, 0.3f));
        CreatePlatform("Wall_Right", new Vector3(208, 6, 0), new Vector2(1, 14), new Color(0.2f, 0.25f, 0.3f));

        CreateLabel("Physics 2D Demo Course\n[A / D] กลิ้ง  |  [Space / W] กระโดด", new Vector3(0, 3.5f, 0));

        // ==========================================
        // 1. One-Way Platform (PlatformEffector2D)
        // ==========================================
        Vector3 p1Pos = new Vector3(7, 2.5f, 0);
        GameObject p1 = CreatePlatform("OneWayPlatform", p1Pos, new Vector2(5, 0.5f), new Color(0.2f, 0.7f, 0.4f));
        var p1Effector = p1.AddComponent<PlatformEffector2D>();
        p1Effector.useOneWay = true;
        p1.GetComponent<BoxCollider2D>().usedByEffector = true;
        p1.AddComponent<OneWayPlatform>();
        CreateLabel("1. One-Way Platform\n(โดดทะลุขึ้นได้ / กด S เพื่อทิ้งตัวลง)", new Vector3(7, 4.5f, 0));

        // ==========================================
        // 2. Conveyor Belt (SurfaceEffector2D)
        // ==========================================
        Vector3 p2Pos = new Vector3(16, 0.25f, 0);
        GameObject p2 = CreatePlatform("ConveyorBelt", p2Pos, new Vector2(6, 0.6f), new Color(0.9f, 0.6f, 0.1f));
        var p2Effector = p2.AddComponent<SurfaceEffector2D>();
        p2Effector.speed = 8f;
        p2.GetComponent<BoxCollider2D>().usedByEffector = true;
        p2.AddComponent<ConveyorBelt>();
        CreateLabel("2. Conveyor Belt\n(สายพานลำเลียงส่งลูกบอลไปข้างหน้า)", new Vector3(16, 2.5f, 0));

        // ==========================================
        // 3. Wind Tunnel (AreaEffector2D)
        // ==========================================
        Vector3 p3Pos = new Vector3(25, 3f, 0);
        GameObject p3 = new GameObject("WindTunnel_Area");
        p3.transform.parent = demoRoot.transform;
        p3.transform.position = p3Pos;
        var p3Sr = p3.AddComponent<SpriteRenderer>();
        if (squareSprite != null) p3Sr.sprite = squareSprite;
        p3Sr.drawMode = SpriteDrawMode.Sliced;
        p3Sr.size = new Vector2(3, 6);
        p3Sr.color = new Color(0.3f, 0.8f, 1f, 0.25f);
        var p3Col = p3.AddComponent<BoxCollider2D>();
        p3Col.size = new Vector2(3, 6);
        p3Col.isTrigger = true;
        p3Col.usedByEffector = true;
        var p3Effector = p3.AddComponent<AreaEffector2D>();
        p3Effector.forceMagnitude = 22f;
        p3Effector.forceAngle = 90f;
        p3.AddComponent<WindZoneArea>();
        CreateLabel("3. Wind Tunnel\n(อุโมงค์ลมเป่าลูกบอลลอยขึ้น)", new Vector3(25, 6.8f, 0));

        CreatePlatform("HighPlatform_1", new Vector3(31, 5f, 0), new Vector2(6, 0.5f), new Color(0.4f, 0.4f, 0.5f));

        // ==========================================
        // 4. Spring Pad (SpringJoint2D)
        // ==========================================
        Vector3 p4AnchorPos = new Vector3(31, 3.5f, 0);
        GameObject p4Anchor = CreatePlatform("SpringAnchor", p4AnchorPos, new Vector2(1, 0.5f), Color.gray);
        var p4AnchorRb = p4Anchor.AddComponent<Rigidbody2D>();
        p4AnchorRb.bodyType = RigidbodyType2D.Static;

        GameObject p4Pad = CreatePlatform("SpringPad", new Vector3(31, 4.3f, 0), new Vector2(3, 0.4f), new Color(1f, 0.3f, 0.6f));
        var p4Rb = p4Pad.AddComponent<Rigidbody2D>();
        p4Rb.mass = 2f;
        p4Rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        var springJoint = p4Pad.AddComponent<SpringJoint2D>();
        springJoint.connectedBody = p4AnchorRb;
        springJoint.autoConfigureDistance = false;
        springJoint.distance = 0.8f;
        springJoint.frequency = 4f;
        springJoint.dampingRatio = 0.2f;
        p4Pad.AddComponent<SpringPad>();
        CreateLabel("4. Spring Trampoline\n(แท่นสปริงเด้งดึ๋ง)", new Vector3(31, 6.2f, 0));

        // ==========================================
        // 5. Elevator (SliderJoint2D)
        // ==========================================
        Vector3 p5Pos = new Vector3(38, 1.5f, 0);
        GameObject p5Elevator = CreatePlatform("ElevatorPlatform", p5Pos, new Vector2(3.5f, 0.5f), new Color(0.8f, 0.3f, 0.2f));
        var p5Rb = p5Elevator.AddComponent<Rigidbody2D>();
        p5Rb.mass = 5f;
        p5Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        var sliderJoint = p5Elevator.AddComponent<SliderJoint2D>();
        sliderJoint.angle = 90f;
        sliderJoint.useLimits = true;
        JointTranslationLimits2D limits = new JointTranslationLimits2D();
        limits.min = 0f;
        limits.max = 5f;
        sliderJoint.limits = limits;
        sliderJoint.useMotor = true;
        JointMotor2D sliderMotor = new JointMotor2D();
        sliderMotor.motorSpeed = 3f;
        sliderMotor.maxMotorTorque = 1000f;
        sliderJoint.motor = sliderMotor;
        p5Elevator.AddComponent<SliderElevatorController>();
        CreateLabel("5. Elevator\n(Slider Joint เลื่อนขึ้น-ลง)", new Vector3(38, 7.5f, 0));

        // ==========================================
        // 6. Water Zone (BuoyancyEffector2D)
        // ==========================================
        Vector3 p6Pos = new Vector3(47, 0.5f, 0);
        GameObject p6Water = new GameObject("WaterPool");
        p6Water.transform.parent = demoRoot.transform;
        p6Water.transform.position = p6Pos;
        var p6Sr = p6Water.AddComponent<SpriteRenderer>();
        if (squareSprite != null) p6Sr.sprite = squareSprite;
        p6Sr.drawMode = SpriteDrawMode.Sliced;
        p6Sr.size = new Vector2(8, 3);
        p6Sr.color = new Color(0.1f, 0.5f, 1f, 0.4f);
        var p6Col = p6Water.AddComponent<BoxCollider2D>();
        p6Col.size = new Vector2(8, 3);
        p6Col.isTrigger = true;
        p6Col.usedByEffector = true;
        var buoyancy = p6Water.AddComponent<BuoyancyEffector2D>();
        buoyancy.density = 2.5f;
        buoyancy.linearDamping = 4f;
        buoyancy.angularDamping = 3f;
        buoyancy.surfaceLevel = 0.5f;
        p6Water.AddComponent<WaterZone>();
        CreateLabel("6. Water Pool\n(Buoyancy Effector จำลองน้ำลอยตัว)", new Vector3(47, 3.2f, 0));

        // ==========================================
        // 7. Gravity Attractor / Black Hole (PointEffector2D)
        // ==========================================
        Vector3 p7Pos = new Vector3(57, 4f, 0);
        GameObject p7Hole = new GameObject("BlackHole");
        p7Hole.transform.parent = demoRoot.transform;
        p7Hole.transform.position = p7Pos;
        var p7Sr = p7Hole.AddComponent<SpriteRenderer>();
        if (ballSprite != null) p7Sr.sprite = ballSprite;
        p7Sr.color = new Color(0.5f, 0f, 0.8f);
        p7Hole.transform.localScale = Vector3.one * 1.5f;
        var p7Col = p7Hole.AddComponent<CircleCollider2D>();
        p7Col.isTrigger = true;
        p7Col.usedByEffector = true;
        p7Col.radius = 3.5f;
        var p7PointEffector = p7Hole.AddComponent<PointEffector2D>();
        p7PointEffector.forceMagnitude = -25f;
        p7PointEffector.distanceScale = 0.5f;
        p7Hole.AddComponent<AttractorPoint>();
        CreateLabel("7. Gravity Well\n(Point Effector ดูดลูกบอลเข้าหาศูนย์กลาง)", new Vector3(57, 6.5f, 0));

        for (int i = 0; i < 3; i++)
        {
            GameObject miniBall = new GameObject("OrbitBall_" + i);
            miniBall.transform.parent = demoRoot.transform;
            miniBall.transform.position = p7Pos + new Vector3(1.8f + i * 0.6f, (i - 1) * 0.8f, 0);
            var mbSr = miniBall.AddComponent<SpriteRenderer>();
            if (ballSprite != null) mbSr.sprite = ballSprite;
            mbSr.color = Color.magenta;
            miniBall.transform.localScale = Vector3.one * 0.5f;
            var mbRb = miniBall.AddComponent<Rigidbody2D>();
            mbRb.gravityScale = 0.2f;
            miniBall.AddComponent<CircleCollider2D>();
        }

        // ==========================================
        // 8. Swinging Blade Trap (HingeJoint2D)
        // ==========================================
        Vector3 p8AnchorPos = new Vector3(66, 6f, 0);
        GameObject p8Anchor = CreatePlatform("HingeAnchor", p8AnchorPos, new Vector2(0.6f, 0.6f), Color.gray);
        var p8AnchorRb = p8Anchor.AddComponent<Rigidbody2D>();
        p8AnchorRb.bodyType = RigidbodyType2D.Static;

        GameObject p8Blade = CreatePlatform("SwingingBlade", new Vector3(66, 3.5f, 0), new Vector2(0.5f, 3.5f), new Color(0.9f, 0.2f, 0.2f));
        var p8BladeRb = p8Blade.AddComponent<Rigidbody2D>();
        p8BladeRb.mass = 3f;
        var hinge = p8Blade.AddComponent<HingeJoint2D>();
        hinge.connectedBody = p8AnchorRb;
        hinge.autoConfigureConnectedAnchor = false;
        hinge.anchor = new Vector2(0, 1.75f);
        hinge.connectedAnchor = Vector2.zero;
        hinge.useLimits = true;
        JointAngleLimits2D hLimits = new JointAngleLimits2D();
        hLimits.min = -65f;
        hLimits.max = 65f;
        hinge.limits = hLimits;
        hinge.useMotor = true;
        JointMotor2D hMotor = new JointMotor2D();
        hMotor.motorSpeed = 90f;
        hMotor.maxMotorTorque = 500f;
        hinge.motor = hMotor;
        p8Blade.AddComponent<HingeMotorController>();
        CreateLabel("8. Swinging Blade\n(Hinge Joint หมุนสลับทิศทาง)", new Vector3(66, 7.5f, 0));

        // ==========================================
        // 9. Breakable Bridge (FixedJoint2D)
        // ==========================================
        Vector3 p9Start = new Vector3(72, 1.5f, 0);
        CreateLabel("9. Breakable Bridge\n(Fixed Joint พังเมื่อรับแรงกด)", new Vector3(76, 3.8f, 0));
        Rigidbody2D lastRb = null;
        for (int i = 0; i < 4; i++)
        {
            GameObject plank = CreatePlatform("BridgePlank_" + i, p9Start + new Vector3(i * 1.8f, 0, 0), new Vector2(1.6f, 0.3f), new Color(0.6f, 0.4f, 0.2f));
            var plankRb = plank.AddComponent<Rigidbody2D>();
            plankRb.mass = 1f;

            var fixedJoint = plank.AddComponent<FixedJoint2D>();
            fixedJoint.breakForce = 250f;
            fixedJoint.breakTorque = 250f;
            if (lastRb != null)
            {
                fixedJoint.connectedBody = lastRb;
            }
            plank.AddComponent<BreakableJoint>();
            lastRb = plankRb;
        }

        // ==========================================
        // 10. Mouse Draggable Box (TargetJoint2D)
        // ==========================================
        Vector3 p10Pos = new Vector3(84, 2f, 0);
        GameObject p10Box = CreatePlatform("DraggableBox", p10Pos, new Vector2(1.5f, 1.5f), new Color(0.2f, 0.8f, 0.6f));
        var p10Rb = p10Box.AddComponent<Rigidbody2D>();
        p10Rb.mass = 1.5f;
        p10Box.AddComponent<MouseDragTargetJoint>();
        CreateLabel("10. Mouse Draggable Box\n(คลิกเมาส์ซ้ายค้างเพื่อหยิบลาก)", new Vector3(84, 4.5f, 0));

        // =========================================================================
        // 11. Wheel Joint 2D (รถ 2D ฟิสิกส์ พร้อมโช้คอัพ Suspension)
        // =========================================================================
        Vector3 carPos = new Vector3(98, 1.2f, 0);
        GameObject car = CreatePlatform("PhysicsCar_Body", carPos, new Vector2(3.2f, 0.8f), new Color(0.9f, 0.3f, 0.3f));
        var carRb = car.AddComponent<Rigidbody2D>();
        carRb.mass = 4f;

        // ล้อหลัง
        GameObject backWheel = new GameObject("BackWheel");
        backWheel.transform.parent = demoRoot.transform;
        backWheel.transform.position = carPos + new Vector3(-1.1f, -0.4f, 0);
        var bwSr = backWheel.AddComponent<SpriteRenderer>();
        if (ballSprite != null) bwSr.sprite = ballSprite;
        bwSr.color = Color.darkGray;
        backWheel.transform.localScale = Vector3.one * 0.8f;
        var bwRb = backWheel.AddComponent<Rigidbody2D>();
        bwRb.mass = 1f;
        backWheel.AddComponent<CircleCollider2D>();
        var bwJoint = backWheel.AddComponent<WheelJoint2D>();
        bwJoint.connectedBody = carRb;
        bwJoint.anchor = Vector2.zero;
        bwJoint.connectedAnchor = new Vector2(-1.1f, -0.4f);
        bwJoint.useMotor = true;
        JointSuspension2D bwSusp = new JointSuspension2D { frequency = 5f, dampingRatio = 0.7f, angle = 90f };
        bwJoint.suspension = bwSusp;

        // ล้อหน้า
        GameObject frontWheel = new GameObject("FrontWheel");
        frontWheel.transform.parent = demoRoot.transform;
        frontWheel.transform.position = carPos + new Vector3(1.1f, -0.4f, 0);
        var fwSr = frontWheel.AddComponent<SpriteRenderer>();
        if (ballSprite != null) fwSr.sprite = ballSprite;
        fwSr.color = Color.darkGray;
        frontWheel.transform.localScale = Vector3.one * 0.8f;
        var fwRb = frontWheel.AddComponent<Rigidbody2D>();
        fwRb.mass = 1f;
        frontWheel.AddComponent<CircleCollider2D>();
        var fwJoint = frontWheel.AddComponent<WheelJoint2D>();
        fwJoint.connectedBody = carRb;
        fwJoint.anchor = Vector2.zero;
        fwJoint.connectedAnchor = new Vector2(1.1f, -0.4f);
        fwJoint.useMotor = true;
        JointSuspension2D fwSusp = new JointSuspension2D { frequency = 5f, dampingRatio = 0.7f, angle = 90f };
        fwJoint.suspension = fwSusp;

        var vCtrl = car.AddComponent<Vehicle2DController>();
        vCtrl.backWheelJoint = bwJoint;
        vCtrl.frontWheelJoint = fwJoint;
        vCtrl.motorSpeed = 700f;
        CreateLabel("11. Wheel Joint 2D\n(รถ 2D พร้อมโช้คอัพ ขับเคลื่อนได้)", new Vector3(98, 4.2f, 0));

        // =========================================================================
        // 12. Distance Joint 2D (ลูกตุ้มแขวนเชือกคงระยะห่าง ไม่ยืด)
        // =========================================================================
        Vector3 p12AnchorPos = new Vector3(112, 6.5f, 0);
        GameObject p12Anchor = CreatePlatform("DistanceAnchor", p12AnchorPos, new Vector2(0.6f, 0.6f), Color.gray);
        var p12AnchorRb = p12Anchor.AddComponent<Rigidbody2D>();
        p12AnchorRb.bodyType = RigidbodyType2D.Static;

        Vector3 p12BallPos = new Vector3(114.5f, 4f, 0); // วางเยื้องให้แกว่ง
        GameObject p12Ball = new GameObject("DistanceWreckingBall");
        p12Ball.transform.parent = demoRoot.transform;
        p12Ball.transform.position = p12BallPos;
        var p12Sr = p12Ball.AddComponent<SpriteRenderer>();
        if (ballSprite != null) p12Sr.sprite = ballSprite;
        p12Sr.color = new Color(0.8f, 0.5f, 0.2f);
        p12Ball.transform.localScale = Vector3.one * 1.4f;
        var p12Rb = p12Ball.AddComponent<Rigidbody2D>();
        p12Rb.mass = 5f;
        p12Ball.AddComponent<CircleCollider2D>();
        var distJoint = p12Ball.AddComponent<DistanceJoint2D>();
        distJoint.connectedBody = p12AnchorRb;
        distJoint.autoConfigureDistance = false;
        distJoint.distance = 3.8f;
        distJoint.maxDistanceOnly = false;
        p12Ball.AddComponent<PendulumRope>();
        CreateLabel("12. Distance Joint 2D\n(ลูกตุ้มแขวนเชือก/โซ่ คงระยะห่าง)", new Vector3(112, 7.8f, 0));

        // =========================================================================
        // 13. Friction Joint 2D & Capsule Collider 2D (โซนหนืดชะลอความเร็ว)
        // =========================================================================
        Vector3 p13Pos = new Vector3(127, 1.5f, 0);
        GameObject p13Crate = CreatePlatform("FrictionCrate", p13Pos, new Vector2(1.8f, 1.2f), new Color(0.5f, 0.4f, 0.3f));
        var p13Rb = p13Crate.AddComponent<Rigidbody2D>();
        p13Rb.mass = 2f;
        var fJoint = p13Crate.AddComponent<FrictionJoint2D>();
        fJoint.maxForce = 30f;
        fJoint.maxTorque = 30f;
        p13Crate.AddComponent<FrictionZone>();

        // เพิ่มถังแคปซูล Capsule Collider 2D ในโซนนี้
        GameObject capsuleBarrel = new GameObject("CapsuleBarrel");
        capsuleBarrel.transform.parent = demoRoot.transform;
        capsuleBarrel.transform.position = p13Pos + new Vector3(2.5f, 0, 0);
        var capSr = capsuleBarrel.AddComponent<SpriteRenderer>();
        if (squareSprite != null) capSr.sprite = squareSprite;
        capSr.drawMode = SpriteDrawMode.Sliced;
        capSr.size = new Vector2(1f, 1.8f);
        capSr.color = new Color(0.7f, 0.6f, 0.2f);
        var capRb = capsuleBarrel.AddComponent<Rigidbody2D>();
        capRb.mass = 1.5f;
        var capCol = capsuleBarrel.AddComponent<CapsuleCollider2D>();
        capCol.size = new Vector2(1f, 1.8f);
        CreateLabel("13. Friction Joint 2D\n(โซนหนืดต้านทานการเลื่อน + Capsule)", new Vector3(128, 4.2f, 0));

        // =========================================================================
        // 14. Constant Force 2D (ไอพ่นจรวดส่งแรงขับเคลื่อนคงที่)
        // =========================================================================
        Vector3 p14Pos = new Vector3(142, 1.5f, 0);
        GameObject p14Rocket = CreatePlatform("RocketPad", p14Pos, new Vector2(2.5f, 0.6f), new Color(0.2f, 0.6f, 0.9f));
        var p14Rb = p14Rocket.AddComponent<Rigidbody2D>();
        p14Rb.mass = 3f;
        p14Rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        var cForce = p14Rocket.AddComponent<ConstantForce2D>();
        cForce.force = new Vector2(0, 35f); // แรงพยุงไอพ่นแนวตั้ง
        p14Rocket.AddComponent<ConstantBooster2D>();
        CreateLabel("14. Constant Force 2D\n(ไอพ่นจรวดส่งแรงพยุงต่อเนื่อง)", new Vector3(142, 5.5f, 0));

        // =========================================================================
        // 15. Relative Joint 2D (วัตถุติดตามรักษาระยะและมุมสัมพัทธ์)
        // =========================================================================
        Vector3 p15BasePos = new Vector3(156, 1.5f, 0);
        GameObject p15Base = CreatePlatform("RelativeBasePlatform", p15BasePos, new Vector2(3f, 0.6f), new Color(0.7f, 0.3f, 0.7f));
        var p15BaseRb = p15Base.AddComponent<Rigidbody2D>();
        p15BaseRb.mass = 4f;

        Vector3 p15SatPos = p15BasePos + new Vector3(0, 2.5f, 0);
        GameObject p15Satellite = CreatePlatform("SatelliteDrone", p15SatPos, new Vector2(1.5f, 0.4f), new Color(0.9f, 0.4f, 0.9f));
        var p15SatRb = p15Satellite.AddComponent<Rigidbody2D>();
        p15SatRb.mass = 1f;
        var relJoint = p15Satellite.AddComponent<RelativeJoint2D>();
        relJoint.connectedBody = p15BaseRb;
        relJoint.autoConfigureOffset = false;
        relJoint.linearOffset = new Vector2(0, 2.5f);
        relJoint.maxForce = 400f;
        relJoint.maxTorque = 400f;
        p15Satellite.AddComponent<RelativeFollower>();
        CreateLabel("15. Relative Joint 2D\n(วัตถุบริวารรักษาระยะสัมพัทธ์ตามตัวหลัก)", new Vector3(156, 5.8f, 0));

        // =========================================================================
        // 16. Edge Collider 2D & Composite Collider 2D (ทางลาดโค้ง & รวมขอบการชน)
        // =========================================================================
        // ก. ทางลาดโค้ง Half-Pipe ด้วย Edge Collider 2D
        Vector3 rampPos = new Vector3(172, 0, 0);
        GameObject edgeRamp = new GameObject("CurvedRamp_EdgeCollider");
        edgeRamp.transform.parent = demoRoot.transform;
        edgeRamp.transform.position = rampPos;
        var edgeCol = edgeRamp.AddComponent<EdgeCollider2D>();
        Vector2[] rampPoints = new Vector2[] {
            new Vector2(0, 5f),
            new Vector2(1.5f, 3.2f),
            new Vector2(3.5f, 1.5f),
            new Vector2(6f, 0.3f),
            new Vector2(9f, 0.3f),
            new Vector2(11.5f, 1.5f),
            new Vector2(13.5f, 3.2f),
            new Vector2(15f, 5f)
        };
        edgeCol.points = rampPoints;
        edgeCol.edgeRadius = 0.1f;

        // วาดเส้นโค้งให้มองเห็นด้วย LineRenderer
        var line = edgeRamp.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = rampPoints.Length;
        for (int i = 0; i < rampPoints.Length; i++) {
            line.SetPosition(i, new Vector3(rampPoints[i].x, rampPoints[i].y, 0));
        }
        line.startWidth = 0.25f;
        line.endWidth = 0.25f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(0.2f, 0.9f, 0.5f);
        line.endColor = new Color(0.2f, 0.9f, 0.5f);

        // ข. แท่นรวม Composite Collider 2D
        GameObject compPlatform = new GameObject("CompositePlatform_Root");
        compPlatform.transform.parent = demoRoot.transform;
        compPlatform.transform.position = new Vector3(193, 2.5f, 0);
        var compRb = compPlatform.AddComponent<Rigidbody2D>();
        compRb.bodyType = RigidbodyType2D.Static;
        var compCol = compPlatform.AddComponent<CompositeCollider2D>();
        compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;

        for (int i = 0; i < 3; i++) {
            GameObject cBlock = CreatePlatform("CompBlock_" + i, new Vector3(191 + i * 2.2f, 2.5f + (i % 2) * 0.4f, 0), new Vector2(2.6f, 0.8f), new Color(0.3f, 0.7f, 0.6f));
            cBlock.transform.parent = compPlatform.transform;
            cBlock.GetComponent<BoxCollider2D>().compositeOperation = Collider2D.CompositeOperation.Merge;
        }

        CreateLabel("16. Edge & Composite Collider\n(ทางลาด Half-Pipe โค้ง & รวมชิ้น Collider)", new Vector3(184, 6.8f, 0));

        // บันทึก Scene
        string scenePath = "Assets/Scenes/Physics2D_Demo.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("<color=green><b>Physics 2D Complete Demo Scene Generated at: " + scenePath + "</b></color>");
    }
}
#endif
