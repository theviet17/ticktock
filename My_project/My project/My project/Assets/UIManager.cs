using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Android;
using TMPro;
using UnityEngine.EventSystems;
using MoreMountains.NiceVibrations;
using BaseService;
using GoogleMobileAds.Api;
using System.Xml.Linq;


[Serializable]
public enum GameStatus
{
    home,
    in_game
}
public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    instance = obj.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }
    public float LoadingTime;
    private void Awake()
    {

        Application.targetFrameRate = 120;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //SceneManager.LoadScene(0);
        //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);

        //asyncOperation.completed += (AsyncOperation op) =>
        //{
            
        //};
        ButtonListener();



    }

    public void Haptic()
    {
        if (Setting.HapticCheck())
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
            Debug.Log("Haptic");
        }
    }
    private const string CurrentLevelID = "Current_Level_Index";
    [HideInInspector] public GameStatus gameStatus = GameStatus.home;

    public GameObject canvas;
    public ButtonActive buttonActive;
    [Header("Vote")]
    public GameObject votePanel;
    public StarRating rating;
    public GameObject Tutorial;
    public float timerTutorial;


    [Header("Loading")]
    public CustomProgressBar loadingBar;
    public GameObject _loadingPanel;

    private string policyKey = "policy";

    [Header("Ads")]
    public ADS banner;
    public Interstitial interval;
    public Rewarded reward;
    public AppOpenAdTest aop;
    

    private void Start()
    {
       

        ShowOffPanelWhentStart();

        var accepted = PlayerPrefs.GetInt(policyKey, 0) == 1;

        if (accepted)
        {
            TermsOfServiceDialogClosed();
            return;
        }
            

        SimpleGDPR.ShowDialog(new TermsOfServiceDialog().
                SetPrivacyPolicyLink("https://docs.google.com/document/d/13xYHRdlLqCkubRXIxgAKnqc0Eiq9_YA4XZvkQ0IGEsI/edit?usp=sharing"),
                TermsOfServiceDialogClosed);

       

     
    }
    private void TermsOfServiceDialogClosed()
    {
        PlayerPrefs.SetInt(policyKey, 1);
        loadingBar.gameObject.SetActive(true);
        LoadingScenes();
    }
    void LoadingScenes()
    {
        float myValue = 0f;
        buttonActive.DeActive();
        DOTween.To(() => myValue, x => myValue = x, 1f, LoadingTime).SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                loadingBar.Value = myValue;
            })
            .OnComplete(() =>
            {
                aop.ShowAdIfReady(() =>
                {
                    _loadingPanel.gameObject.SetActive(false);
                    ChangeMusic(bg_music);
                    ShowOffHomePanel();
                    banner.ShowBanner();


                    DOVirtual.DelayedCall(0.7f, () =>
                    {
                        buttonActive.Active();
                    });
                });

            });
    }

    
    [Header("Home")]

    public GameObject _homePanel;

    public GameObject logo;
    public Button playButton;
    public Button settingButton;

    public GameObject _settingPanel;
    public Camera _camera;
    public CanvasGroup teeBreakPanel;
    

    void ShowOffHomePanel(bool open = true)
    {
        if (open)
        {
            banner.ReloadBanner();
            //AdsUtils.HideAndDestroyBannerNormal();
            //AdsUtils.LoadAndShowBannerNormal(true);
        }
        buttonActive.DeActive();
        DOVirtual.DelayedCall(open ? 0 : 1f, () =>
        {
            _homePanel.gameObject.SetActive(open);
        });
      

        UIAnimation.HorizontalTween(logo, 0.5f, open, _camera, UIAnimation.Direction.left);
        UIAnimation.HorizontalTween(playButton.gameObject, 0.5f, open, _camera, UIAnimation.Direction.right , 0.1f);
        UIAnimation.HorizontalTween(settingButton.gameObject, 0.5f, open, 0.2f);


    }

    public void ShowOffPanelWhentStart()
    {
        _loadingPanel.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(false);
        _homePanel.gameObject.SetActive(false);
        _settingPanel.gameObject.SetActive(false);
        _ingamePanel.gameObject.SetActive(false);
        _camPanel.gameObject.SetActive(false);
        endGameLoad.losePanel.gameObject.SetActive(false);
        endGameLoad.winPanel.gameObject.SetActive(false);
        votePanel.gameObject.SetActive(false);
        teeBreakPanel.gameObject.SetActive(false);
    }




    [Header("Ingame")]
    public TMP_Text request;
    public TMP_Text count;
    public TMP_Text Time;
    public GameObject _ingamePanel;
    public Button setting_Button_InGame;
    public Button home_Button;
    public GameObject level;
    public GameObject tempPlace;
    public Image manche;

    public void ShowRequest(bool show , string req , string cot)
    {
        request.gameObject.SetActive(show);
        request.text = req;
        count.gameObject.SetActive(show);
        count.text = cot;
    }
    public void ShowTime(bool show, int time)
    {
        Time.gameObject.SetActive(show);
        Time.text = "Time: " + time;
    }
    public void ChangeCount(string cot)
    {
        try
        {
            count.text = cot;
        }
        catch { }
       
    }
    public void ChangeTime(int time)
    {
        try
        {
            Time.text = "Time: 00:" + (time < 10 ? "0" + time : time);
        }
        catch { }
       
    }
    public static bool IsPointerOverButton(Vector3 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Kiểm tra tất cả các đối tượng dưới chuột
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = pos
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Kiểm tra nếu bất kỳ đối tượng nào là button
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.tag != "CanClick")
                {
                    return true; // Có button dưới chuột
                }
            }
        }
        return false; // Không có button dưới chuột
    }

    public NativeAdHandler ingameNative;
    void OpenIngamePanel(bool open = true)
    {
        DOVirtual.DelayedCall(open ? 0 : 1f, () =>
        {
            _ingamePanel.gameObject.SetActive(open);
        });

        UIAnimation.HorizontalTween(setting_Button_InGame.gameObject, 0.5f, open);
        UIAnimation.HorizontalTween(home_Button.gameObject, 0.5f, open);
        
        UIAnimation.VerticalTween(level, 0.5f, open, _camera, UIAnimation.Direction.top,0, -139.9f);
        UIAnimation.VerticalTween(tempPlace, 0.5f, open, _camera, UIAnimation.Direction.top, 0, -400);
        if(open)
        {
            UIAnimation.Fade(manche, 1f, !open, 1);
        }
       

        if (!isOpenCam)
        {
            UIAnimation.HorizontalTween(openCam.gameObject, 0.5f, open);
        }
        if (open)
        {
            LoadLevel();
        }
       
      
    }
 
    void ButtonListener()
    {
        playButton.onClick.AddListener(LoadScene);

        openCam.onClick.AddListener(SetUP);
        switchCam.onClick.AddListener(SwitchCamera);
        exitCam.onClick.AddListener(StopShowCamera);


        home_Button.onClick.AddListener(BackHome);
      
    }
    public bool _pause;

    void BackHome()
    {
        tickShowTutorial = false;
        buttonActive.DeActive();
        if (isOpenCam)
        {
            OpenCamPanel(false); 
        }

        OpenIngamePanel(false);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            UIAnimation.Fade(bg, 0.5f, true, 1);
            bg.gameObject.SetActive(true);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                bg_Ingame.gameObject.SetActive(false);
            });

        });

        DOVirtual.DelayedCall(0.7f, () =>
        {
           

            ChangeMusic(bg_music);
            UnLoadScene();
            ShowOffHomePanel(true);
        });
        DOVirtual.DelayedCall(1.2f, () =>
        {
            buttonActive.Active();
        });



    }


    void LoadScene()
    {
        buttonActive.DeActive();
        ShowOffHomePanel(false);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            bg_Ingame.gameObject.SetActive(true);
            UIAnimation.Fade(bg, 0.2f, false, 1);
        });

        DOVirtual.DelayedCall(0.7f, () =>
        {
            OpenIngamePanel(true);

            ChangeMusic(in_music);

            if (PlayerPrefs.GetInt(CurrentLevelID) == 12)
            {
                TurnOnMusic(false);
            }
          

            DOVirtual.DelayedCall(0.5f, () =>
            {
                buttonActive.Active();
            });

        });
       


    }
    public EndGameLoad endGameLoad;
    public ObjectPool pool;
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                var fx = pool.GetObjectFromPool();
                fx.transform.position = pos;
                fx.gameObject.SetActive(true);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    pool.ReturnObjectToPool(fx);
                });
            });
          
        }
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    NextLevel();

        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    endGameLoad.Lose();

        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    endGameLoad.Win();

        //}
        if(tickShowTutorial)
        {

            timerTutorial += UnityEngine.Time.deltaTime;

            if(PlayerPrefs.GetInt(CurrentLevelID) == 0 && !Tutorial.activeInHierarchy)
            {
                Tutorial.gameObject.SetActive(true);
            }
            else
            {
                if(timerTutorial > 7 && !Tutorial.activeInHierarchy)
                {
                    Tutorial.gameObject.SetActive(true);
                }
            }
        }
        if (Input.GetMouseButtonDown(0)  )
        {
            tickShowTutorial = false;

        }
        if(!tickShowTutorial )
        {
            timerTutorial = 0;

            if (Tutorial.activeInHierarchy)
            {
                Tutorial.gameObject.SetActive(false);
            }
        }



    }
    public void LevelUp()
    {
        int currentLevel = PlayerPrefs.GetInt(CurrentLevelID);
        currentLevel++;
        if (currentLevel > 19)
        {
            currentLevel = 0;
        }
        PlayerPrefs.SetInt(CurrentLevelID, currentLevel);

    }
    public bool tickShowTutorial;
    public int currentSceneLoad;
    //4ECAFC
    public Color colorNormal;
    public Color color13;
    public void LoadLevel()
    {
        


        interval.LoadInterstitial();
        reward.LoadRewardedAd();

        tickShowTutorial = true;
        ingameNative.ReLoadNative();

        banner.ReloadBanner();
        //AdsUtils.HideAndDestroyBannerNormal();
        //AdsUtils.LoadAndShowBannerNormal(true);

        int currentLevel = PlayerPrefs.GetInt(CurrentLevelID);

        if (PlayerPrefs.GetInt(CurrentLevelID) == 12)
        {
            bg_Ingame.color = color13;
            TurnOnMusic(false);
        }
        else
        {
            bg_Ingame.color = colorNormal;
        }

        currentSceneLoad = currentLevel + 1;
        ADS.LogStartLevel(currentSceneLoad);


        string sceneName = currentSceneLoad.ToString();
        level.GetComponentInChildren<TMP_Text>().text ="Level " + sceneName;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        _pause = false;

        //manche.GetComponent<SpriteRenderer>().DOFade(0, 1);
        //asyncOperation.completed += (AsyncOperation op) =>
        //{
        //    Debug.Log("Load complet te" + sceneName);
        //    Scene scene = SceneManager.GetSceneByName(sceneName); // Bỏ dấu ngoặc kép quanh sceneName
        //    if (scene.isLoaded)
        //    {
        //        GameObject[] rootObjects = scene.GetRootGameObjects();
        //        foreach (GameObject obj in rootObjects)
        //        {
        //            if (obj.name == "Manche")
        //            {
        //                manche = obj;   
        //                Debug.Log("Found object: " + obj.name);

        //                // Thực hiện hành động với obj
        //            }
        //        }
        //    }
        //};

    }
    private void OnApplicationQuit()
    {
        if(endGameLoad.losePanel.activeInHierarchy)
        {
            ADS.LogEndLevel(currentSceneLoad, "", false);

        }
        else if (endGameLoad.winPanel.activeInHierarchy)
        {
            ADS.LogStartLevel(currentSceneLoad);
        }
        else
        {
            ADS.LogEndLevel(currentSceneLoad, "", false);
        }
    }
    public void UnLoadScene()
    {
        
        Scene scene = SceneManager.GetSceneByName("UIDemo"); // Bỏ dấu ngoặc kép quanh sceneName
        if (scene.isLoaded)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                if (obj.name != "Main")
                {
                    Destroy(obj);
                    // Thực hiện hành động với obj
                }
            }
        }

        string sceneName = currentSceneLoad.ToString();
        // Unload Scene B
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);
    }
    void NextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt(CurrentLevelID);
        string sceneName = (currentLevel + 1).ToString();
        // Unload Scene B
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);

        currentLevel++;
        PlayerPrefs.SetInt(CurrentLevelID, currentLevel);
        string sceneName2 = (currentLevel + 1).ToString();
        // Sau khi scene B được unload, load scene C
        unloadOperation.completed += (AsyncOperation op) =>
        {
            SceneManager.LoadSceneAsync(sceneName2, LoadSceneMode.Additive);
        };
    }

    [Header("WebCam")]
    public Image bg;
    public Image bg_Ingame;
    public Image webCamBG;
    public RawImage rawImage; // Gắn RawImage vào đây
    private WebCamTexture webCamTexture;
    private WebCamDevice[] devices;    // Danh sách các camera
    private int currentCameraIndex = 0;

    public Button openCam;
    public GameObject _camPanel;
    public Button switchCam;
    public Button exitCam;

    public bool isOpenCam = false;
    public ObjectPool sourcePool;


    public void OpenCamPanel(bool open)
    {
        DOVirtual.DelayedCall(open ? 0.5f : 1f, () =>
        {
            _camPanel.gameObject.SetActive(open);
            isOpenCam = open;
        });

        DOVirtual.DelayedCall(open?0: 0.5f, () =>
        {
            UIAnimation.HorizontalTween(openCam.gameObject, 0.5f, !open);
        });
       
        DOVirtual.DelayedCall(open?0.5f : 0, () =>
        {
            UIAnimation.HorizontalTween(switchCam.gameObject, 0.5f, open);
            UIAnimation.HorizontalTween(exitCam.gameObject, 0.5f, open);
        });
        
    }
    void SetUP()
    {
        buttonActive.DeActive();
        OpenCamPanel(true);
        bg_Ingame.gameObject.SetActive(false);
        rawImage.gameObject.SetActive(true);
        webCamBG.gameObject.SetActive(true);
        DOVirtual.DelayedCall(1f, () =>
        {
            buttonActive.Active();
        });


#if UNITY_ANDROID //&& !UNITY_EDITOR

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        StartCoroutine(WaitToHavePermission());
#endif
    }
