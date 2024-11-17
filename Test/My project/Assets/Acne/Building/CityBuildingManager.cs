using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class CityBuildingManager : MonoBehaviour
{
    // Start is called before the first frame update
   
    public GameObject sample;
    public GameObject tang1;
    public GameObject tanggiua;
    public GameObject tangmai;
    public GameObject move;
    public Transform startPoint;
    public Transform endPoint;
    public float moveDuration = 2f;
    protected Tween moveTween;
    public List<GameObject> Builder;
    public int count;
    public int requset = 20;

    public GameObject winZone;
    public Transform winZoneLimit1;
    public Transform winZoneLimit2;

    public GameObject loseZone;
    public Transform loseZoneLimit1;
    public Transform loseZoneLimit2;

    public GameObject winZone2;
    public List<Sprite> alls;

    public AudioSource perfect;
    public AudioSource drop;
    public AudioSource sound;

    public float timer;
    bool Win;
    bool End;

    

    //public List<CityBuildingManager()>
    void Start()
    {
        moves.Add(Camera.main.gameObject);
        BlockStarting();
        UIManager.I.ShowRequest(true, "Floors request: " + requset, Builder.Count.ToString());
        UIManager.I.ShowTime(true, (int)timer);

        StartCoroutine(WaitEnd());
        //move.AddCom
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);
        UIManager.I.buttonActive.DeActive();
        DOVirtual.DelayedCall(2f, () =>
        {
            if (Win)
            {
                UIManager.I.endGameLoad.Win();
            }
            else
            {
                UIManager.I.endGameLoad.Lose();
            }
            Camera.main.transform.position = new Vector3(0, 1, -10);
        });

    }
    void BlockStarting()
    {
        int a = Random.Range(0, 10);
        sample = count== 0? tang1 : count == requset -1? tangmai: tanggiua;
        if (a < 5)
        {
            move = Instantiate(sample, startPoint.position, Quaternion.identity);
            if(sample == tanggiua)
            {
                move.GetComponent<SpriteRenderer>().sprite = alls[Random.Range(0, alls.Count)];
            }
           
            StartPingPongMovement(move.transform, endPoint.position.x);

            winZoneLimit1 = winZone2.transform.GetChild(0);
                winZoneLimit2 = winZone2.transform.GetChild(1);
        }
        else
        {
            move = Instantiate(sample, endPoint.position, Quaternion.identity);
            if (sample == tanggiua)
            {
                move.GetComponent<SpriteRenderer>().sprite = alls[Random.Range(0, alls.Count)];
            }
            StartPingPongMovement(move.transform, startPoint.position.x);

            winZoneLimit1 = winZone.transform.GetChild(0);
            winZoneLimit2 = winZone.transform.GetChild(1);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!End && !UIManager.I._pause)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);
            bool perfected =false;
            if (Input.GetMouseButtonDown(0) && !IsPointerOverButton() && timer >= 0)
            {
                if (CheckWinZone(move.transform))
                {
                    sound = perfect;
                    perfected = true;
                }
                else
                {
                    sound = drop;
                }
                moveTween.Kill();
                move.GetComponent<Rigidbody2D>().gravityScale = 1;
                GameObject temp = move.transform.GetChild(0).gameObject;
                temp.transform.SetParent(null);
                
                if (CheckLoseZone(move.transform) && count>0)
                {
                    DOVirtual.DelayedCall(1, () =>
                    {
                        End = true;
                    });
                }
                else
                {
                    CheckBuildingCrash(temp, perfected);
                }

               
            }
            if (!CheckWinOrLose())
            {
                End = true;
            }
        }

        if ( timer < 0)
        {
            End = true;
        }
        
    }
    void CheckBuildingCrash(GameObject cau, bool perfected)
    {
        count++;
        float timeDelay = count < requset ? 0.9f : 1.5f;
        Transform raycast = move.transform.GetChild(move.transform.childCount - 1);
        StartCoroutine(BuildingCrash(raycast, cau , perfected));
        //DOVirtual.DelayedCall(timeDelay, () =>
        //{
        //    if (Setting.SoundCheck())
        //    {
        //        sound.Play();
        //    }
        //    UIManager.I.Haptic();

        //    Destroy(cau);

        //    Builder.Add(move);
        //    winZone.transform.DOMoveX(move.transform.position.x, 0);
        //    winZone2.transform.DOMoveX(move.transform.position.x, 0);
        //    loseZone.transform.DOMoveX(move.transform.position.x, 0);
        //    DisableGravity();
        //    MovementObject();

        //    UIManager.I.ChangeCount(Builder.Count.ToString());

        //    if (Builder.Count >= requset)
        //    {
        //        End = true;
        //        Win = true;

        //        for (int i = 0; i < Builder.Count; i++)
        //        {
        //            Destroy(Builder[i].GetComponent<Rigidbody2D>());
        //        }
        //    }
        //    else
        //    {
        //        DOVirtual.DelayedCall(0.5f, () =>
        //        {
        //            BlockStarting();
        //        });
        //    }

        //});
    }
    public ObjectPool Pool_perfect;
    public ObjectPool Pool_vacham;
    IEnumerator BuildingCrash(Transform raycastPoint ,GameObject cau, bool perfected)
    {
        bool check = false;
        while (!check)
        {
            Debug.Log("Raycast trúng: " + "0");
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            
            origin = raycastPoint;
            Vector2 direction = Vector2.down;

            RaycastHit2D hit = Physics2D.Raycast(origin.position, direction, 0.01f);

            // Kiểm tra nếu ray trúng một vật thể
            if (hit.collider != null && hit.collider.transform != raycastPoint.parent)
            {
                check = true;
                Debug.Log("Raycast trúng: " + hit.collider.name);
                if(hit.collider.name == "Quad" && Builder.Count != 0)
                {
                    End = true;
                    
                }
                else
                {
                    if (Setting.SoundCheck())
                    {
                        sound.Play();
                    }
                    if (perfected)
                    {
                        Animator fx_perfect = Pool_perfect.GetObjectFromPool().GetComponent<Animator>();

                        fx_perfect.gameObject.SetActive(true);
                        fx_perfect.SetTrigger("perfect");
                        
                        DOVirtual.DelayedCall(0.7f, () => Pool_perfect.ReturnObjectToPool(fx_perfect.gameObject));

                        fx_perfect.gameObject.transform.position = origin.position + new Vector3(-2,2,0);
                    }
                    Animator fx = Pool_vacham.GetObjectFromPool().GetComponent<Animator>();

                    fx.gameObject.SetActive(true);
                    fx.SetTrigger("vacham");
                    DOVirtual.DelayedCall(0.7f, () => Pool_vacham.ReturnObjectToPool(fx.gameObject));

                    fx.gameObject.transform.position = origin.position;


                    UIManager.I.Haptic();

                    Destroy(cau);

                    Builder.Add(move);
                    winZone.transform.DOMoveX(move.transform.position.x, 0);
                    winZone2.transform.DOMoveX(move.transform.position.x, 0);
                    loseZone.transform.DOMoveX(move.transform.position.x, 0);
                    DisableGravity();
                    MovementObject();

                    UIManager.I.ChangeCount(Builder.Count.ToString());

                    if (Builder.Count >= requset)
                    {
                        End = true;
                        Win = true;

                        for (int i = 0; i < Builder.Count; i++)
                        {
                            Destroy(Builder[i].GetComponent<Rigidbody2D>());
                        }
                    }
                    else
                    {
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            BlockStarting();
                        });
                    }
                }
               
                
            }
        }
    }
    public Transform origin;
    //private void OnDrawGizmos()
    //{
    //    Vector2 originPos = new Vector2(origin.position.x, origin.position.y);

    //    // Hướng ngược trục Y
    //    Vector2 direction = Vector2.down;

    //    // Màu ray
    //    Gizmos.color = Color.blue;

    //    // Vẽ ray
    //    Gizmos.DrawLine(originPos, originPos + direction * 0.5f);
    //}
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
    public void StartPingPongMovement(Transform move, float x)
    {
        moveTween.Kill();
        //move.position = new Vector3(startPoint.position.x, move.position.y, move.position.z);
     
        moveTween = move.DOMoveX(x, moveDuration)
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo).OnStepComplete(() =>
        {
            if (winZoneLimit1 == winZone2.transform.GetChild(0))
            {
                winZoneLimit1 = winZone.transform.GetChild(0);
                winZoneLimit2 = winZone.transform.GetChild(1);
            }
            else
            {
                winZoneLimit1 = winZone2.transform.GetChild(0);
                winZoneLimit2 = winZone2.transform.GetChild(1);
            }
            //winZoneLimit1 = winZone2.transform.GetChild(0);
            //winZoneLimit2 = winZone2.transform.GetChild(1);
        });
    }
    public List<GameObject> moves;
    public void MovementObject()
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (Builder.Count < 3)
            {
                moves[0].transform.DOMoveY(moves[0].transform.position.y + 2.821851f, 0.5f);

            }
            else
            {
                moves[i].transform.DOMoveY(moves[i].transform.position.y + 2.821851f, 0.5f);
            }
           
        }
        
       
    }
    public void DisableGravity()
    {
        if (Builder.Count >= 3)
        {
            Destroy(Builder[Builder.Count - 3].GetComponent<Rigidbody2D>());
            Builder[Builder.Count - 3].transform.eulerAngles = Vector3.zero;
        }
    }

    public bool CheckWinZone(Transform move)
    {
        Debug.LogWarning("Check");

        float xMin = Mathf.Min(winZoneLimit1.position.x, winZoneLimit2.position.x);
        float xMax = Mathf.Max(winZoneLimit1.position.x, winZoneLimit2.position.x);
        move.transform.GetChild(1).gameObject.SetActive(false);

        if (move.position.x >= xMin && move.position.x <= xMax)
        {
            Debug.LogWarning("true");
            move.DOMoveX(winZone.transform.position.x, 0.5f);
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool CheckLoseZone(Transform move)
    {
        Debug.LogWarning("Check");

        float xMin = Mathf.Min(loseZoneLimit1.position.x, loseZoneLimit2.position.x);
        float xMax = Mathf.Max(loseZoneLimit1.position.x, loseZoneLimit2.position.x);
        //move.transform.GetChild(1).gameObject.SetActive(false);

        if (move.position.x < xMin || move.position.x > xMax)
        {
            Debug.LogWarning("Drop");
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool CheckWinOrLose()
    {
        if(count >= 20)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < Builder.Count; i++)
            {
                if (Builder[i].transform.eulerAngles.z > 45 && Builder[i].transform.eulerAngles.z < 315)
                {
                    Debug.Log(Builder[i].transform.eulerAngles.z);
                    return false ;
                }

            }
        }
        return true;
    }


}
