using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseService
{
    [System.Serializable]
    public class PlatformRemoteStringConfigKey {

        public string key;

        public string value;


        public PlatformRemoteStringConfigKey(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string GetKey()
        {
            return key;
        }

        public string GetValue()
        {
            return value;
        }

        public void SetValue(string value)
        {
            this.value = value;
        }
        
    }
    
    [System.Serializable]
    public class PlatformRemoteConfigKey {

        public string key;

        public int value;


        public PlatformRemoteConfigKey(string key, int value)
        {
            this.key = key;
            this.value = value;
        }

        public string GetKey()
        {
            return key;
        }

        public int GetValue()
        {
            return value;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }
        
    }
}


