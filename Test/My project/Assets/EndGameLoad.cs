using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

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
        if (UIManager.I.isOpenCam)
        {
            UIManager.I.StopShowCamera();
            UIManager.I.OpenCamPanel(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.I.UnLoadScene();
                OpenLosePanel();
                DOVirtual.DelayedCall(2.1f, () =>
                {
                    UIManager.I.buttonActive.Active();
                });
            });
        }
        else
        {
            UIManager.I.UnLoadScene();
            OpenLosePanel();
            DOVirtual.DelayedCall(2.1f, () =>
            {
                UIManager.I.buttonActive.Active();
            });
        }

       
      

    }
    void OpenLosePanel(bool open = true)
    {
        if(open)
        {

            Sound(false);
            bg.DOColor(Color.black, 0);
            losePanel.gameObject.SetActive(open);
            UIAnimation.HorizontalTween(you_lose.gameObject, 0, open, UIManager.I._camera, UIAnimation.Direction.left);
            UIAnimation.HorizontalTween(icon.gameObject, 0, open, UIManager.I._camera, UIAnimation.Direction.left);

            UIAnimation.ScaleTween(you_lose.gameObject.transform, 0.7f, open, Vector3.zero, Vector3.one);
            UIAnimation.ScaleTween(icon.gameObject.transform, 0.7f, open, Vector3.zero, Vector3.one, 0.7f);

            UIAnimation.HorizontalTween(peplay.gameObject, 0.7f, open, UIManager.I._camera, UIAnimation.Direction.left, 1.4f);
        }
        else
        {
            UIManager.I.buttonActive.DeActive();

            UIAnimation.HorizontalTween(you_lose.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left,0.4f);
            UIAnimation.HorizontalTween(icon.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.right,0.2f);
            UIAnimation.HorizontalTween(peplay.gameObject, 0.5f, open, UIManager.I._camera, UIAnimation.Direction.left);
            DOVirtual.DelayedCall(0.9f, () =>
            {
                bg.DOColor(color, 0.5f).OnComplete(() =>
                {
                    UIManager.I.TurnOnMusic(Setting.MusicCheck());
                    UIManager.I.LoadLevel();
                    losePanel.gameObject.SetActive(open);
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        UIManager.I.buttonActive.Active();
                    });
                });
            });
            
        }
       
    }
    private void Start()
    {
        peplay.onClick.AddListener( ()=>OpenLosePanel(false));
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
        if (UIManager.I.isOpenCam)
        {
            UIManager.I.StopShowCamera();
            UIManager.I.OpenCamPanel(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                UIManager.I.UnLoadScene();
                OpenWinPanel();
                DOVirtual.DelayedCall(1f, () =>
                {
                    UIManager.I.LevelUp();
                    UIManager.I.buttonActive.Active();
                });
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
       

        // Tạo Texture2D từ ảnh chụp màn hình
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Đọc các pixel từ khung hình vào Texture2D
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Gán Texture2D vào Sprite và hiện trong Image
        pic.sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));

        UIManager.I.ShowRequest(false, "0", "0");
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
