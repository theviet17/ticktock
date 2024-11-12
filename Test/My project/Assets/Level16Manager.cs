using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level16Manager : MonoBehaviour
{
    public RuoiController ruoiController1;
    public RuoiController ruoiController2;

    public List<GameObject> ruois;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckColider();
        }
    }

    private void CheckColider()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Thực hiện raycast từ vị trí con trỏ chuột
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 10);

        // Kiểm tra nếu raycast trúng một collider
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            RuoiController ruoiController = hit.collider.GetComponent<RuoiController>();
            if (ruoiController != null)
            {
                ruoiController.RuoiDeath();

                var newRuoi = Instantiate(ruois[Random.Range(0, ruois.Count)]);
                ruoiController.ruoi = newRuoi.transform;
                ruoiController.StartFlying();
            }

            //if (hit.collider.gameObject.)
            //Debug.Log("Found Diem at objecxt: " + hit.collider.name);
            //SamePoint diemFound = points.FirstOrDefault(x => x.points.Contains(hit.collider.gameObject));// ==  || x.point2 == hit.collider.gameObject);
            //if (diemFound != null)
            //{
            //    diemFound.GoGo();
            //    points.Remove(diemFound);
            //    if (points.Count <= 0)
            //    {
            //        Win = true;
            //    }

                //}

        }
    }

}
