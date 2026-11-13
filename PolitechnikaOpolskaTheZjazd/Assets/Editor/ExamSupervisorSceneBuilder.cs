using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheZjazd.ExamSupervisor.EditorTools
{
    /// <summary>Generates the Student prefab and ExamSupervisorTestScene (classroom with walls) from the art in Sprites/Sala.</summary>
    public static class ExamSupervisorSceneBuilder
    {
        const string PrefabPath = "Assets/Prefabs/Student.prefab";
        const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
        const string ScenePath = "Assets/Scenes/ExamSupervisorTestScene.unity";
        // Pixel art (16 px = 1 unit, same as the player). Import settings live in the .meta files.
        const string ArtDir = "Assets/Sprites/Sala/";
        const string FloorPath = ArtDir + "podloga_linoleum.png";
        const string StudentLookPathFormat = ArtDir + "student_lawka_{0}.png";
        const string MarkerPath = ArtDir + "wykrzyknik.png";
        const int StudentLookCount = 6;

        [MenuItem("Zjazd/Build Exam Supervisor Test Scene")]
        public static void Build()
        {
            RulerSpriteGenerator.EnsureSprites();
            EnsureStudentPrefab();
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Load after NewScene: creating a scene unloads previously loaded assets.
            var studentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath).GetComponent<Student>();

            var camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camGo.tag = "MainCamera";
            var cam = camGo.GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 9.2f;
            cam.backgroundColor = new Color(0.18f, 0.18f, 0.2f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            // Frames the whole classroom including its walls.
            camGo.transform.position = new Vector3(-5f, 2.5f, -10f);

            // Linoleum floor, tiled instead of stretched so the pattern keeps its size.
            var floor = Sprite(AssetDatabase.LoadAssetAtPath<Sprite>(FloorPath), "ClassroomFloor", new Vector3(-5f, 1.25f, 0f), Color.white, Vector3.one, -10);
            var floorSr = floor.GetComponent<SpriteRenderer>();
            floorSr.drawMode = SpriteDrawMode.Tiled;
            // Exactly the room interior: x -12..2, y -6..8.5.
            floorSr.size = new Vector2(14f, 14.5f);
            // Far below every Y-sorted sprite (those use roughly +-1000).
            floorSr.sortingOrder = -10000;

            var classroomGo = new GameObject("Classroom");
            // Raised a bit so the front row's chairs end just above the see-through bottom wall.
            classroomGo.transform.position = new Vector3(-5f, 0.8f, 0f);
            var layout = classroomGo.AddComponent<ClassroomLayout>();
            Set(layout, "studentPrefab", studentPrefab);

            BuildRoom();

            // The exam starts with the player standing behind the professor's desk (left of the whiteboard).
            var entry = new GameObject("ClassroomEntry").transform;
            entry.position = new Vector3(-9f, 7.9f, 0f);
            var exit = new GameObject("ClassroomExit").transform;
            // The room is closed: after the exam the player stays inside, next to the professor's desk.
            exit.position = new Vector3(-6.2f, 7.9f, 0f);

            // Real player made by the team (movement, animations, E key), not a placeholder.
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.transform.position = exit.position;
            var playerCtl = player.GetComponent<PlayerControls>();
            // Feet are 1 unit below the sprite centre; lets desks hide the player only when the player stands behind them.
            SetFloat(player.AddComponent<YSorter>(), "footOffset", -1f);

            // Ruler: a child of the player (its origin is the hand) plus a separate arc sprite.
            var rulerGo = new GameObject("Ruler", typeof(SpriteRenderer));
            rulerGo.transform.SetParent(player.transform, false);
            var rulerSr = rulerGo.GetComponent<SpriteRenderer>();
            rulerSr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(RulerSpriteGenerator.RulerPath);
            rulerSr.sortingOrder = 2;
            var swishGo = new GameObject("Swish", typeof(SpriteRenderer));
            swishGo.transform.SetParent(player.transform, false);
            var swishSr = swishGo.GetComponent<SpriteRenderer>();
            swishSr.sortingOrder = 1;
            var swishFrames = new Sprite[RulerSpriteGenerator.SwishFrameCount];
            for (int i = 0; i < swishFrames.Length; i++)
                swishFrames[i] = AssetDatabase.LoadAssetAtPath<Sprite>(RulerSpriteGenerator.SwishPath(i));
            swishSr.sprite = swishFrames[0];
            var rulerSwing = rulerGo.AddComponent<RulerSwing>();
            Set(rulerSwing, "ruler", rulerSr);
            Set(rulerSwing, "swish", swishSr);
            SetObjectArray(rulerSwing, "swishFrames", swishFrames);

            var uiGo = new GameObject("ExamUI");
            var ui = uiGo.AddComponent<ExamUI>();

            var questGo = new GameObject("ExamSupervisorQuest");
            var wallet = questGo.AddComponent<MockEctsWallet>();
            var quest = questGo.AddComponent<ExamSupervisorQuest>();
            Set(quest, "classroom", layout);
            Set(quest, "ui", ui);
            Set(quest, "player", playerCtl);
            Set(quest, "ruler", rulerSwing);
            Set(quest, "walletBehaviour", wallet);
            Set(quest, "classroomEntry", entry);
            Set(quest, "classroomExit", exit);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("ExamSupervisorTestScene built at " + ScenePath);
        }

        // Walls, window sunlight and the professor's desk around the 4x4 desk grid (classroom centre x = -5).
        static void BuildRoom()
        {
            var room = new GameObject("ClassroomRoom").transform;
            // Back walls: drawn behind every Y-sorted object.
            RoomPiece(room, "WallTop", "sciana_gorna", new Vector2(-5f, 9.75f), -9000, 1f, new Vector2(14f, 2.5f), Vector2.zero);
            RoomPiece(room, "WallLeft", "sciana_lewa", new Vector2(-12.625f, 2.5f), -9000, 1f, new Vector2(1.25f, 17f), Vector2.zero);
            // Front walls close the room; half transparent and always in front so the player stays visible behind them.
            // Right wall runs along the floor edge down to the bottom wall; its collider reaches the corner (y -6..11).
            RoomPiece(room, "WallRight", "sciana_prawa", new Vector2(2.625f, 3.5625f), 3000, 0.5f, new Vector2(1.25f, 17f), new Vector2(0f, -1.0625f));
            // Bottom wall stands on the floor edge (y = -6) and, seen from above at an angle, rises into the room up to y -3.875.
            RoomPiece(room, "WallBottom", "sciana_dolna", new Vector2(-5f, -4.9375f), 3000, 0.5f, new Vector2(16.5f, 0.5f), new Vector2(0f, -0.8125f));
            // Sun patches on the floor next to the three left-wall windows.
            float[] windowY = { 7.8f, 3f, -1.8f };
            for (int i = 0; i < windowY.Length; i++)
                RoomPiece(room, "WindowSunlight" + (i + 1), "swiatlo_okna", new Vector2(-11.1f, windowY[i] - 0.5f), -9990, 1f, null, Vector2.zero);
            var desk = RoomPiece(room, "ProfessorDesk", "biurko_profesora", new Vector2(-9f, 6.3f), 0, 1f, new Vector2(4f, 1.2f), new Vector2(0f, -0.15f));
            // Y-sorted like the student desks, so the player standing behind it is partly hidden by it.
            SetFloat(desk.AddComponent<YSorter>(), "footOffset", -0.75f);
        }

        static GameObject RoomPiece(Transform parent, string name, string art, Vector2 pos, int order, float alpha, Vector2? colliderSize, Vector2 colliderOffset)
        {
            var go = Sprite(AssetDatabase.LoadAssetAtPath<Sprite>(ArtDir + art + ".png"), name, pos, new Color(1f, 1f, 1f, alpha), Vector3.one, order);
            go.transform.SetParent(parent, true);
            if (colliderSize.HasValue)
            {
                var box = go.AddComponent<BoxCollider2D>();
                box.size = colliderSize.Value;
                box.offset = colliderOffset;
            }
            return go;
        }

        static GameObject Sprite(Sprite sprite, string name, Vector3 pos, Color color, Vector3 scale, int order)
        {
            var go = new GameObject(name, typeof(SpriteRenderer));
            go.transform.position = pos;
            go.transform.localScale = scale;
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = order;
            return go;
        }

        static void EnsureStudentPrefab()
        {
            var looks = new Sprite[StudentLookCount];
            for (int i = 0; i < looks.Length; i++)
                looks[i] = AssetDatabase.LoadAssetAtPath<Sprite>(string.Format(StudentLookPathFormat, i + 1));

            // Sprite is 1.25x1.75 units, seen from behind (students face the whiteboard):
            // desk on top, student's back and chair below it. Pivot = desk centre.
            var root = new GameObject("Student", typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Student));
            var sr = root.GetComponent<SpriteRenderer>();
            sr.sprite = looks[0];
            sr.sortingOrder = 1;
            // Collider covers the desk and the seated student, so the player can hit them from the aisles.
            var box = root.GetComponent<BoxCollider2D>();
            box.size = new Vector2(1.25f, 1.6f);
            box.offset = new Vector2(0f, -0.5f);

            var marker = new GameObject("CheatingMarker", typeof(SpriteRenderer));
            marker.transform.SetParent(root.transform, false);
            marker.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            var msr = marker.GetComponent<SpriteRenderer>();
            msr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(MarkerPath);
            msr.sortingOrder = 2;
            marker.SetActive(false);

            // Chair legs end 1.35 units below the pivot.
            SetFloat(root.AddComponent<YSorter>(), "footOffset", -1.35f);

            var student = root.GetComponent<Student>();
            Set(student, "cheatingMarker", marker);
            SetObjectArray(student, "looks", looks);

            Directory.CreateDirectory(Path.GetDirectoryName(PrefabPath));
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
        }

        static void SetObjectArray(Object target, string field, Object[] values)
        {
            var so = new SerializedObject(target);
            var array = so.FindProperty(field);
            array.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetFloat(Object target, string field, float value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).floatValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Set(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            if (value == null) throw new System.InvalidOperationException("Null reference for " + field);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
