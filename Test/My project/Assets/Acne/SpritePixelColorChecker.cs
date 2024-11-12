﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpritePixelColorChecker : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // SpriteRenderer của đối tượng
    public Color targetColor;
    public Camera cam;// Màu sắc cần kiểm tra
    public bool isDragging = false;
    public GameObject pixelgreen;
    public GameObject pixelred;

    float timer;
    private Camera mainCamera;
    private Vector3 offset;
    public GameObject kim;
    public Transform daukim;

    public Transform diemParent;

    bool Win;
    bool End;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            offset = kim.transform.position - GetMouseWorldPosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        timer += Time.deltaTime;
        if (isDragging)
        {
            kim.transform.position = GetMouseWorldPosition() + offset;

            CheckColider();
            // Lấy vị trí chuột trong không gian thế giới
            Vector2 mouseWorldPosition = daukim.position;//cam.ScreenToWorldPoint(Input.mousePosition);

            // Kiểm tra xem chuột có nằm trong sprite không
            if (spriteRenderer.bounds.Contains(mouseWorldPosition))
            {
                // Lấy texture của sprite
                Texture2D texture = spriteRenderer.sprite.texture;

                // Chuyển đổi vị trí chuột từ không gian thế giới sang không gian của sprite
                Vector2 localPosition = (Vector2)mouseWorldPosition - (Vector2)spriteRenderer.transform.position;

                // Tính toán lại localPosition dựa trên scale của sprite
                Vector2 scaledLocalPosition = new Vector2(
                    localPosition.x / spriteRenderer.transform.lossyScale.x,
                    localPosition.y / spriteRenderer.transform.lossyScale.y
                );

                // Tính vị trí pixel trong texture dựa vào tỷ lệ và pivot của sprite
                Vector2 pivot = spriteRenderer.sprite.pivot;
                Vector2 spriteSize = spriteRenderer.sprite.rect.size;

                int pixelX = Mathf.FloorToInt((scaledLocalPosition.x * spriteRenderer.sprite.pixelsPerUnit + pivot.x) * (texture.width / spriteSize.x));
                int pixelY = Mathf.FloorToInt((scaledLocalPosition.y * spriteRenderer.sprite.pixelsPerUnit + pivot.y) * (texture.height / spriteSize.y));


                // Đảm bảo tọa độ pixel nằm trong phạm vi của texture
                if (pixelX >= 0 && pixelX < texture.width && pixelY >= 0 && pixelY < texture.height)
                {
                    // Lấy màu của pixel
                    Color pixelColor = texture.GetPixel(pixelX, pixelY);

                    // Kiểm tra màu pixel với targetColor
                    if (IsColorSimilar(pixelColor, targetColor, 0.2f))//(pixelColor == targetColor)
                    {
                        if (timer > 0.1f)
                        {
                            var pixel = Instantiate(pixelgreen, mouseWorldPosition, Quaternion.identity);
                            pixel.transform.SetParent(diemParent);
                            timer = 0;
                        }

                        Debug.Log("Pixel có màu mong muốn!");
                    }
                    else
                    {
                        if (timer > 0.1f)
                        {
                            VeraNgoai();
                            var pixel = Instantiate(pixelred, mouseWorldPosition, Quaternion.identity);
                            pixel.transform.SetParent(diemParent);
                            timer = 0;
                        }
                        
                        Debug.Log("Pixel không có màu mong muốn.");
                    }
                }
            }
            else
            {
                if (timer > 0.1f)
                {
                    VeraNgoai();
                    var pixel = Instantiate(pixelred, mouseWorldPosition, Quaternion.identity);
                    pixel.transform.SetParent(diemParent);
                    timer = 0;
                }

            }
        }
        // Kiểm tra nếu người dùng click chuột trái

    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // Đảm bảo khoảng cách Z so với camera không đổi
        return Camera.main.ScreenToWorldPoint(mousePos); // Chuyển đổi vị trí chuột sang vị trí thế giới
    }
    bool IsColorSimilar(Color color1, Color color2, float tolerance)
    {
        float distance = Mathf.Sqrt(
            Mathf.Pow(color1.r - color2.r, 2) +
            Mathf.Pow(color1.g - color2.g, 2) +
            Mathf.Pow(color1.b - color2.b, 2) +
            Mathf.Pow(color1.a - color2.a, 2)
        );
        return distance <= tolerance;
    }

    public List<Diem> diems;
    public LayerMask layer;
    public int Complete = 0;
    void CheckColider()
    {
        
        Vector2 mousePosition = daukim.position;// cam.ScreenToWorldPoint(Input.mousePosition);

        // Thực hiện raycast từ vị trí con trỏ chuột
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 10,layer);

        // Kiểm tra nếu raycast trúng một collider
        if (hit.collider != null)
        {
            Debug.Log("Found Diem at objecxt: " + hit.collider.name);
            Diem diemFound = diems.FirstOrDefault(x => x.points.Contains(hit.collider.gameObject));// ==  || x.point2 == hit.collider.gameObject);
            if (diemFound != null)
            {
                //int index = diems.IndexOf(diemFound);
                //Debug.Log("Found Diem at index: " + index);
                int indexOfPoint = diemFound.points.IndexOf(hit.collider.gameObject);
                diemFound.pointActive[indexOfPoint] = true;

                if (CompleteAllPointInDiem(diemFound))
                {
                    diemFound.GoGo();
                    diems.Remove(diemFound);
                    if(diems.Count <= 0)
                    {
                        Win = true;
                    }
                }
            }

        }
    }
    bool CompleteAllPointInDiem(Diem diemFound)
    {
        for (int i = 0; i < diemFound.pointActive.Count; i++)
        {
            if(diemFound.pointActive[i] == false)
                return false;
        }
        return true;
    }

    void VeraNgoai()
    {
        End = true;

    }
}

[Serializable]
public class Diem
{
    public List<GameObject> points;
    public List<bool> pointActive;
    //public GameObject point1;
    //public bool point1Actice;
    //public GameObject point2;
    //public bool point2Actice;
    public bool actice;

    public GameObject ob;
    public Transform target;

    public void GoGo()
    {
        if(!actice)
        {
            actice = true;
            ob.transform.DOMove(target.position, 1f);
            ob.GetComponent<SpriteRenderer>().DOFade(0, 1f);
        }

    }
}
