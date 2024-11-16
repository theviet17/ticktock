using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseMe
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private bool m_IsNeedDestroy = false;

        private static T m_Instance;
        public static T I
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = GameObject.FindObjectOfType<T>();
                }
                return m_Instance;
            }
        }

        protected virtual void Awake()
        {

            if (m_Instance == null || m_IsNeedDestroy)
            {
                m_Instance = this as T;
                if (!m_IsNeedDestroy && transform.parent == null)
                {
                    //If I am the first instance, make me the Singleton
                    DontDestroyOnLoad(this);
                }
                OnRegisterInstance();
            }
            else
            {
                //If a Singleton already exists and you find
                //another reference in scene, destroy it!
                if (this != m_Instance)
                    Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_Instance == this || m_IsNeedDestroy)
                m_Instance = null;
        }

        protected virtual void OnRegisterInstance()
        {

        }

        public void DoWhenFrameEnd(System.Action action)
        {
            StartCoroutine(IDoWhenFrameEnd(action));
        }

        private IEnumerator IDoWhenFrameEnd(System.Action action)
        {
            yield return new WaitForEndOfFrame();
            action.Invoke();
        }

        public void WaitUtil(Func<bool> condition, System.Action onWaitDone)
        {
            StartCoroutine(IWaitUtil(condition, onWaitDone));
        }

        public IEnumerator IWaitUtil(Func<bool> condition, System.Action onWaitDone)
        {
            yield return new WaitUntil(condition);
            onWaitDone.Invoke();
        }

        public string GetDefaultTag()
        {
            return $"{gameObject.name}_{this.GetType().Name}_";
        }

        public void DebugLog(string message)
        {
            #if DEBUG
            Debug.Log($"{GetDefaultTag()}__ {message}");
            #endif
        }

        public static bool InstanceIsValid()
        {
            return I != null;
        }
    }
}

