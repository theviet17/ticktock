using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ServicePrefabMenu : Editor
{
    [MenuItem("Services/Services Settings")]
    static void OpenServicesPrefab()
    {
        OpenPrefab("Services");
    }

    static void OpenPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }
        else
        {
            Debug.LogError("Prefab not found: " + prefabName);
        }
    }
    
    [MenuItem("Services/Script Execution Order")]
    static void OpenScriptExecutionOrderMenu()
    {
        SettingsService.OpenProjectSettings("Project/Script Execution Order");
    }
}
#endif