#if UNITY_ANDROID  //&& !UNITY_EDITOR
    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus && Permission.HasUserAuthorizedPermission(Permission.Camera) && webCamTexture == null)
    //    {
    //        // Nếu quay lại app và có quyền, khởi động camera
    //        StartCamera(currentCameraIndex);
    //    }
    //    else if (hasFocus && !Permission.HasUserAuthorizedPermission(Permission.Camera))
    //    {
    //        StopAllCoroutines();
    //        // Nếu không có quyền, hiển thị thông báo
    //        //ShowPermissionDeniedDialog();
    //    }
    //}
#endif
    IEnumerator WaitToHavePermission()
    {
        yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
        // Lấy danh sách các camera
        devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // Khởi tạo camera với camera đầu tiên trong danh sách
            StartCamera(currentCameraIndex);
        }
        else
        {
            Debug.LogError("No camera found!");
        }
    }
    private void StartCamera(int cameraIndex)
    {
        // Dừng camera cũ nếu đang chạy
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }

        // Khởi tạo WebCamTexture với deviceName từ danh sách devices
        webCamTexture = new WebCamTexture(devices[cameraIndex].name);

        // Gán texture cho RawImage
        rawImage.texture = webCamTexture;
        rawImage.material.mainTexture = webCamTexture;

        rawImage.rectTransform.localEulerAngles = new Vector3(0, cameraIndex == 0 ? 0 : -180, cameraIndex == 0 ? -90 : -270);

        //rawImage.uvRect = webCamTexture.videoVerticallyMirrored
        //        ? new Rect(1, 0, -1, 1)  // Lật theo chiều dọc
        //        : new Rect(0, 0, 1, 1);   // Không lật

        // Bắt đầu phát camera
        webCamTexture.Play();
        StartCoroutine(AdjustRawImageSize(webCamTexture));
    }
    IEnumerator AdjustRawImageSize(WebCamTexture webCamTexture)
    {
        // Chờ camera khởi động
        yield return new WaitUntil(() => webCamTexture.width > 100);

        // Tính tỷ lệ khung hình của camera
        float aspectRatio = (float)webCamTexture.width / webCamTexture.height;

        // Kích thước của RectTransform
        RectTransform rt = rawImage.rectTransform;

        // Điều chỉnh chiều rộng hoặc chiều cao của RawImage để phù hợp với tỷ lệ khung hình của camera
        if (aspectRatio > 1)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.y * aspectRatio, rt.sizeDelta.y);
        }
        else
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.x / aspectRatio);
        }
    }
    public void SwitchCamera()
    {
#if UNITY_ANDROID  //&& !UNITY_EDITOR

        // Đảo chỉ mục camera (0 -> 1 hoặc 1 -> 0)
        currentCameraIndex = (currentCameraIndex + 1) % devices.Length;

        // Bắt đầu lại camera với chỉ mục mới
        StartCamera(currentCameraIndex);
#endif

    }
    void StopShowCamera()
    {
        buttonActive.DeActive();
        StopCam();
        DOVirtual.DelayedCall(1f, () =>
        {
            buttonActive.Active();
        });


    }
    public void StopCam()
    {
        OpenCamPanel(false);
        rawImage.texture = null;
        rawImage.gameObject.SetActive(false);
        webCamBG.gameObject.SetActive(false);
        bg_Ingame.gameObject.SetActive(true);

#if UNITY_ANDROID //&& !UNITY_EDITOR


        // Dừng camera khi không cần nữa
        if (webCamTexture != null)
        {
            Destroy(webCamTexture);
            webCamTexture.Stop();
        }
#endif
    }
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    //[Header("Music")]
    //public AudioSource _audioSource;

    //public void PlayMusic(bool play)
    //{
    //    if (play)
    //    {
    //        _audioSource.Play();
    //    }
    //    else
    //    {
    //        _audioSource.Stop();
    //    }
    //}
    [Header("Sound")]
    public AudioClip wrong;
    public AudioClip click;
    public AudioClip lose;
    public AudioClip win;
    public AudioClip bg_music;
    public AudioClip in_music;

    public AudioSource bgMusic;

    void ChangeMusic(AudioClip clip)
    {
        bgMusic.clip = clip;
        if (Setting.MusicCheck())
        {
            bgMusic.Play();
        }
       
    }
    public void TurnOnMusic(bool on)
    {
        if(on)
        {
            bgMusic.Play();
        }
        else
        {
            bgMusic.Stop();
        }
    }
}
//public class LoadRewardedAd : MonoBehaviour
//{
//    private RewardedAd rewardedAd;

