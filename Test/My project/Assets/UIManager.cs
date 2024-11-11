﻿using System.Collections;
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
    [Header("Loading")]
    public CustomProgressBar loadingBar;
    public GameObject _loadingPanel;
    private void Start()
    {
        ShowOffPanelWhentStart();
        LoadingScenes();

        
    }
    void LoadingScenes()
    {
        float myValue = 0f;
        buttonActive.DeActive();
        DOTween.To(() => myValue, x => myValue = x, 1f, 6f).SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                loadingBar.Value = myValue;
            })
            .OnComplete(() =>
            {
                _loadingPanel.gameObject.SetActive(false);
                ChangeMusic(bg_music);
                ShowOffHomePanel();
                

                DOVirtual.DelayedCall(0.7f, () =>
                {
                    buttonActive.Active();
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

    

    void ShowOffHomePanel(bool open = true)
    {
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
        _homePanel.gameObject.SetActive(false);
        _settingPanel.gameObject.SetActive(false);
        _ingamePanel.gameObject.SetActive(false);
        _camPanel.gameObject.SetActive(false);
        endGameLoad.losePanel.gameObject.SetActive(false);
        endGameLoad.winPanel.gameObject.SetActive(false);
    }




    [Header("Ingame")]
    public TMP_Text request;
    public TMP_Text count;
    public TMP_Text Time;
    public GameObject bg_Ingame;
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


    void OpenIngamePanel(bool open = true)
    {
        DOVirtual.DelayedCall(open ? 0 : 1f, () =>
        {
            _ingamePanel.gameObject.SetActive(open);
        });

        UIAnimation.HorizontalTween(setting_Button_InGame.gameObject, 0.5f, open);
        UIAnimation.HorizontalTween(home_Button.gameObject, 0.5f, open);
        UIAnimation.HorizontalTween(openCam.gameObject, 0.5f, open);
        UIAnimation.VerticalTween(level, 0.5f, open, _camera, UIAnimation.Direction.top,0, -139.9f);
        UIAnimation.VerticalTween(tempPlace, 0.5f, open, _camera, UIAnimation.Direction.top, 0, -307.6f);
        UIAnimation.Fade(manche, 1f, !open,1);


        if (open)
        {
            LoadLevel();
        }
       
      
    }
    void ButtonListener()
    {
        playButton.onClick.AddListener(LoadSceme);

        openCam.onClick.AddListener(SetUP);
        switchCam.onClick.AddListener(SwitchCamera);
        exitCam.onClick.AddListener(StopShowCamera);


        home_Button.onClick.AddListener(BackHome);
    }
    void BackHome()
    {
        buttonActive.DeActive();
        if (isOpenCam)
        {
            OpenCamPanel(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                OpenIngamePanel(false);
                DOVirtual.DelayedCall(0.7f, () =>
                {
                    ChangeMusic(bg_music);
                    UnLoadScene();
                    ShowOffHomePanel(true);
                });
            });
            DOVirtual.DelayedCall(2.2f, () =>
            {
                buttonActive.Active();
            });
        }
        else
        {
            OpenIngamePanel(false);
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
       
    }

    void LoadSceme()
    {
        buttonActive.DeActive();
        ShowOffHomePanel(false);
        DOVirtual.DelayedCall(0.7f, () =>
        {
            OpenIngamePanel(true);
            ChangeMusic(in_music);

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
    }
    public void LevelUp()
    {
        int currentLevel = PlayerPrefs.GetInt(CurrentLevelID);
        currentLevel++;
        if (currentLevel > 9)
        {
            currentLevel = 0;
        }
        PlayerPrefs.SetInt(CurrentLevelID, currentLevel);
       
    }
    public void LoadLevel()
    {
        bg_Ingame.gameObject.SetActive(true);
        int currentLevel = PlayerPrefs.GetInt(CurrentLevelID);
        string sceneName = (currentLevel + 1).ToString();
        level.GetComponentInChildren<TMP_Text>().text ="Level " + sceneName;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

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

        int currentLevel = PlayerPrefs.GetInt(CurrentLevelID);
        string sceneName = (currentLevel + 1).ToString();
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
        bg.gameObject.SetActive(false);
        bg_Ingame.gameObject.SetActive(false);
        rawImage.gameObject.SetActive(true);
        DOVirtual.DelayedCall(1f, () =>
        {
            buttonActive.Active();
        });


#if UNITY_ANDROID && !UNITY_EDITOR

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        StartCoroutine(WaitToHavePermission());
#endif
    }
#if UNITY_ANDROID  && !UNITY_EDITOR
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && Permission.HasUserAuthorizedPermission(Permission.Camera) && webCamTexture == null)
        {
            // Nếu quay lại app và có quyền, khởi động camera
            StartCamera(currentCameraIndex);
        }
        else if (hasFocus && !Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            StopAllCoroutines();
            // Nếu không có quyền, hiển thị thông báo
            //ShowPermissionDeniedDialog();
        }
    }
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

        rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, cameraIndex == 0 ? -90 : -270);
        //rawImage.uvRect = webCamTexture.videoVerticallyMirrored
        //        ? new Rect(1, 0, -1, 1)  // Lật theo chiều dọc
        //        : new Rect(0, 0, 1, 1);   // Không lật

        // Bắt đầu phát camera
        webCamTexture.Play();
    }
    public void SwitchCamera()
    {
#if UNITY_ANDROID  && !UNITY_EDITOR

        // Đảo chỉ mục camera (0 -> 1 hoặc 1 -> 0)
        currentCameraIndex = (currentCameraIndex + 1) % devices.Length;

        // Bắt đầu lại camera với chỉ mục mới
        StartCamera(currentCameraIndex);
#endif

    }
    public void StopShowCamera()
    {
        buttonActive.DeActive();
        OpenCamPanel(false);
        rawImage.texture = null;
        rawImage.gameObject.SetActive(false);
        bg.gameObject.SetActive(true);
        bg_Ingame.gameObject.SetActive(true);
        DOVirtual.DelayedCall(1f, () =>
        {
            buttonActive.Active();
        });

#if UNITY_ANDROID && !UNITY_EDITOR

       
        // Dừng camera khi không cần nữa
        if (webCamTexture != null)
        {
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
