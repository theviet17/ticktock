using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Level16Manager : MonoBehaviour
{
    public RuoiController ruoiController1;
    public RuoiController ruoiController2;
    public int ruoiRequest = 10;
    public int ruoiKill = 0;

    bool Win;
    bool End;
    public float timer = 30;

    public List<GameObject> ruois;
    private void Start()
    {
        UIManager.I.ShowRequest(true, "Flies request: " + ruoiRequest, ruoiKill.ToString());
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
        if (!End && !UIManager.I._pause )
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);
            if (Input.GetMouseButtonDown(0) && !IsPointerOverButton())
            {
                CheckColider();
            }
        }

        if (timer < 0)
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
    public SpriteRenderer vot;
    Tween votTween;
    public AudioSource dap;

    private void CheckColider()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Thực hiện raycast từ vị trí con trỏ chuột
        //RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 10);
        float radius = 0.3f;

        // Thực hiện CircleCast tại vị trí của mousePosition với bán kính và khoảng cách
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, radius, Vector2.zero, 10);

        // Kiểm tra nếu raycast trúng một collider
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            RuoiController ruoiController = hit.collider.GetComponent<RuoiController>();

            if (ruoiController != null)
            {
                if (Setting.SoundCheck())
                {
                    dap.Play();
                }
                UIManager.I.Haptic();
                ShowVot(ruoiController.transform.position);
                ruoiKill++;
                UIManager.I.ChangeCount(ruoiKill.ToString());

                ruoiController.RuoiDeath();

                var newRuoi = Instantiate(ruois[Random.Range(0, ruois.Count)]);
                ruoiController.ruoi = newRuoi.transform;
                ruoiController.StartFlying();

                if(ruoiKill >= ruoiRequest)
                {
                    Win= true;
                    End =true;
                }
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

    public void ShowVot(Vector3 ruoiPos)
    {
        vot.transform.parent.gameObject.SetActive(true);

        vot.transform.parent.position = ruoiPos;

        vot.DOFade(1, 0);
        votTween.Kill();
        votTween = vot.DOFade(0, 0.5f).OnComplete(() =>
        {
            vot.transform.parent.gameObject.SetActive(false);
        });
    }

}