//#if UNITY_ANDROID
//    private string adUnitId = "ca-app-pub-3940256099942544/5224354917";
//#elif UNITY_IPHONE
//        private string adUnitId = "ca-app-pub-3940256099942544/1712485313";
//#else
//        private string adUnitId = "unused";
//#endif

//    void Start()
//    {
//        MobileAds.Initialize(initStatus => { });
//        LoadAd();
//    }

//    public void LoadAd()
//    {
//        // Clean up old ad
//        if (rewardedAd != null)
//        {
//            rewardedAd.Destroy();
//        }

//        rewardedAd =  RewardedAd.Load(_adUnitId, adRequest,
//          (RewardedAd ad, LoadAdError error) =>
//          {
//              // if error is not null, the load request failed.
//              if (error != null || ad == null)
//              {
//                  Debug.LogError("Rewarded ad failed to load an ad " +
//                                 "with error : " + error);
//                  return;
//              }

//              Debug.Log("Rewarded ad loaded with response : "
//                        + ad.GetResponseInfo());

//              _rewardedAd = ad;
//          });

//        rewardedAd.OnAdLoaded += HandleOnAdLoaded;
//        rewardedAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
//        rewardedAd.OnAdOpening += HandleOnAdOpening;
//        rewardedAd.OnAdClosed += HandleOnAdClosed;

