using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Text.RegularExpressions;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

public class UIAnimation 
{
    /// <summary>
    /// Tìm kiếm object với tên
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform GetFragmentWithName(GameObject parent, string name)
    {
        return parent.transform.Find(name);
    }
    public static int GetIntInString(string name)
    {
        var match = Regex.Match(name, @"\d+");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        return -1;
    }

    /// <summary>
    /// Fit backgound theo tỷ lệ màn hìn
    /// </summary>
    /// <param name="bg"> Background</param>
    /// <param name="camera"> UI camera</param>
    public static void FitBackgroundToScene(GameObject bg, Camera camera)
    {
        // Lấy  chiều rộng và chiều cao của camera
        var width = camera.pixelWidth;
        var height = camera.pixelHeight;

        RectTransform bgRectTransform = bg.GetComponent<RectTransform>();
      
        // Thiết lập kích thước của RectTransform để phù hợp với kích thước camera
        bgRectTransform.sizeDelta = new Vector2(width, height);
    }
    /// <summary>
    /// Tween object theo chiều ngang
    /// </summary>
    /// <param name="uiObject"> gameobject cần tween </param>
    /// <param name="duration"> khoảng thời gian </param>
    /// <param name="isOpening"> trạng thái target là mở hay đóng</param>
    public static void HorizontalTween(GameObject uiObject, float duration, bool isOpening, float delay =0 )
    {
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();

        AnchorPosition anchorPosition = CheckAnchorPosition(rectTransform);

        if(anchorPosition == AnchorPosition.Top_Left || anchorPosition == AnchorPosition.Middle_Left || anchorPosition == AnchorPosition.Bottom_Left)
        {
            Vector2 targetPosition;

            if ((rectTransform.anchoredPosition.x >= 0 && isOpening) || (rectTransform.anchoredPosition.x < 0 && !isOpening)) // đang ở đúng vị trí, thường là khi bắt đầu game
            {
                targetPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector3(-targetPosition.x, targetPosition.y);
            }
            else
            {
                targetPosition = new Vector3(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
            }
            rectTransform.DOAnchorPos(targetPosition, duration).SetEase(isOpening? Ease.OutBack : Ease.InBack).SetDelay(delay);
            return;

        }
        if (anchorPosition == AnchorPosition.Top_Right || anchorPosition == AnchorPosition.Middle_Right || anchorPosition == AnchorPosition.Bottom_Right)
        {
            Vector2 targetPosition;

            if ((rectTransform.anchoredPosition.x < 0 && isOpening) || (rectTransform.anchoredPosition.x >= 0 && !isOpening))
            {
                targetPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector3(-targetPosition.x, targetPosition.y);
            }
            else
            {
                targetPosition = new Vector3(-rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
            }
            rectTransform.DOAnchorPos(targetPosition, duration).SetEase(isOpening ? Ease.OutBack : Ease.InBack).SetDelay(delay);
            return;
        }
    
    }
    /// <summary>
    /// Tween object theo chiều ngang
    /// </summary>
    /// <param name="uiObject">gameobject cần tween </param>
    /// <param name="duration"> khoảng thời gian </param>
    /// <param name="isOpening"> trạng thái target là mở hay đóng </param>
    /// <param name="camera"> ui Camera để lấy chiều ngang màn hình</param>
    /// <param name="direction"> hướng tween sẽ là từ trái hoặc đến bên trái , từ phải hoặc đến bên phải</param>
    /// <param name="custumeX"> chỉnh sửa vị trí x phù hợp </param>
    public static void HorizontalTween(GameObject uiObject, float duration, bool isOpening, Camera camera , Direction direction ,float delay = 0 , float custumeX = 0)
    {
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        float width = camera.pixelWidth;
        Vector2 dir = direction == Direction.left ? new Vector2(-width*2, rectTransform.anchoredPosition.y) : new Vector2(width*2, rectTransform.anchoredPosition.y);

        AnchorPosition anchorPosition = CheckAnchorPosition(rectTransform);

        if (anchorPosition == AnchorPosition.Center_Top || anchorPosition == AnchorPosition.Center || anchorPosition == AnchorPosition.Center_Bottom)
        {
            Vector2 targetPosition;

            rectTransform.anchoredPosition = isOpening ? dir : new Vector2(custumeX, rectTransform.anchoredPosition.y);
            targetPosition = isOpening ? new Vector2(custumeX, rectTransform.anchoredPosition.y) : dir;

            rectTransform.DOAnchorPos(targetPosition, duration).SetEase(isOpening ? Ease.OutBack : Ease.InBack).SetDelay(delay);
        

        }
        
    }
    /// <summary>
    /// Tween object theo chiều dọc
    /// </summary>
    /// <param name="uiObject"> gameobject cần tween </param>
    /// <param name="duration"> khoảng thời gian </param>
    /// <param name="isOpening"> trạng thái target là mở hay đóng</param>
    public static void VerticalTween(GameObject uiObject, float duration, bool isOpening, float delay = 0)
    {
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();

        AnchorPosition anchorPosition = CheckAnchorPosition(rectTransform);

        if (anchorPosition == AnchorPosition.Top_Left || anchorPosition == AnchorPosition.Top_Right || anchorPosition == AnchorPosition.Center_Top)
        {
            Vector2 targetPosition;

            if ((rectTransform.anchoredPosition.y < 0 && isOpening) || (rectTransform.anchoredPosition.y >= 0 && !isOpening)) // đang ở đúng vị trí, thường là khi bắt đầu game
            {
                targetPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector3(targetPosition.x, -targetPosition.y);
            }
            else
            {
                targetPosition = new Vector3(rectTransform.anchoredPosition.x, -rectTransform.anchoredPosition.y);
            }
            rectTransform.DOAnchorPos(targetPosition, duration).SetEase(isOpening ? Ease.OutBack : Ease.InBack).SetDelay(delay);
            return;

        }
        if (anchorPosition == AnchorPosition.Bottom_Left || anchorPosition == AnchorPosition.Bottom_Right || anchorPosition == AnchorPosition.Center_Bottom)
        {
            Vector2 targetPosition;

            if ((rectTransform.anchoredPosition.y >= 0 && isOpening) || (rectTransform.anchoredPosition.y < 0 && !isOpening)) // đang ở đúng vị trí, thường là khi bắt đầu game
            {
                targetPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector3(targetPosition.x, -targetPosition.y);
            }
            else
            {
                targetPosition = new Vector3(rectTransform.anchoredPosition.x, -rectTransform.anchoredPosition.y);
            }
            rectTransform.DOAnchorPos(targetPosition, duration).SetEase(isOpening ? Ease.OutBack : Ease.InBack).SetDelay(delay);
            return;

        }

    }
    /// <summary>
    /// Tween object theo chiều dọc
    /// </summary>
    /// <param name="uiObject">gameobject cần tween </param>
    /// <param name="duration"> khoảng thời gian </param>
    /// <param name="isOpening"> trạng thái target là mở hay đóng </param>
    /// <param name="camera"> ui Camera để lấy chiều ngang màn hình</param>
    /// <param name="direction"> hướng tween sẽ là từ trái hoặc đến bên trái , từ phải hoặc đến bên phải</param>
    /// <param name="custumeX"> chỉnh sửa vị trí x phù hợp </param>
    public static void VerticalTween(GameObject uiObject, float duration, bool isOpening, Camera camera, Direction direction, float delay = 0, float custumeY = 0)
    {
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        Debug.Log("Target Vertical Tween 2" + rectTransform.anchoredPosition);
        float height = camera.pixelHeight;
        Vector2 dir = direction == Direction.top ? new Vector2(rectTransform.anchoredPosition.x, height) : new Vector2(rectTransform.anchoredPosition.x , -height);

        AnchorPosition anchorPosition = CheckAnchorPosition(rectTransform);

        if (anchorPosition == AnchorPosition.Center_Top || anchorPosition == AnchorPosition.Center || anchorPosition == AnchorPosition.Center_Bottom)
        {
            Vector2 targetPosition;

            rectTransform.anchoredPosition = isOpening ? dir : new Vector2(rectTransform.anchoredPosition.x, custumeY);
            targetPosition = isOpening ? new Vector2( rectTransform.anchoredPosition.x, custumeY) : dir;
            Debug.Log("Target Vertical Tween" + targetPosition + custumeY);
            rectTransform.DOAnchorPos(targetPosition, duration).SetEase(isOpening ? Ease.OutBack : Ease.InBack).SetDelay(delay) ;


        }

    }
    /// <summary>
    /// Fade 
    /// </summary>
    /// <param name="image"> image cần fade </param>
    /// <param name="duration"> khoảng thời gian fade </param>
    /// <param name="isOpening">trạng thái target là mở hay đóng</param>
    /// <param name="maxOpacity"></param>
    public static void Fade(Image image, float duration, bool isOpening, float maxOpacity = 0.9f)
    {
        float targetOpacity = isOpening ? maxOpacity : 0;
        var a = image.color;
        a.a = isOpening ? 0 : maxOpacity;
        image.color = a;
        image.DOFade(targetOpacity, duration).SetEase(Ease.Linear);
    }
    public static void Fade(SpriteRenderer sprite, float duration, bool isOpening, float maxOpacity = 0.9f)
    {
        float targetOpacity = isOpening ? maxOpacity : 0;
        var a = sprite.color;
        a.a = isOpening ? 0 : maxOpacity;
        sprite.color = a;
        sprite.DOFade(targetOpacity, duration).SetEase(Ease.Linear);
    }
    /// <summary>
    /// Fade 
    /// </summary>
    /// <param name="cg"> canvas group cần fade </param>
    /// <param name="duration"> khoảng thời gian fade </param>
    /// <param name="isOpening">trạng thái target là mở hay đóng</param>
    /// <param name="maxOpacity"></param>
    public static void Fade(CanvasGroup cg, float duration, bool isOpening, float maxOpacity = 0.9f)
    {
        float targetOpacity = isOpening ? maxOpacity : 0;
        cg.alpha = isOpening ? 0 : maxOpacity;
        cg.DOFade(targetOpacity, duration).SetEase(Ease.Linear);
    }

    /// <summary>
    /// Scale object
    /// </summary>
    /// <param name="tf"></param>
    /// <param name="duration"></param>
    /// <param name="isOpening"></param>
    /// <param name="startScale"> scale lúc bắt đầu</param>
    /// <param name="endScale"> scale khi kết thúc </param>
    public static void ScaleTween(Transform tf, float duration, bool isOpening, Vector3 minScale, Vector3 maxScale, float delay = 0)
    {
     
        Vector3 targetScale= isOpening ? maxScale : minScale;
        tf.transform.localScale = isOpening? minScale : maxScale;
        tf.DOScale(targetScale, duration).SetEase(isOpening ? Ease.OutBack : Ease.InBack).SetDelay(delay);
    }

    /// <summary>
    /// lấy số trong text, Dotween số về targetValue , OnUpdate cập nhật text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="targetValue"></param>
    /// <param name="duration"></param>
    public static void TextTween(TMP_Text text, int targetValue, float duration)
    {
        Dictionary<string, int> dictionary = GetIndexDictionaryFromString(text.text);
   
        string textValue = dictionary.Keys.First();
        int startValue = dictionary[textValue];

        // Sử dụng DOTween để tween giá trị từ startValue đến targetValue
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            text.text = textValue + Mathf.RoundToInt(startValue).ToString(); // Cập nhật text với giá trị làm tròn
        }, targetValue, duration);
    }

