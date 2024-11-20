using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class AdjustPlatform : MonoBehaviour
{
    private const int ANDROID = 0;
    private const int IOS = 1;
    
    [SerializeField] private GameObject[] adjusts;
    
    #if UNITY_EDITOR
    private void Update()
    {
        if (adjusts.Length < 0)
        {
            return;
        }
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        if (target == BuildTarget.iOS)
        {
            adjusts[ANDROID].SetActive(false);
            adjusts[IOS].SetActive(true);
        }
        else if (target == BuildTarget.Android)
        {
            adjusts[ANDROID].SetActive(true);
            adjusts[IOS].SetActive(false);
        }
        else
        {
            adjusts[ANDROID].SetActive(false);
            adjusts[IOS].SetActive(false);
        }
    }
    #endif
}
