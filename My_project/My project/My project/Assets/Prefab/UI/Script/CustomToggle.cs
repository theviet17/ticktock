using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CustomToggle : MonoBehaviour 
{
    Toggle toggle;
    Image onBG;
    Image offBG;
    RectTransform dotOn;
    RectTransform dotOff;
    Vector2 onDotAnchor;
    void Awake()
    {
        toggle = GetComponent<Toggle>();

        onBG = transform.Find("On").GetComponent<Image>();
        offBG = transform.Find("Off").GetComponent<Image>();
        dotOn = transform.Find("Dot").GetComponent<RectTransform>();
        onDotAnchor = dotOn.anchoredPosition;

        dotOff = transform.Find("DotOff").GetComponent<RectTransform>();
        onDotAnchor = dotOff.anchoredPosition;

        toggle.onValueChanged.AddListener(OnSwitch);


        OnSwitch(toggle.isOn);
    }

    void OnSwitch(bool on)
    {
        onBG.DOFade(on ? 1f : 0f, 0.1f);
        offBG.DOFade(on ? 0f : 1f, 0.1f);
        dotOn.DOAnchorPos(on ? onDotAnchor : -onDotAnchor, 0.1f);

        dotOff.DOAnchorPos(on ? onDotAnchor : -onDotAnchor, 0.1f);

        dotOn.GetComponent<Image>().DOFade(on ? 1f : 0f, 0.1f);
        dotOff.GetComponent<Image>().DOFade(on ? 0f : 1f, 0.1f);
        //onIcon.gameObject.SetActive(on);
        //offIcon.gameObject.SetActive(!on);

    }

}
