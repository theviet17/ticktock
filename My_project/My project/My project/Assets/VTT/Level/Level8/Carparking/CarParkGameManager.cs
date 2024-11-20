using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarParkGameManager : MonoBehaviour
{
    //public Camera camera;
    public bool End = false;
    bool Win = false;

    public float timer = 15;
    private void Start()
    {
        StartCoroutine(WaitToAllDone());
        StartCoroutine(WaitEnd());
        UIManager.I.ShowTime(true, (int)timer);
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);
        UIManager.I.buttonActive.DeActive();
        if (Win)
        {
            DOVirtual.DelayedCall(1.3f, () =>
            {
                UIManager.I.endGameLoad.Win();

            });

        }
        else
        {

            DOVirtual.DelayedCall(1.3f, () =>
            {
                UIManager.I.endGameLoad.Lose();

            });

        }


    }
    void Update()
    {
        if (!End && !UIManager.I._pause)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverButton())
                {
                    // Lấy vị trí của con trỏ chuột trong không gian thế giới
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Thực hiện raycast từ vị trí con trỏ chuột
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                    // Kiểm tra nếu raycast trúng một collider
                    if (hit.collider != null)
                    {
                        if (Setting.SoundCheck())
                        {
                            AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                            audioSource.gameObject.SetActive(true);
                            audioSource.clip = UIManager.I.click;
                            audioSource.Play();
                        }

                        Debug.Log("Clicked on object: " + hit.collider.name);
                        hit.collider.GetComponent<Car>().MovingPaths();
                        // Thêm các xử lý cho object được click ở đây
                    }
                }
               
            }
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);

            if (timer < 0)
            {
                End = true;
            }
        }
        // Kiểm tra nếu người dùng click chuột trái
       
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
    public List<Car> cars;

    bool CheckAllCarDone()
    {
        if(cars.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < cars.Count; i++)
        {
            if( !cars[i].moveDone)
            {
                return false;
            }
        }
        return true;
    }
    IEnumerator WaitToAllDone()
    {
        yield return new WaitUntil( () =>CheckAllCarDone());

        End = true;
        Win = true;
        Debug.Log("Win");

    }
   
}
