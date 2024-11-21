using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Android;
using Spine.Unity;
using BaseService;
using TMPro;
public class EndGameLoad : MonoBehaviour
{
    public NativeAdHandler winNative;
    public NativeAdHandler loseNative;
    public int WinCount;
  
    [Header("Lose")]
    public Image bg;
    public GameObject losePanel;
    public Image you_lose;
    public Image icon;
    public Button skip;
    public Button noThank;
    public Button peplay;
    public Color color;

    void Sound(bool win)
    {
        UIManager.I.TurnOnMusic(false);
        if (Setting.SoundCheck())
        {
            AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
            audioSource.gameObject.SetActive(true);
            audioSource.clip = win ? UIManager.I.win : UIManager.I.lose;
            audioSource.Play();
        }
    }
    public void Lose()
    {
        ADS.LogEndLevel(UIManager.I.currentSceneLoad, "", false);

        UIManager.I.tickShowTutorial = false;
        loseNative.ReLoadNative();
        if (UIManager.I.currentSceneLoad > 2)
        {
            loseCount++;
        }
       

        UIManager.I.buttonActive.DeActive();
        if (UIManager.I._settingPanel.activeInHierarchy)
        {
            UIManager.I.GetComponent<Setting>().Close();
        }
        float timeDelayOpenLosePanel = 0;

        if (UIManager.I.isOpenCam)
        {
            UIManager.I.StopCam();
            UIManager.I.OpenCamPanel(false);
            timeDelayOpenLosePanel = 1;
        }
        DOVirtual.DelayedCall(timeDelayOpenLosePanel, () =>
        {
            //UIManager.I.UnLoadScene();
            OpenLosePanel();
            DOVirtual.DelayedCall(2.1f, () =>
            {
                UIManager.I.buttonActive.Active();
            });
        });
       




    }
    void OpenLosePanel(bool open = true)
    {
        if(open)
        {
            //loseNative.Native.gameObject.SetActive(true);
            skip.enabled = true;
            loading.gameObject.SetActive(false);
            SkipLevel.gameObject.SetActive(true);
            Sound(false);
            losePanel.gameObject.SetActive(open);
            UIAnimation.Fade(bg, 0.3f, true, 0.97f);

            UIAnimation.HorizontalTween(you_lose.gameObject, 0, open, UIManager.I._camera, UIAnimation.Direction.left);
            UIAnimation.HorizontalTween(icon.gameObject, 0, open, UIManager.I._camera, UIAnimation.Direction.left);

            UIAnimation.ScaleTween(you_lose.gameObject.transform, 0.7f, open, Vector3.zero, Vector3.one);
            UIAnimation.ScaleTween(icon.gameObject.transform, 0.7f, open, Vector3.zero, Vector3.one, 0.7f);

            //UIAnimation.HorizontalTween(peplay.gameObject, 0.7f, open, UIManager.I._camera, UIAnimation.Direction.left, 1.4f);
            UIAnimation.HorizontalTween(skip.gameObject, 0.7f, open, UIManager.I._camera, UIAnimation.Direction.left, 1.4f);

            UIAnimation.HorizontalTween(noThank.gameObject, 0, open, UIManager.I._camera, UIAnimation.Direction.left);
            UIAnimation.ScaleTween(noThank.gameObject.transform, 0.5f, open, Vector3.zero, Vector3.one, 2);

        }
        else
        {
            UIManager.I.buttonActive.DeActive();

            UIAnimation.HorizontalTween(you_lose.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left,0.6f);
            UIAnimation.HorizontalTween(icon.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.right,0.4f);
            //UIAnimation.HorizontalTween(peplay.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left);
            UIAnimation.HorizontalTween(skip.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left, 0.3f);
            UIAnimation.HorizontalTween(noThank.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.right);
            loseNative.Native.gameObject.SetActive(false);
            DOVirtual.DelayedCall(0.9f, () =>
            {
                
                bg.DOFade(1, 0.5f).OnComplete(() =>
                {
                    UIManager.I.ShowRequest(false, "0", "0");
                    UIManager.I.UnLoadScene();
                    
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        UIManager.I.LoadLevel();
                    });
                    bg.DOFade(0, 0.5f).SetDelay(0.2f).OnComplete(() =>
                    {
                        UIManager.I.TurnOnMusic(Setting.MusicCheck());
                        losePanel.gameObject.SetActive(open);
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            UIManager.I.buttonActive.Active();
                        });
                    });
                });
                
            });
            
        }
       
    }

    public SkeletonGraphic tym;
    void OpenWinPanel(bool open = true)
    {
        if (open)
        {
            Sound(true);
            //winNative.Native.gameObject.SetActive(true );
            UIAnimation.Fade(bgWin, 0.3f, true, 0.97f);

            winPanel.gameObject.SetActive(open);

            UIAnimation.VerticalTween(Box.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.top, 0.3f, 283f);
            UIAnimation.ScaleTween(frame.gameObject.transform, 0.5f, open, Vector3.zero, Vector3.one * 0.3f, 0.8f);

            UIAnimation.HorizontalTween(Next.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.right, 0.9f, 162.92f);
            UIAnimation.HorizontalTween(SavePhoto.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left, 1.1f, -126.28f);

            DOVirtual.DelayedCall(1, () => tym.AnimationState.SetAnimation(0, "anim", false));

          

        }
        else
        {
            UIManager.I.buttonActive.DeActive();
            UIAnimation.VerticalTween(Box.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.top, 0, 283f);
            UIAnimation.ScaleTween(frame.gameObject.transform, 0.5f, open, Vector3.zero, Vector3.one * 0.3f);
            UIAnimation.HorizontalTween(Next.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.right);
            UIAnimation.HorizontalTween(SavePhoto.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left);
            winNative.Native.gameObject.SetActive(false);
            DOVirtual.DelayedCall(0.7f, () =>
            {
               
                bgWin.DOFade(1, 0.5f).OnComplete(() =>
                {
                    UIManager.I.ShowRequest(false, "0", "0");
                    UIManager.I.UnLoadScene();

                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        UIManager.I.LoadLevel();
                    });
                    bgWin.DOFade(0, 0.5f).SetDelay(0.2f).OnComplete(() =>
                    {
                        UIManager.I.TurnOnMusic(Setting.MusicCheck());
                        winPanel.gameObject.SetActive(open);
                        EnableeSaveButton(true);
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            UIManager.I.buttonActive.Active();
                        });
                    });
                });

            });

 

        }

    }
    private void Start()
    {
        peplay.onClick.AddListener( ()=>OpenLosePanel(false));
        skip.onClick.AddListener(() => SkipLevel_Click());

        noThank.onClick.AddListener(() => RePlayButton_Click());

        Next.onClick.AddListener(() => NextButton_Click());
        SavePhoto.onClick.AddListener(() => SaveImageToGallery());
    }
    public int winCount = 0;
    public int loseCount = 0;
    void NextButton_Click()
    {
        if(winCount >= 2)
        {
            UIManager.I.buttonActive.DeActive();
            UIManager.I.interval.ShowInterstitial(() =>
            {
                OpenWinPanel(false);
                winCount = 0;
            });
        }
        else
        {
            OpenWinPanel(false);
        }
    }
    void RePlayButton_Click()
    {
        if (loseCount >= 2)
        {
            UIManager.I.buttonActive.DeActive();
            UIManager.I.interval.ShowInterstitial(() =>
            {
                OpenLosePanel(false);
                loseCount = 0;
            });
        }
        else
        {
            OpenLosePanel(false);
        }
    }
    public GameObject loading;
    public GameObject SkipLevel;
    
    void SkipLevel_Click()
    {
        Debug.Log("Reward start");
        //AdsUtils.ShowRewardWithBackFill((result) =>
        //{
        //    if (result == AdsManager.AdsResult.Success)
        //    {
        //        Debug.Log("Reward done");
        //        UIManager.I.LevelUp();
        //        OpenLosePanel(false);
        //        // Handle success (e.g., grant reward)
        //    }
        //    else
        //    {
        //        Debug.Log("Reward fail");
        //        // Handle failure (e.g., show error or retry)
        //    }
        loading.gameObject.SetActive(true);
        SkipLevel.gameObject.SetActive(false);
        skip.enabled = false;
        UIManager.I.buttonActive.DeActive();
        //
        //});
        UIManager.I.reward.ShowRewardedAd(() =>

        {
            Debug.Log("Reward done");
            UIManager.I.LevelUp();
            UIManager.I.buttonActive.Active();
            OpenLosePanel(false);
            loading.gameObject.SetActive(false);
            SkipLevel.gameObject.SetActive(true);

            skip.enabled = true;
        }, () =>
        {
            loading.gameObject.SetActive(false);
            SkipLevel.gameObject.SetActive(true);
            OpenNotification("  No ads available.");
            UIManager.I.buttonActive.Active();
            skip.enabled = true;
        });

    }


    [Header("Win")]
   
    public Image bgWin;
    public GameObject winPanel;
    public Image Box;
    public Image frame;
    public Button Next;
    public Button SavePhoto;
    public Image pic;

    public void Win()
    {
        ADS.LogStartLevel(UIManager.I.currentSceneLoad);

        UIManager.I.tickShowTutorial = false;
        WinCount++;
        winNative.ReLoadNative();
        if (UIManager.I.currentSceneLoad > 2)
        {
            winCount++;
        }

        UIManager.I.LevelUp();
        SceceCapture();
       
        //UIManager.I.ShowTime(false, 100);


    }
    public void SceceCapture()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        // Đợi đến khi khung hình cuối được vẽ xong
        yield return new WaitForEndOfFrame();

        
        
        if (UIManager.I._settingPanel.activeInHierarchy)
        {
            UIManager.I.GetComponent<Setting>().Close();
        }

        UIManager.I.buttonActive.DeActive();


        float timeDelayOpenLosePanel = 0;
        if (UIManager.I.isOpenCam)
        {
            UIManager.I.StopCam();
            UIManager.I.OpenCamPanel(false);
            timeDelayOpenLosePanel = 1;
             
            
        }
        DOVirtual.DelayedCall(timeDelayOpenLosePanel, () =>
        {
            //UIManager.I.UnLoadScene();
            OpenWinPanel();
            //DOVirtual.DelayedCall(2.5f, () =>
            //{
            //    if (WinCount == 3)
            //    {
            //        UIManager.I.rating.Voted();
            //    }
            //});

            if(WinCount == 3)
            {
                if (UIManager.I.rating.Voted())
                {
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        UIManager.I.buttonActive.Active();
                    });
                }
            }
            else
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    UIManager.I.buttonActive.Active();
                });
            }
           
        });

        ScreenShoot();
  

      
    }
    void ScreenShoot()
    {



        int captureHeight = 1500;//Screen.height * 7 / 10;
        Texture2D screenshot = new Texture2D(Screen.width, captureHeight, TextureFormat.RGB24, false);


        int startY = (Screen.height / 2) - (1500 / 2); //(Screen.height / 10)*2;

        // Đọc các pixel từ vùng giữa của khung hình vào Texture2D
        screenshot.ReadPixels(new Rect(0, startY, Screen.width, captureHeight), 0, 0);
        screenshot.Apply();

        // Gán Texture2D vào Sprite và hiển thị trong Image
        pic.sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));

        pic.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width , captureHeight);

        saveTexture = screenshot;
        fileName = "TickTockSave" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        //SaveImageToGallery();
       
    }
    Texture2D saveTexture;
    string fileName = "TickTockSave";
    public void SaveImageToGallery()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            // Nếu chưa có quyền, yêu cầu cấp quyền
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            StartCoroutine(WaitToHavePermission());

        }
        else
        {
            SaveImage(saveTexture, fileName);
        }

        

        
    }
    //void OnApplicationFocus(bool hasFocus)
    //{
    //    if (hasFocus && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
    //    {
    //        // Nếu có quyền, lưu ảnh
    //        SaveImageToGallery();
    //    }
    //    else if (hasFocus && !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
    //    {
    //        StopCoroutine("WaitToHavePermission");
    //        // Nếu không có quyền, có thể hiển thị thông báo hoặc yêu cầu cấp lại quyền
    //        Debug.LogWarning("Permission for storage access denied!");
    //    }
    //}
    IEnumerator WaitToHavePermission()
    {
        // Chờ cho đến khi người dùng cấp quyền
        yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite));

        // Sau khi có quyền, thực hiện lưu ảnh
        SaveImageToGallery();
    }
    void EnableeSaveButton(bool enable)
    {
        SavePhoto.enabled = enable;
        Image grayImage = SavePhoto.transform.GetChild(0).GetComponent<Image>();
        if (enable)
        {
            grayImage.gameObject.SetActive(false);
            grayImage.DOFade(0, 0);
        }
        else
        {
            grayImage.gameObject.SetActive(true);
            grayImage.DOFade(1, 0.3f);
        }

    }
    public GameObject notification;
    void OpenNotification(String noti)
    {
        Debug.Log("Show Notification");
        UIManager.I.Haptic();
        notification.gameObject.SetActive(true );
        notification.GetComponentInChildren<TMP_Text>().text = noti;// "  The image has been saved to your device.";

        RectTransform rt = notification.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 160);
        rt.DOAnchorPos(new Vector2(rt.anchoredPosition.x, -160), 0.5f);
        DOVirtual.DelayedCall(2, () =>
        {
            rt.DOAnchorPos(new Vector2(rt.anchoredPosition.x, 160), 0.5f).OnComplete(() =>
            {
                notification.gameObject.SetActive(false);
            });
        });
    }
    private void SaveImage(Texture2D texture, string fileName)
    {
        EnableeSaveButton(false);
        OpenNotification("  The image has been saved to your device.");

        // Chuyển texture thành dữ liệu PNG
        byte[] imageData = texture.EncodeToPNG();

        // Đặt tên file với định dạng PNG
        string fileFullName = fileName + ".png";

        // Lưu ảnh vào thư mục Pictures trên Android
#if UNITY_ANDROID
        string path = Path.Combine(GetAndroidExternalStoragePublicDirectory(), fileFullName);
        File.WriteAllBytes(path, imageData);

        // Yêu cầu hệ thống quét lại thư mục ảnh
        AddImageToGallery(path);
        Debug.Log("Image saved to: " + path);
#else
        Debug.LogWarning("This method only works on Android.");
#endif
    }

    // Lấy thư mục Pictures trên Android
    private string GetAndroidExternalStoragePublicDirectory()
    {
        using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
        {
            using (AndroidJavaObject directory = environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", environment.GetStatic<string>("DIRECTORY_PICTURES")))
            {
                return directory.Call<string>("getAbsolutePath");
            }
        }
    }

    // Thêm ảnh vào thư viện ảnh Android để người dùng có thể xem ngay
    private void AddImageToGallery(string path)
    {
        using (AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection"))
        using (AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            mediaScanner.CallStatic("scanFile", context, new string[] { path }, null, null);
        }
    }
   

    
}
