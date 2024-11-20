using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomProgressBar : MonoBehaviour
{
    public List<Color> colors = new List<Color>();
    public RectTransform content;
    Image contentValue;
    Vector2 contentAnchorPos;
    Vector2 contentStartAnchorPos;
    public bool changColor = true;

    void Awake()
    {
        contentValue = content.GetComponent<Image>();
        if (changColor)
        {
            contentValue.color = colors[0];
        }
        contentAnchorPos = content.anchoredPosition;
        contentStartAnchorPos = new Vector2(-contentAnchorPos.x , contentAnchorPos.y);
        Debug.Log("SetProgressBar_Start" + contentStartAnchorPos);


    }
    public void SetStartContentAnchor()
    {
        //_value = 0;
        Value = 0;
        //content.anchoredPosition = contentStartAnchorPos;
        Debug.Log("SetProgressBar" + contentStartAnchorPos);
    }

    [SerializeField]private float _value;
    public float Value
    {
        get => _value;
        set
        {
            if (this._value != value || value == 0)
            {
                this._value = value;
                OnValueChanged();
            }
        }
    }

    void OnValueChanged()
    {
        if (changColor)
        {
            if (_value / 1 > 0.6f)
            {
                contentValue.DOColor(colors[2], 0.2f);
            }
            else if (_value / 1 > 0.3f)
            {
                contentValue.DOColor(colors[1], 0.2f);
            }
            else
            {
                contentValue.DOColor(colors[0], 0f);
            }
        }
       

        content.anchoredPosition = Vector2.Lerp(contentStartAnchorPos , contentAnchorPos, _value/1);
    }
}
