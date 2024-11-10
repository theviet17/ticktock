using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpriteCutter;
using DG.Tweening;
using System;
using Unity.VisualScripting;
using MoreMountains.NiceVibrations;

public class Knift : MonoBehaviour
{
    public LayerMask layer;
    public Transform point1;
    public Transform point2;
    public float splitForce = 5f;
    public KnifeThrowManager knifeThrowManager;
    public bool playAudio = false;

    // Start is called before the first frame update

    public void StartCutter()
    {
       
    }
    public void PlaySound(int id)
    {
        if (Setting.SoundCheck())
        {
            if (!playAudio)
            {

                if (id == 1)
                {
                    knifeThrowManager.kvsw.Play();
                }
                else if (id == 2)
                {
                    knifeThrowManager.kvf.Play();
                }
                playAudio = true;
            }
            if (id == 0)
            {
                knifeThrowManager.kvsk.Play();
            }
        }
       


    }
 

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
                Vector2 forceDirection1 = new Vector2(-0.5f, 1);
                Vector2 forceDirection2 = new Vector2(0.5f, 1);

                output.firstSideGameObject.transform.SetParent(null);
                
                Rigidbody2D newRigidbody1 = output.firstSideGameObject.AddComponent<Rigidbody2D>();
                Rigidbody2D newRigidbody2 = output.secondSideGameObject.AddComponent<Rigidbody2D>();
                newRigidbody1.AddForce(forceDirection1 * splitForce, ForceMode2D.Impulse);
                newRigidbody1.GetComponent<Collider2D>().isTrigger = true;
                newRigidbody2.AddForce(forceDirection2 * splitForce, ForceMode2D.Impulse);
                newRigidbody2.GetComponent<Collider2D>().isTrigger = true;
                //secondSideGameObject.velocity = output.firstSideGameObject.GetComponent<Rigidbody2D>().velocity;
            }
        }
    }
    bool HitCounts(RaycastHit2D hit)
    {
        return (hit.transform.GetComponent<SpriteRenderer>() != null ||
                 hit.transform.GetComponent<MeshRenderer>() != null);
    }

    public bool Active = true;
    public Tween throwTween = null;
    public Color color;

    public void ThrowKnife(Transform circle, float throwSpeed, Action action)
    {
        
        // Di chuyển dao vào vị trí của vòng tròn
        throwTween = transform.DOMove(circle.position - transform.up * 3f, throwSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            UIManager.I.Haptic();
            PlaySound(1);
            StartCutter();
            transform.SetParent(circle);
            circle.transform.DOMoveY(circle.position.y + 0.1f, 0.05f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            circle.GetComponent<SpriteRenderer>().DOColor(color , 0.05f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            action.Invoke();
            //isThrown = false;
        });
    }
    
    public float bounceForce = 5f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "knife")
        {
            UIManager.I.Haptic();
            Active = false;
            PlaySound(0);
            knifeThrowManager.UnActive();
            Debug.Log("Collision with knife" + gameObject.name + "with" + collision.gameObject.name);
            throwTween.Kill();
            BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
            box.isTrigger = true;
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            transform.SetParent(null);

            Vector2 bounceDirection = -collision.contacts[0].normal; // Ngược hướng va chạm
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            rb.gravityScale = 10;

            transform.DORotate(new Vector3(0,0,180),0.1f).SetLoops(10, LoopType.Incremental);

        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Cucai" && Active)
        {
            knifeThrowManager.ChargeCount();
            PlaySound(2);
            collision.gameObject.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.transform.SetParent(null);
            collision.GetComponent<BoxCollider2D>().enabled = false;
            for (int i = 0; i < collision.transform.childCount; i++)
            {
                GameObject piece = collision.transform.GetChild(i).gameObject;
                piece.GetComponent<SpriteRenderer>().enabled = true;

                Vector2 forceDirection = new Vector2(i == 0 ?-0.2f:0.2f, i == 2?1:0.5f);

                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                rb.gravityScale = 7;
                rb.AddForce(forceDirection * UnityEngine.Random.Range(0 ,splitForce), ForceMode2D.Impulse);

                
            }
        }
    }
  
}
