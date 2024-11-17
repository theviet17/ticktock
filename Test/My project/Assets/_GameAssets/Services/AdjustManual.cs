using System.Collections;
using System.Collections.Generic;
using AdjustSdk;
using UnityEngine;

public class AdjustManual : MonoBehaviour
{

    [SerializeField] private string token;

    [SerializeField] private AdjustEnvironment _environment = AdjustEnvironment.Production;

    [SerializeField] private bool allowSuppressLogLevel = true;
    
    [SerializeField] private AdjustLogLevel logLevel = AdjustLogLevel.Verbose;
    
    
    private void Start()
    {
        AdjustConfig config = new AdjustConfig(token, _environment, allowSuppressLogLevel);
        config.LogLevel = logLevel;
        config.IsSendingInBackgroundEnabled = true;
        Adjust.InitSdk(config);
    }
    

}
