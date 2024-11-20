using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuoiController : MonoBehaviour
{
    public SplineFollower follower;
    public Transform ruoi;
    bool active = false;

    void Start()
    {
        follower = GetComponent<SplineFollower>();
        follower.SetPercent(0); // Bắt đầu từ điểm đầu của spline
        StartFlying();

        //follower.follow = true; // Bật chế độ di chuyển
    }

    void Update()
    {
        double percent = follower.GetPercent();

        if(active)
        {
            // Kiểm tra nếu percent đạt 1 (cuối spline) hoặc 0 (đầu spline)
            if (percent >= 1)
            {
                follower.SetPercent(0); // Quay lại đầu spline
            }
            else if (percent <= 0)
            {
                follower.SetPercent(1); // Quay lại cuối spline
            }
            follower.follow = true;
        }
       

        // Tiến hành di chuyển tiếp trên spline
        if(follower.follow)
        {
            ruoi.position = transform.position;
            // Tính toán góc quay dựa trên hướng của spline
            float angle = transform.eulerAngles.z;
            //Debug.Log(angle);

            if (angle > 70 && angle < 210)
            {
                ruoi.transform.eulerAngles = new Vector3(0, 180); // Lật trục y 180 độ và quay quanh trục z theo góc -angle
            }
            else
            {
                ruoi.transform.eulerAngles = new Vector3(0, 0);
            }
        }
       
    }
    public void StartFlying()
    {
        follower.SetPercent(Random.Range(0.0f, 1.0f));
        follower.follow = false;
        ruoi.DOMove(transform.position, 0.5f).OnComplete(() =>{
            active = true;
            follower.follow = true;
        });
    }
    public void RuoiDeath()
    {
        active = false;
        //follower.SetPercent(0);
        follower.follow = false;

        ruoi.GetComponent<MeshRenderer>().enabled = false;
        ruoi.GetChild(0).gameObject.SetActive(true);
        var temp = ruoi.GetChild(0).GetComponent<SpriteRenderer>();
        DOVirtual.DelayedCall(2f, () =>
        {
            UIAnimation.Fade(temp, 0.5f, false, 1);
        });
        
    }
}
