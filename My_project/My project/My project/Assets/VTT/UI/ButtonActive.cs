using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonActive : MonoBehaviour
{
    public enum ButtonStatus
    {
        CantActive,
        Active
    }
    public Button[] buttons;
    public ButtonStatus buttonStatus = ButtonStatus.Active;
    public void DeActive()
    {
        buttonStatus = ButtonStatus.CantActive;
        SetButtonsInteractable(false);
    }
    public void Active()
    {
        buttonStatus = ButtonStatus.Active;
        SetButtonsInteractable(true);
    }
    private void Start()
    {
        //buttons = UIManager.I.canvas.GetComponentsInChildren<Button>();
    }

    void Update()
    {
        //CheckHaveTweening()
    }
    //public List<string> running = new List<string>();
    //void CheckHaveTweening()
    //{
    //    bool isAnyTweenRunning;
    //    try
    //    {
    //        List<Tween> runningTweens = DOTween.PlayingTweens();

    //        // Sử dụng List<Tween>.RemoveAll để loại bỏ các tween có hasLoops
    //        runningTweens.RemoveAll(tween => tween.hasLoops);

    //        isAnyTweenRunning = runningTweens.Count > 0;
    //    }
    //    catch
    //    {
    //        isAnyTweenRunning = false;
    //    }


    //    if (isAnyTweenRunning && buttonStatus == ButtonStatus.Active)
    //    {
    //        buttonStatus = ButtonStatus.CantActive;
    //        SetButtonsInteractable(false);
    //    }
    //    else if (!isAnyTweenRunning && buttonStatus == ButtonStatus.CantActive)
    //    {
    //        buttonStatus = ButtonStatus.Active;
    //        SetButtonsInteractable(true);
    //    }
    //}

    private void SetButtonsInteractable(bool active)
    {
        foreach (Button btn in buttons)
        {
            btn.enabled = active;
        }
    }
}
