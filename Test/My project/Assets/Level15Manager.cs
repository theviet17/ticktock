using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Level15Manager : MonoBehaviour
{
    public List<SamePoint> points = new List<SamePoint>();
    bool Win;
    bool End;
    public float timer = 30;

    private void Start()
    {
        UIManager.I.ShowRequest(true, "Defferent request: " + points.Count,  ( 4 - points.Count ).ToString());
        UIManager.I.ShowTime(true, (int)timer);

        StartCoroutine(WaitEnd());
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);
        UIManager.I.buttonActive.DeActive();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            if (Win)
            {
                UIManager.I.endGameLoad.Win();
            }
            else
            {
                UIManager.I.endGameLoad.Lose();
            }

        });

    }


    private void Update()
    {
       
        if (!End && !UIManager.I._pause)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);
            if (Input.GetMouseButtonDown(0) && !IsPointerOverButton())
            {
                CheckColider();
            }
        }

        if ( timer < 0)
        {
            End = true;
        }


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
    public AudioSource source;
    void CheckColider()
    {

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Thực hiện raycast từ vị trí con trỏ chuột
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 10);

        // Kiểm tra nếu raycast trúng một collider
        if (hit.collider != null)
        {
            Debug.Log("Found Diem at objecxt: " + hit.collider.name);
            SamePoint diemFound = points.FirstOrDefault(x => x.points.Contains(hit.collider.gameObject));// ==  || x.point2 == hit.collider.gameObject);
            
            if (diemFound != null)
            {
                
                diemFound.GoGo();
                if (Setting.SoundCheck())
                {
                    source.Play();
                }
                UIManager.I.Haptic();
                points.Remove(diemFound);
                UIManager.I.ChangeCount((4 - points.Count).ToString());
                if (points.Count <= 0)
                {
                    Win = true;
                    End = true;
                }
                
            }

        }
    }
}

[Serializable]
public class SamePoint
{
    public List<GameObject> points;
    public bool actice;

    public SpriteRenderer cirle1;
    public SpriteRenderer cirle2;

    public void GoGo()
    {
        if (!actice)
        {
            actice = true;
            cirle1.gameObject.SetActive(true);
            cirle2.gameObject.SetActive(true);

            UIAnimation.Fade(cirle1, 0.5f, true, 1);
            UIAnimation.Fade(cirle2, 0.5f, true, 1);
        }

    }
}
