using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhichDien : MonoBehaviour
{
    // Danh sách các điểm đích
    public List<Transform> targetPositions;

    // Khoảng cách cho phép để tự động di chuyển đến đích khi thả chuột
    public float snapDistance = 1f;

    // Thời gian để di chuyển tới đích nếu đạt khoảng cách
    public float moveDuration = 0.5f;

    // Cờ để kiểm tra nếu đang kéo
    private bool isDragging = false;

    public bool daCam;

    public ElectricWireManager wireManager;

    public Camera main;
    public bool islargePichDien;

    GameObject connectTedODien;
    public LineRenderer conChuot;
    public SpriteRenderer conChuotSprite;
    public SpriteRenderer daucam;
    public GameObject chuotDen;
    public AudioSource sound;
    private void Update()
    {
        conChuot.positionCount = 2;
        conChuot.SetPosition(0, conChuot.transform.position - new Vector3(0,0.7f,0));
        conChuot.SetPosition(1 , gameObject.transform.position);


        if (isDragging)
        {
  
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z; 
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = worldPosition;
        }


        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;


            GameObject nearestTarget = targetPositions[0].gameObject;
            float nearestDistance = Mathf.Infinity;

            foreach (var target in targetPositions)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (distanceToTarget < nearestDistance)
                {
                    nearestDistance = distanceToTarget;
                    nearestTarget = target.gameObject;
                }
            }


            if (nearestDistance <= snapDistance)
            {
                if( !wireManager.CheckODien(nearestTarget, islargePichDien))
                { 

                    transform.DOMove(nearestTarget.transform.position, moveDuration).SetEase(Ease.OutQuad);
                    daucam.gameObject.SetActive(false);
                    conChuotSprite.sortingOrder = 2;

                    daCam = true;
                    chuotDen.gameObject.SetActive(true);
                    if (Setting.SoundCheck())
                    {
                        sound.Play();
                    }
                    connectTedODien = nearestTarget;
                    wireManager.ChargeOdien(nearestTarget ,  islargePichDien);
                }
               
            }
            wireManager.CheckDOne();
        }
    }

    private void OnMouseDown()
    {
        if (!wireManager.End)
        {
            isDragging = true;
            conChuotSprite.sortingOrder = 3;

            daCam = false;
            chuotDen.gameObject.SetActive(false);
            sound.Stop();

            daucam.gameObject.SetActive(true);

            if (connectTedODien != null)
            {
                wireManager.DisChargeOdien(connectTedODien, islargePichDien);
                connectTedODien = null;
            }
        }
       
    }
}
