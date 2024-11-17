using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class KnifeThrowManager : MonoBehaviour
{
    public GameObject knifeSample;
    public Transform knifePosition;
    public Knift knife; // Dao
    public Transform circle;         // Vòng xoay
    public float rotationSpeed = 100f;  // Tốc độ quay của vòng
    public float throwSpeed = 0.5f;  // Thời gian để dao phi vào vòng

    public int knc = 0;
    
    bool Active = true;
    Tween circleTween;

    public AudioSource kvsk;
    public AudioSource kvsw;
    public AudioSource kvf;

    public int requset = 5;
    public int count = 0;

    public float timer = 15;

    public void UnActive()
    {
        circleTween.Kill(); 
         Active = false; 
    }
    void Start()
    {
        // Quay vòng tròn với DOTween vô hạn
        circleTween = circle.DORotate(Vector3.forward * 360, 1 / (rotationSpeed / 360), RotateMode.FastBeyond360)
              .SetEase(Ease.Linear)
              .SetLoops(-1, LoopType.Restart);

        CloneKnife();

        StartCoroutine(WaitEnd());

        UIManager.I.ShowRequest(true, "Radish request: " + requset, count.ToString());
        UIManager.I.ShowTime(true, (int)timer);
    }

    public void ChargeCount()
    {
        count++;
        count = Mathf.Clamp(count, 0, 5);
        UIManager.I.ChangeCount(count.ToString());

        if(count >= requset)
        {
            UnActive();
        }
    }

    Tween knifeTween;
    public void CloneKnife()
    {
        var knifeClone = Instantiate(knifeSample);
        knifeClone.name = knc.ToString();
        knc ++;
        knife = knifeClone.GetComponent<Knift>();
        knifeTween = knifeClone.transform.DOMove(knifePosition.position, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
         
        });
        UIAnimation.Fade(knifeClone.GetComponent<SpriteRenderer>(), 0.2f, true, 1);
    }

    void Update()
    {
        if (Active && !UIManager.I._pause)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);

            if (Input.GetMouseButtonDown(0) && knife != null)
            {
                if (!IsPointerOverButton())
                {
                    ThrowKnife();
                    knife = null;
                }

            }

            if ( timer < 0)
            {
                Active = false;
            }
        }

       

        // Nhấn chuột để ném dao
       
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => !Active);

        UIManager.I.buttonActive.DeActive();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            if (count >= requset)
            {
                UIManager.I.endGameLoad.Win();
            }
            else
            {
                UIManager.I.endGameLoad.Lose();
            }
           
        });
       
    }
    private bool IsPointerOverButton()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };

                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.CompareTag("CanClick") == false)
                    {
                        return true; // Có button dưới ngón tay
                    }
                }
            }
        }
        return false; // Không có button dưới ngón tay
    }

    void ThrowKnife()
    {
        knifeTween.Kill();
        knife.GetComponent<BoxCollider2D>().isTrigger = false;
        knife.ThrowKnife(circle, throwSpeed, CloneKnife);
        // Di chuyển dao vào vị trí của vòng tròn
        //knife.DOMove(circle.position + knife.transform.up*0.8f, throwSpeed).SetEase(Ease.Linear).OnComplete(() =>
        //{
        //    // Gắn dao vào vòng xoay sau khi va chạm
        //    knife.GetComponent<Knift>().StartCutter();
        //    knife.SetParent(circle);
        //    isThrown = false;
        //});
    }
}