    /// <summary>
    /// Tween đơn vị tiền tệ , sinh ra tại 1 vị trí và di chuyển về vị trí đích
    /// </summary>
    /// <param name="startPosition"> vị trí bắt đầu </param>
    /// <param name="randomStart"> khoảng random quanh vị trí bắt đầu</param>
    /// <param name="target"> vị trí đích</param>
    /// <param name="value"> tổng giá trị của phần thưởng</param>
    /// <param name="currencyValue">giá trị mỗi phần thưởng , ví dụ 1 đông vàng có giá trị là 5 từ đó tính được số lượng đồng vàng dựa trên <param name="value" </param>
    /// <param name="currencyPool"></param>
    /// <param name="action"></param>
    public static void CurrencyTween(Transform startPosition, float randomStart, Transform target, int value, int currencyValue,ObjectPool currencyPool , Action action = null)
    {
        int currencyNumber = value / currencyValue;
        currencyNumber = Mathf.Clamp(currencyNumber, 1, 10);// giới hạn từ 1 đồng đến 10 đồng 

        for (int i = 0; i < currencyNumber; i++)
        {
            GameObject currencyIcon = currencyPool.GetObjectFromPool();
            currencyIcon.gameObject.SetActive(true);

            currencyIcon.transform.localScale = Vector3.zero;

            //RectTransform rectTransform = currencyIcon.GetComponent<RectTransform>();
            var iconStartPosition = new Vector2(startPosition.position.x + UnityEngine.Random.Range(-randomStart, randomStart), startPosition.position.y + UnityEngine.Random.Range(-randomStart, randomStart));
            currencyIcon.transform.position = iconStartPosition;

            var timedelay = 0.1f * i;
            currencyIcon.transform.DOScale(1f, 0.3f).SetDelay(timedelay).SetEase(Ease.OutBack);

            currencyIcon.transform.DOMove(target.position, 0.8f)
                .SetDelay(timedelay + 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    currencyIcon.transform.DOScale(Vector3.one, 0.05f);
                    currencyPool.ReturnObjectToPool(currencyIcon);
                    action?.Invoke(); 
                });

         
        }
    }

    /// <summary>
    /// lấy số trong chuỗi string
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static int GetIndexFromString(string name)
    {
        var match = Regex.Match(name, @"\d+");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        return -1;
    }
    /// <summary>
    /// lấy ra cả string và int từ string
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static Dictionary<string, int> GetIndexDictionaryFromString(string name)
    {
        var result = new Dictionary<string, int>();

        // Tách số và chữ trong chuỗi
        var match = Regex.Match(name, @"\d+");
        var nameOnly = Regex.Replace(name, @"\d+", ""); // Loại bỏ số trong tên

        if (match.Success)
        {
            int number = int.Parse(match.Value);
            result.Add(nameOnly, number);
        }
        else
        {
            result.Add(nameOnly, -1);
        }

        return result;
    }

    /// <summary>
    /// // Hàm kiểm tra giá trị có nằm trong khoảng (a, b) không
    /// </summary>
    /// <param name="value"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static bool IsInRange(float value, float a, float b)
    {
        // Xác định khoảng nhỏ và lớn để đảm bảo không bị lỗi nếu a > b
        float min = Mathf.Min(a, b);
        float max = Mathf.Max(a, b);

        // Kiểm tra nếu giá trị nằm trong khoảng (a, b)
        Debug.Log(value > min && value < max);
        return value > min && value < max;
    }

    static AnchorPosition CheckAnchorPosition(RectTransform rectTransform)
    {
        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;

        if (anchorMin == new Vector2(0, 1) && anchorMax == new Vector2(0, 1))
        {
            Debug.Log("Anchor is at Top Left");
            return AnchorPosition.Top_Left;
        }
        else if (anchorMin == new Vector2(1, 1) && anchorMax == new Vector2(1, 1))
        {
            Debug.Log("Anchor is at Top Right");
            return AnchorPosition.Top_Right;
        }
        else if (anchorMin == new Vector2(0, 0) && anchorMax == new Vector2(0, 0))
        {
            Debug.Log("Anchor is at Bottom Left");
            return AnchorPosition.Bottom_Left;
        }
        else if (anchorMin == new Vector2(1, 0) && anchorMax == new Vector2(1, 0))
        {
            Debug.Log("Anchor is at Bottom Right");
            return AnchorPosition.Bottom_Right;
        }
        else if (anchorMin == new Vector2(0, 0.5f) && anchorMax == new Vector2(0, 0.5f))
        {
            Debug.Log("Anchor is at Middle Left");
            return AnchorPosition.Middle_Left;
        }
        else if (anchorMin == new Vector2(1, 0.5f) && anchorMax == new Vector2(1, 0.5f))
        {
            Debug.Log("Anchor is at Middle Right");
            return AnchorPosition.Middle_Right;
        }
        else if (anchorMin == new Vector2(0.5f, 1) && anchorMax == new Vector2(0.5f, 1))
        {
            Debug.Log("Anchor is at Center Top");
            return AnchorPosition.Center_Top;
        }
        else if (anchorMin == new Vector2(0.5f, 0.5f) && anchorMax == new Vector2(0.5f, 0.5f))
        {
            Debug.Log("Anchor is at Center");
            return AnchorPosition.Center;
        }
        else if (anchorMin == new Vector2(0.5f, 0) && anchorMax == new Vector2(0.5f, 0))
        {
            Debug.Log("Anchor is at Center Bottom");
            return AnchorPosition.Center_Bottom;
        }
        else
        {
            Debug.Log("Anchor is at a different position.");
            return AnchorPosition.Diffirent;
        }
    }

    public enum AnchorPosition
    {
        Top_Left,
        Top_Right,
        Bottom_Left,
        Bottom_Right,
        Middle_Left,
        Middle_Right,
        Center_Top,
        Center,
        Center_Bottom,
        Diffirent

    }
    public enum Direction
    {
        top,
        bottom,
        left,
        right,
        center
    }
}

