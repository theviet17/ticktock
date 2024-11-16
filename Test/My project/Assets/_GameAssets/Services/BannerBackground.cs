using System.Collections;
using System.Collections.Generic;
using BaseMe;
using UnityEngine;
using UnityEngine.UI;

public class BannerBackground : Singleton<BannerBackground>
{
    [SerializeField]
    private Image background;

    public void ShowBannerBackground(bool show)
    {
        background.enabled = show;
    }

    public float GetBannerHeight()
    {
        var rect = background.GetComponent<RectTransform>();
        return rect.rect.height;
    }
}
