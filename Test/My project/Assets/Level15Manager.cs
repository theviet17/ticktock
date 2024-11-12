using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level15Manager : MonoBehaviour
{
    public List<SamePoint> points = new List<SamePoint>();
    bool Win;
    bool End;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckColider();
        }
    }

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
                points.Remove(diemFound);
                if (points.Count <= 0)
                {
                    Win = true;
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
