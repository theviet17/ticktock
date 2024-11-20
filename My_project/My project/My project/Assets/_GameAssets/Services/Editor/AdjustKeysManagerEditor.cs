using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdjustKeysManager))]
public class AdjustKeysManagerEditor : Editor
{
    private SerializedProperty isEnabledProperty;
    private SerializedProperty eventListProperty;

    // Màu sắc cho tiêu đề và nút theo phong cách Material Design
    private static readonly Color PrimaryColor = new Color(0.12f, 0.73f, 0.61f); // Teal
    private static readonly Color ButtonColor = new Color(0.0f, 0.5f, 0.8f); // Xanh dương đậm
    private static readonly Color ButtonHoverColor = new Color(0.0f, 0.6f, 0.9f); // Màu nhạt hơn khi hover

    private void OnEnable()
    {
        isEnabledProperty = serializedObject.FindProperty("isEnabled");
        eventListProperty = serializedObject.FindProperty("eventList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Nút "Open Script"
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open Script", GUILayout.Width(100)))
        {
            AssetDatabase.OpenAsset(MonoScript.FromMonoBehaviour((MonoBehaviour)target));
        }
        GUILayout.EndHorizontal();

        // Nút "Initialize Default Events" theo kiểu Material Design với hiệu ứng hover
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Tạo nút với màu nền và các trạng thái khác nhau cho cảm giác nhấn
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold,
            fixedHeight = 30,
            fixedWidth = 200,
            alignment = TextAnchor.MiddleCenter
        };

        // Kiểm tra trạng thái hover và pressed để đổi màu nút
        Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent("Initialize Default Events"), buttonStyle);
        bool isHovered = buttonRect.Contains(Event.current.mousePosition);
        bool isPressed = Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition);

        buttonStyle.normal.textColor = Color.white;
        buttonStyle.normal.background = MakeTexture(1, 1, isPressed ? ButtonColor * 0.8f : (isHovered ? ButtonHoverColor : ButtonColor));

        if (GUI.Button(buttonRect, "Initialize Default Events", buttonStyle))
        {
            ((AdjustKeysManager)target).InitializeDefaultEvents();
        }
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Phần Enable Manager
        EditorGUILayout.Space();
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = PrimaryColor } };
        EditorGUILayout.LabelField("Enable Manager", headerStyle);
        DrawBox(() =>
        {
            EditorGUILayout.PropertyField(isEnabledProperty, new GUIContent("Enable"));
        });

        // Phần Event List
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Event List", headerStyle);
        DrawBox(() =>
        {
            // Tiêu đề các cột
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Event Type", GUILayout.Width(120));
            GUILayout.Label("Event Name", GUILayout.MinWidth(100));
            GUILayout.Label("Event Token Android", GUILayout.MinWidth(100));
            GUILayout.Label("Event Token iOS", GUILayout.MinWidth(100));
            GUILayout.Label("", GUILayout.Width(20)); // Cột trống cho nút X
            EditorGUILayout.EndHorizontal();

            // Lặp qua danh sách sự kiện
            for (int i = 0; i < eventListProperty.arraySize; i++)
            {
                var entry = eventListProperty.GetArrayElementAtIndex(i);
                var eventType = entry.FindPropertyRelative("eventType");
                var eventData = entry.FindPropertyRelative("eventData");

                EditorGUILayout.BeginHorizontal();

                // Hiển thị eventType dưới dạng văn bản thay vì DropdownMenu
                EditorGUILayout.LabelField(eventType.enumDisplayNames[eventType.enumValueIndex], GUILayout.Width(120));

                // Hiển thị eventNameKey
                EditorGUILayout.PropertyField(eventData.FindPropertyRelative("eventNameKey"), GUIContent.none, GUILayout.MinWidth(100));

                // Hiển thị eventNameValueAndroid
                EditorGUILayout.PropertyField(eventData.FindPropertyRelative("eventNameValueAndroid"), GUIContent.none, GUILayout.MinWidth(100));

                // Hiển thị eventNameValueiOS
                EditorGUILayout.PropertyField(eventData.FindPropertyRelative("eventNameValueiOS"), GUIContent.none, GUILayout.MinWidth(100));

                // Nút X để xóa
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    eventListProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        });

        serializedObject.ApplyModifiedProperties();
    }

    // Hàm tạo texture màu nền cho nút
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    // Hàm tạo hộp chứa với padding và margin hợp lý
    private void DrawBox(System.Action inside)
    {
        EditorGUILayout.BeginVertical(new GUIStyle("box")
        {
            padding = new RectOffset(8, 8, 8, 8),
            margin = new RectOffset(10, 10, 10, 10)
        });
        inside.Invoke();
        EditorGUILayout.EndVertical();
    }
}
