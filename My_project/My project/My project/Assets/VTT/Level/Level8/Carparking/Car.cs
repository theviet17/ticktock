using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Car : MonoBehaviour
{
    public CarParkGameManager CarParkGameManager;
    public List<Transform> points;
    public float moveDuration = 1f; // Thời gian di chuyển giữa các điểm

    private int currentPointIndex = 0;
    public bool moveDone = false;
    // Start is called before the first frame update
    public void GetPaths(List<Transform> allpoint)
    {
        points = allpoint;
    }
    Tween cartween;
    public void MovingPaths()
    {
        if (currentPointIndex >= points.Count) 
        {
           
            return;
        } 

        // Lấy vị trí và hướng của điểm tiếp theo
        Transform targetPoint = points[currentPointIndex];

        // Di chuyển đến điểm tiếp theo với DOTween
        cartween = transform.DOMove(targetPoint.position, moveDuration)
            .OnComplete(() =>
            {
                // Khi đến nơi, đặt transform.up của xe bằng transform.up của điểm
                transform.up = targetPoint.up;

                // Chuyển sang điểm tiếp theo
                currentPointIndex++;

                // Gọi lại để di chuyển tiếp nếu còn điểm
                if (currentPointIndex < points.Count)
                {
                    MovingPaths();
                }
                else
                {
                    moveDone = true;
                }
            });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Car>()!= null)
        {
            GetComponent<Rigidbody2D>().simulated = false;
            CarParkGameManager.End = true;
            if (Setting.SoundCheck())
            {
                AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                audioSource.gameObject.SetActive(true);
                audioSource.clip = UIManager.I.wrong;
                audioSource.Play();
            }
            UIManager.I.Haptic();
            Debug.Log("Lose");
            cartween.Kill();    
        } 
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Car>() != null)
        {
            GetComponent<Rigidbody2D>().simulated = false;
            CarParkGameManager.End = true;
            if (Setting.SoundCheck())
            {
                AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                audioSource.gameObject.SetActive(true);
                audioSource.clip = UIManager.I.wrong;
                audioSource.Play();
            }
            UIManager.I.Haptic();
            Debug.Log("Lose");
            cartween.Kill();
        }
    }
}
