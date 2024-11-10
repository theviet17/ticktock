using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpriteCutter;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CuttingLevelManager : MonoBehaviour
{
    public Transform knife;
    public Transform targetx;
    public float xDuration;

    public Transform targety;
    float startY;
    public float yDuration;


    public int request;
    public TMP_Text request_Txt;
    public TMP_Text count_Txt;

    public float timer = 0;
    bool End = false;



    // Start is called before the first frame update
    void Start()
    {
        startY = knife.position.y;
        KnifeMoveX();

        request_Txt.text = "Number of requests: " + request;

        UIManager.I.ShowRequest(true, "Piece request: " + request, 0.ToString());
        timer = xDuration;
        UIManager.I.ShowTime(true, (int)timer);
        StartCoroutine(WaitEnd());
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);

        DOVirtual.DelayedCall(1.3f, () =>
        {
            if (cutter.Count >= request)
            {
                UIManager.I.endGameLoad.Win();
            }
            else
            {
                UIManager.I.endGameLoad.Lose();
            }

        });

    }
    public AudioSource catting;

    // Update is called once per frame
    void Update()
    {
       
        if (!End)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverButton())
                {
                    if (Setting.SoundCheck())
                    {
                        catting.Play();
                    }
                    
                    KnifeMoveY();
                }
            }
            if ( timer < 0)
            {
                End = true;
            }
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
    Tween ytween;
    Tween xTween;
    void KnifeMoveX()
    {
        knife.DOMoveX(targetx.position.x, xDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            if(cutter.Count >= request)
            {
                Debug.Log("Win");
            }
            else
            {
                Debug.Log("Lose");
            }
        });
    }
    void KnifeMoveY()
    {
        ytween.Kill();
        ytween = knife.DOMoveY(targety.position.y, yDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            LinecastCut(knife.transform.GetChild(0).transform.position, knife.transform.GetChild(1).transform.position);
            knife.DOMoveY(startY, yDuration).SetEase(Ease.Linear);
        });
        
    }
    public LayerMask layer;
    public List<Transform> cutter;

   

    void LinecastCut(Vector2 lineStart, Vector2 lineEnd)
    {
        List<GameObject> gameObjectsToCut = new List<GameObject>();
        RaycastHit2D[] hits = Physics2D.LinecastAll(lineStart, lineEnd, layer);
        foreach (RaycastHit2D hit in hits)
        {
            if (HitCounts(hit))
            {
                gameObjectsToCut.Add(hit.transform.gameObject);
            }
        }

        foreach (GameObject go in gameObjectsToCut)
        {
            SpriteCutterOutput output = SpriteCutter.Cut(new SpriteCutterInput()
            {
                lineStart = lineStart,
                lineEnd = lineEnd,
                gameObject = go,
                gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
            });

            if (output != null && output.secondSideGameObject != null)
            {
                Vector2 forceDirection1 = new Vector2(-1, 0.5f);
                Vector2 forceDirection2 = new Vector2(1, 0.5f);

                output.firstSideGameObject.transform.SetParent(null);

                
                cutter.Add(output.firstSideGameObject.transform);
                UIManager.I.ChangeCount(cutter.Count.ToString());
                count_Txt.text = cutter.Count.ToString();
                MoveCutter();
                //Rigidbody2D newRigidbody1 = output.firstSideGameObject.AddComponent<Rigidbody2D>();
                //Rigidbody2D newRigidbody2 = output.secondSideGameObject.AddComponent<Rigidbody2D>();
                //newRigidbody1.AddForce(forceDirection1 * splitForce, ForceMode2D.Impulse);
                //newRigidbody2.AddForce(forceDirection2 * splitForce, ForceMode2D.Impulse);
                //secondSideGameObject.velocity = output.firstSideGameObject.GetComponent<Rigidbody2D>().velocity;
            }
        }
    }
    bool HitCounts(RaycastHit2D hit)
    {
        return (hit.transform.GetComponent<SpriteRenderer>() != null ||
                 hit.transform.GetComponent<MeshRenderer>() != null);
    }
    [Header("Cutter")]
    public float cutterMoveX;
    void MoveCutter()
    {
        for (int i = 0; i< cutter.Count; i++)
        {
            cutter[i].DOMoveX(cutter[i].position.x + cutterMoveX, yDuration);
        }
    }
}
