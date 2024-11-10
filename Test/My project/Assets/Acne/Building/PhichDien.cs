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

    public ElectricWireManager wireManager;
    private void Update()
    {
  
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
                if( !wireManager.CheckODien(nearestTarget))
                { 
                    transform.DOMove(nearestTarget.transform.position, moveDuration).SetEase(Ease.OutQuad);
                    wireManager.ChargeOdien(nearestTarget);
                }
               
            }
        }
    }

    private void OnMouseDown()
    {

        isDragging = true;
    }
}
