using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Crashlytics;

namespace BaseService
{
    public class FirebaseManager : MonoBehaviour
    {
        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    
        public bool isInitialized = false;
        public float callRate = 1f;
        public float maxTime = 60;
        float time = 0;
        public static FirebaseManager instance;
        void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
        }
    
        IEnumerator Start()
        {
            while (!isInitialized && (time < maxTime))
            {
                time += 1;
                try
                {
                    Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                    {
                        dependencyStatus = task.Result;
                        if (dependencyStatus == Firebase.DependencyStatus.Available)
                        {
                            Debug.Log("UnityFirebase is ready!");
                            isInitialized = true;
                            Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
    
                            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                            RemoteConfigControl.I.InitializeFirebase();
                        }
                        else
                        {
                            Debug.Log("UnityFirebase Could not resolve all Firebase dependencies: " + dependencyStatus);
                        }
                    });
                }
                catch (System.Exception)
                {
                    throw;
                }
                yield return new WaitForSeconds(callRate);
            }
        }
    }
}