//        AdRequest request = new AdRequest.Builder().Build();
//        rewardedAd.LoadAd(request);
//    }

//    public void ShowAd()
//    {
//        if (rewardedAd.IsLoaded())
//        {
//            rewardedAd.Show();
//        }
//        else
//        {
//            Debug.Log("Rewarded ad is not loaded yet.");
//        }
//    }

//    private void HandleOnAdLoaded(object sender, System.EventArgs args)
//    {
//        Debug.Log("Rewarded ad loaded successfully.");
//    }

//    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//    {
//        Debug.LogError("Failed to load rewarded ad: " + args.Message);
//    }

//    private void HandleOnAdOpening(object sender, System.EventArgs args)
//    {
//        Debug.Log("Rewarded ad is opening.");
//    }

//    private void HandleOnAdClosed(object sender, System.EventArgs args)
//    {
//        Debug.Log("Rewarded ad is closed.");
//        LoadAd(); // Reload the ad after it is closed
//    }
//}

public class LoadingScene : MonoBehaviour
{
    //public Slider loadingBar;
    //[SerializeField] private GameObject loadingRoot;
    //void Start()
    //{
    //    float value = 0;
    //    float target = 1f;
    //    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
    //    //StartCoroutine(TrackProgress(asyncOperation));
    //    DOTween.To(() => value, x => value = x, target, 6f)
    //      .SetEase(Ease.Linear)
    //      .OnUpdate(() =>
    //      {
    //          loadingBar.value = value;
    //      }).OnComplete(() =>
    //      {
    //          try
    //          {
    //              if (RemoteConfigControl.I.showAOAWhenStart.GetValue() == 1)
    //              {

    //                  AppOpenAdManager.Instance.ShowAdIfAvailable(actionDone: () =>
    //                  {
    //                      UnloadLoadingScene();
    //                  });
    //              }
    //              else
    //              {
    //                  UnloadLoadingScene();
    //              }
    //          }
    //          catch (System.Exception e)
    //          {
    //              UnloadLoadingScene();
    //          }
    //      });


    //}

    //private void UnloadLoadingScene()
    //{
    //    //SceneManager.UnloadSceneAsync(0);
    //    loadingRoot.gameObject.SetActive(false);
    //    if (PlayerPrefs.GetInt("PlayedTheDemo") == 1) // neu khong phai man demo
    //    {

    //    }

    //}

    //IEnumerator TrackProgress(AsyncOperation asyncOperation)
    //{
    //    while (!asyncOperation.isDone)
    //    {
    //        if (asyncOperation.progress > loadingBar.value)
    //        {
    //            // In ra tiến trình tải (0 đến 1)
    //            loadingBar.value = asyncOperation.progress;
    //        }

    //        Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");
    //        yield return null;
    //    }
    //    // Khi tải hoàn tất
    //    Debug.Log("Scene loaded!");
    //}

}
