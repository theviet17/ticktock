using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Setting : MonoBehaviour
{
    public enum Status
    {
        open,
        close,
    }

    private const string IsMusicOn = "MusicTracking";
    private const string IsSoundOn = "SoundTracking";
    private const string IsVibrationOn = "HapticTracking";
    Toggle _music;
    Toggle _sound;
    Toggle _vibration;

    public Status status = Status.close;

    Image bg;
    GameObject box;
    public Button openSettingButton;

    public Button openSettingButton2;

    public Button confirmButton;
    Vector3 colseSettingButton_Anchor;



    public void Start()
    {
        GameObject group = UIManager.I._settingPanel.GetComponentInChildren<GridLayoutGroup>().gameObject;


        _music = UIAnimation.GetFragmentWithName(group, "Music").GetComponentInChildren<Toggle>();
        _sound = UIAnimation.GetFragmentWithName(group, "Sound").GetComponentInChildren<Toggle>();
        _vibration = UIAnimation.GetFragmentWithName(group, "Vibration").GetComponentInChildren<Toggle>();

        _music.onValueChanged.AddListener(delegate { TrackingToggle(_music); });
        _sound.onValueChanged.AddListener(delegate { TrackingToggle(_sound); });
        _vibration.onValueChanged.AddListener(delegate { TrackingToggle(_vibration); });

        LoadToggleData(_music);
        LoadToggleData(_sound);
        LoadToggleData(_vibration);

        bg = UIAnimation.GetFragmentWithName(UIManager.I._settingPanel, "BG").GetComponent<Image>();
        box = UIAnimation.GetFragmentWithName(UIManager.I._settingPanel, "Box").gameObject;
        confirmButton = UIAnimation.GetFragmentWithName(box, "Confirm_Button").GetComponent<Button>();
        SetButton();
    }

    float SetBackgroundOpacity()
    {
        return UIManager.I.gameStatus == GameStatus.home ? 1 : 0.6f;

    }
    void SetButton()
    {

        openSettingButton = UIManager.I.settingButton;
        openSettingButton.onClick.AddListener(() => Open());

        openSettingButton2 = UIManager.I.setting_Button_InGame;
        openSettingButton2.onClick.AddListener(() => Open());

        confirmButton.onClick.AddListener(() => Close());


    }
    public void Open()
    {
        UIManager.I._pause = true;
        UIManager.I.buttonActive.DeActive();
        Debug.Log(1);
        UIManager.I._settingPanel.gameObject.SetActive(true);
        UIAnimation.Fade(bg, 0.5f, true, 0.9f);
        UIAnimation.VerticalTween(box, 0.5f, true, UIManager.I._camera, UIAnimation.Direction.top);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            UIManager.I.buttonActive.Active();
        });


    }
    public void Close()
    {
        UIManager.I._pause = false;
        UIManager.I.buttonActive.DeActive();
        UIAnimation.Fade(bg, 0.5f, false, 0.9f);
        UIAnimation.VerticalTween(box, 0.5f, false, UIManager.I._camera, UIAnimation.Direction.top);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            UIManager.I._settingPanel.gameObject.SetActive(false);
        });
        DOVirtual.DelayedCall(0.5f, () =>
        {
            UIManager.I.buttonActive.Active();
        });
    }


    void LoadToggleData(Toggle toggle)
    {
        string linked = toggle == _music ? IsMusicOn : toggle == _sound ? IsSoundOn : IsVibrationOn;

        bool status = PlayerPrefs.GetInt(linked) == 0 ? true : false;

        toggle.isOn = status;
    }
    void TrackingToggle(Toggle toggle)
    {
        if (Time.time > 2)
        {
            if (SoundCheck())
            {
                AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                audioSource.gameObject.SetActive(true);
                audioSource.clip = UIManager.I.click;
                audioSource.Play();
            }
        }

        string linked = toggle == _music ? IsMusicOn : toggle == _sound ? IsSoundOn : IsVibrationOn;

        bool status = toggle.isOn;
        PlayerPrefs.SetInt(linked, status ? 0 : 1);

        if (linked == IsMusicOn)
        {
           UIManager.I.TurnOnMusic(toggle.isOn);
        }
    }

    public static bool SoundCheck()
    {
        string linked = IsSoundOn;
        return PlayerPrefs.GetInt(linked) == 0;
    }
    public static bool HapticCheck()
    {
        string linked = IsVibrationOn;
        return PlayerPrefs.GetInt(linked) == 0;
    }
    public static bool MusicCheck()
    {
        string linked = IsMusicOn;
        return PlayerPrefs.GetInt(linked) == 0;
    }



}
