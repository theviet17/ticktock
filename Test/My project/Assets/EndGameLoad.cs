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
public class EndGameLoad : MonoBehaviour
{
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

            Sound(false);
            UIAnimation.Fade(bg, 0.3f, true, 0.97f);

            losePanel.gameObject.SetActive(open);
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

            DOVirtual.DelayedCall(0.9f, () =>
            {
                bg.DOFade(1, 0.5f).OnComplete(() =>
                {
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
    private void Start()
    {
        peplay.onClick.AddListener( ()=>OpenLosePanel(false));
        skip.onClick.AddListener(() => OpenLosePanel(false));
        noThank.onClick.AddListener(() => OpenLosePanel(false));
        Next.onClick.AddListener(() => OpenWinPanel(false));
    }


    [Header("Win")]
   
    public Image bgWin;
    public GameObject winPanel;
    public Image frame;
    public Button Next;
    public Image pic;

    public void Win()
    {

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
        float timeDelayOpenLosePanel = 0; //a

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

        if (UIManager.I.isOpenCam)
        {
            UIManager.I.StopCam();
            UIManager.I.OpenCamPanel(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.I.UnLoadScene();
               
            });
        }
        else
        {
            UIManager.I.UnLoadScene();
            OpenWinPanel();
            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.I.LevelUp();
                UIManager.I.buttonActive.Active();
            });
        }

        ScreenShoot();
        //// Tạo Texture2D từ ảnh chụp màn hình
        //Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        //// Đọc các pixel từ khung hình vào Texture2D
        //screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //screenshot.Apply();

        //// Gán Texture2D vào Sprite và hiện trong Image
        //pic.sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));

        UIManager.I.ShowRequest(false, "0", "0");
    }
    void ScreenShoot()
    {


        // Tạo Texture2D từ ảnh chụp màn hình với kích thước 3/5 của chiều cao
        int captureHeight = Screen.height * 7 / 10;
        Texture2D screenshot = new Texture2D(Screen.width, captureHeight, TextureFormat.RGB24, false);

        // Tính toán toạ độ Y để bắt đầu chụp từ 1/5 chiều cao từ trên
        int startY = (Screen.height / 10)*2;

        // Đọc các pixel từ vùng giữa của khung hình vào Texture2D
        screenshot.ReadPixels(new Rect(0, startY, Screen.width, captureHeight), 0, 0);
        screenshot.Apply();

        // Gán Texture2D vào Sprite và hiển thị trong Image
        pic.sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));

        pic.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width , captureHeight);

        saveTexture = screenshot;
        fileName = "TickTockSave" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        SaveImageToGallery();
       
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
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            // Nếu có quyền, lưu ảnh
            SaveImageToGallery();
        }
        else if (hasFocus && !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            StopCoroutine("WaitToHavePermission");
            // Nếu không có quyền, có thể hiển thị thông báo hoặc yêu cầu cấp lại quyền
            Debug.LogWarning("Permission for storage access denied!");
        }
    }
    IEnumerator WaitToHavePermission()
    {
        // Chờ cho đến khi người dùng cấp quyền
        yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite));

        // Sau khi có quyền, thực hiện lưu ảnh
        SaveImageToGallery();
    }
    private void SaveImage(Texture2D texture, string fileName)
    {
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
        ShowNotification(fileName);
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
    private void ShowNotification(string fileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = contextClass.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            using (AndroidJavaClass notificationManagerClass = new AndroidJavaClass("android.app.NotificationManager"))
            {
                using (AndroidJavaObject notificationManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "notification"))
                {
                    using (AndroidJavaClass notificationClass = new AndroidJavaClass("android.app.Notification$Builder"))
                    {
                        AndroidJavaObject notificationBuilder = notificationClass.Call<AndroidJavaObject>("new", currentActivity);

                        // Lấy tài nguyên icon đúng cách
                        AndroidJavaObject resources = currentActivity.Call<AndroidJavaObject>("getResources");
                        int iconId = resources.Call<int>("getIdentifier", "ic_launcher", "drawable", currentActivity.Call<string>("getPackageName"));
                        
                        notificationBuilder.Call<AndroidJavaObject>("setContentTitle", "Image Saved");
                        notificationBuilder.Call<AndroidJavaObject>("setContentText", fileName + " has been saved to the gallery.");
                        notificationBuilder.Call<AndroidJavaObject>("setSmallIcon", iconId);  // Sử dụng icon ID lấy từ resources
                        notificationBuilder.Call<AndroidJavaObject>("setAutoCancel", true);

                        AndroidJavaObject notification = notificationBuilder.Call<AndroidJavaObject>("build");

                        // Hiển thị thông báo
                        notificationManager.Call("notify", 0, notification);
                    }
                }
            }
        }
#endif
    }

    void OpenWinPanel(bool open = true)
    {
        if (open)
        {
            Sound(true);

            bgWin.DOColor(Color.black, 0);
            winPanel.gameObject.SetActive(open);
            //UIAnimation.HorizontalTween(frame.gameObject, 0, open, UIManager.I._camera, UIAnimation.Direction.left);

            UIAnimation.ScaleTween(frame.gameObject.transform, 0.5f, open, Vector3.zero, Vector3.one*0.3f);

            UIAnimation.HorizontalTween(Next.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left, 0.5f);
        }
        else
        {
            UIManager.I.buttonActive.DeActive();
            UIAnimation.ScaleTween(frame.gameObject.transform, 0.5f, open, Vector3.zero, Vector3.one * 0.3f);
            UIAnimation.HorizontalTween(Next.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left);

            DOVirtual.DelayedCall(0.7f, () =>
            {
                bgWin.DOColor(color, 0.5f).OnComplete(() =>
                {
                    UIManager.I.TurnOnMusic(Setting.MusicCheck());
                    UIManager.I.LoadLevel();
                    winPanel.gameObject.SetActive(open);

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        UIManager.I.buttonActive.Active();
                    });
                });
            });

        }

    }
}
