using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdsKeysManager))]
public class AdsKeysManagerEditor : Editor
{
    // Các thuộc tính serialized
    private SerializedProperty maxSdkKey;
    private SerializedProperty keyTestMode;
    private SerializedProperty interstitialMaxId;
    private SerializedProperty rewardedMaxId;
    private SerializedProperty bannerMaxId;
    private SerializedProperty mrecMaxId;
    private SerializedProperty interstitialMaxIdIOS;
    private SerializedProperty rewardedMaxIdIOS;
    private SerializedProperty bannerMaxIdIOS;
    private SerializedProperty mrecMaxIdIOS;
    private SerializedProperty adsMode;

    private SerializedProperty bannerIdAdmob;
    private SerializedProperty bannerIdAdmobCollapsible;
    private SerializedProperty interIdAdmob;
    private SerializedProperty aoaIdAdmob;
    private SerializedProperty nativeIdAdmob;

    private SerializedProperty bannerIdAdmobIos;
    private SerializedProperty bannerIdAdmobCollapsibleIos;
    private SerializedProperty interIdAdmobIos;
    private SerializedProperty aoaIdAdmobIos;
    private SerializedProperty nativeIdAdmobIos;

    // Material Design colors
    private static readonly Color PrimaryColor = new Color(0.12f, 0.73f, 0.61f); // Teal
    private static readonly Color BackgroundColor = new Color(0.93f, 0.93f, 0.93f); // Light Background

    private void OnEnable()
    {
        // Khởi tạo các thuộc tính serialized
        maxSdkKey = serializedObject.FindProperty("maxSdkKey");
        keyTestMode = serializedObject.FindProperty("keyTestMode");
        interstitialMaxId = serializedObject.FindProperty("interstitialMaxId");
        rewardedMaxId = serializedObject.FindProperty("rewardedMaxId");
        bannerMaxId = serializedObject.FindProperty("bannerMaxId");
        mrecMaxId = serializedObject.FindProperty("mrecMaxId");

        interstitialMaxIdIOS = serializedObject.FindProperty("interstitialMaxIdIOS");
        rewardedMaxIdIOS = serializedObject.FindProperty("rewardedMaxIdIOS");
        bannerMaxIdIOS = serializedObject.FindProperty("bannerMaxIdIOS");
        mrecMaxIdIOS = serializedObject.FindProperty("mrecMaxIdIOS");

        adsMode = serializedObject.FindProperty("adsMode");

        bannerIdAdmob = serializedObject.FindProperty("BANNER_ID_ADMOB");
        bannerIdAdmobCollapsible = serializedObject.FindProperty("BANNER_ID_ADMOB_COLLAPSIBLE");
        interIdAdmob = serializedObject.FindProperty("INTER_ID_ADMOB");
        aoaIdAdmob = serializedObject.FindProperty("AOA_ID_ADMOB");
        nativeIdAdmob = serializedObject.FindProperty("NATIVE_ID_ADMOB");

        bannerIdAdmobIos = serializedObject.FindProperty("BANNER_ID_ADMOB_IOS");
        bannerIdAdmobCollapsibleIos = serializedObject.FindProperty("BANNER_ID_ADMOB_COLLAPSIBLE_IOS");
        interIdAdmobIos = serializedObject.FindProperty("INTER_ID_ADMOB_IOS");
        aoaIdAdmobIos = serializedObject.FindProperty("AOA_ID_ADMOB_IOS");
        nativeIdAdmobIos = serializedObject.FindProperty("NATIVE_ID_ADMOB_IOS");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Nút Open Script ở góc trên cùng bên phải
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open Script", GUILayout.Width(100)))
        {
            AssetDatabase.OpenAsset(MonoScript.FromMonoBehaviour((MonoBehaviour)target));
        }
        GUILayout.EndHorizontal();

        // Ad Mode - Hiển thị trên cùng
        EditorGUILayout.Space();
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = PrimaryColor } };
        EditorGUILayout.LabelField("Ad Mode", headerStyle);
        DrawBox(() =>
        {
            EditorGUILayout.PropertyField(adsMode, new GUIContent("Ad Mode"));
        });

        // Max Ids - Hiển thị sau Ad Mode
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Applovin Max SDK IDs", headerStyle);
        DrawBox(() =>
        {
            EditorGUILayout.PropertyField(maxSdkKey, new GUIContent("Max SDK Key"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Max Android IDs", headerStyle);
            EditorGUILayout.PropertyField(interstitialMaxId, new GUIContent("Interstitial Max ID"));
            EditorGUILayout.PropertyField(rewardedMaxId, new GUIContent("Rewarded Max ID"));
            EditorGUILayout.PropertyField(bannerMaxId, new GUIContent("Banner Max ID"));
            EditorGUILayout.PropertyField(mrecMaxId, new GUIContent("Mrec Max ID"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Max iOS IDs", headerStyle);
            EditorGUILayout.PropertyField(interstitialMaxIdIOS, new GUIContent("Interstitial Max ID (iOS)"));
            EditorGUILayout.PropertyField(rewardedMaxIdIOS, new GUIContent("Rewarded Max ID (iOS)"));
            EditorGUILayout.PropertyField(bannerMaxIdIOS, new GUIContent("Banner Max ID (iOS)"));
            EditorGUILayout.PropertyField(mrecMaxIdIOS, new GUIContent("Mrec Max ID (iOS)"));
        });

        // AdMob Ids - Hiển thị sau Max Ids
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Google AdMob IDs", headerStyle);
        DrawBox(() =>
        {
            EditorGUILayout.PropertyField(keyTestMode, new GUIContent("Key Test Mode"));
            EditorGUILayout.PropertyField(bannerIdAdmob, new GUIContent("Banner ID AdMob"));
            EditorGUILayout.PropertyField(bannerIdAdmobCollapsible, new GUIContent("Banner ID AdMob Collapsible"));
            EditorGUILayout.PropertyField(interIdAdmob, new GUIContent("Interstitial ID AdMob"));
            EditorGUILayout.PropertyField(aoaIdAdmob, new GUIContent("AOA ID AdMob"));
            EditorGUILayout.PropertyField(nativeIdAdmob, new GUIContent("Native ID AdMob"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Google AdMob IDs (iOS)", headerStyle);
            EditorGUILayout.PropertyField(bannerIdAdmobIos, new GUIContent("Banner ID AdMob (iOS)"));
            EditorGUILayout.PropertyField(bannerIdAdmobCollapsibleIos, new GUIContent("Banner ID AdMob Collapsible (iOS)"));
            EditorGUILayout.PropertyField(interIdAdmobIos, new GUIContent("Interstitial ID AdMob (iOS)"));
            EditorGUILayout.PropertyField(aoaIdAdmobIos, new GUIContent("AOA ID AdMob (iOS)"));
            EditorGUILayout.PropertyField(nativeIdAdmobIos, new GUIContent("Native ID AdMob (iOS)"));
        });

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBox(System.Action inside)
    {
        EditorGUILayout.BeginVertical(new GUIStyle("box") { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(10, 10, 10, 10) });
        inside.Invoke();
        EditorGUILayout.EndVertical();
    }
}
