using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level20Manager : MonoBehaviour
{
    public Transform pointSpawnLimit1;
    public Transform pointSpawnLimit2;

    public List<Sprite> eggSprite;

    public int idTrueEgg;

    public GameObject fixedEgg;
    public List<SpriteRenderer> fixedEggs;

    public float minDelayTime;
    public float maxDelayTime;

    public float maxGravity;
    public float minGravity;

    public List<GameObject> eggs;


    public GameObject sample;

    private void Start()
    {
        idTrueEgg = Random.Range(0, eggSprite.Count);
        fixedEggs = fixedEgg.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var egg in fixedEggs)
        {
            egg.sprite = eggSprite[idTrueEgg];
        }

        StartCoroutine(ReSpawnEgg());

        string nameEgg = idTrueEgg == 0 ? "Yellow eggs " : "Brown eggs ";
        UIManager.I.ShowRequest(true, nameEgg + "request: " + eggRequest, currentEgg.ToString());
        UIManager.I.ShowTime(true, (int)timer);

        StartCoroutine(WaitEnd());
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);

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
    bool isDragging = false;
    Vector3 offset;

    public int eggRequest = 10; // Số lần gõ
    public int currentEgg = 0;

    public bool End;
    public bool Win;
    public float timer;

    private void Update()
    {
        if (!End)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                offset = gameObject.transform.position - GetMouseWorldPosition();
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            };
        }

        if ( timer < 0)
        {
            EndGame();
        }
        
    }
    private void FixedUpdate()
    {
      
        if (isDragging && !End)
        {
            var NewPos = GetMouseWorldPosition() + offset;
            NewPos.x = Mathf.Clamp(NewPos.x, -5, 5);
            gameObject.transform.position = NewPos;

        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        mousePos.y = 0;// Đảm bảo khoảng cách Z so với camera không đổi
        return Camera.main.ScreenToWorldPoint(mousePos); // Chuyển đổi vị trí chuột sang vị trí thế giới
    }
    IEnumerator ReSpawnEgg()
    {
        while (true)
        {
            float randomDelayTime = Random.Range(minDelayTime, maxDelayTime);

            yield return new WaitForSeconds(randomDelayTime);
            randomDelayTime = Random.Range(minDelayTime, maxDelayTime);
            SpawnEgg();
        }
    }
    void SpawnEgg()
    {
        float randomX = Random.Range(pointSpawnLimit1.position.x, pointSpawnLimit2.position.x);
        Vector2 SpawnPosition = new Vector2(randomX, pointSpawnLimit1.position.y);

        float randomGravityScale = Random.Range(minGravity, maxGravity);

        GameObject egg = Instantiate(sample, SpawnPosition, Quaternion.identity);
        eggs.Add(egg);

        int randomEgg = Random.Range(0, eggSprite.Count);

        egg.name = randomEgg.ToString();
        egg.GetComponent<SpriteRenderer>().sprite = eggSprite[randomEgg];
        egg.GetComponent<Rigidbody2D>().gravityScale = randomGravityScale;
        //Tween tween = egg.transform.DOMoveY(pointSpawnLimit2.position.y , randomMoveDuriation).SetEase(Ease.OutQuad);

        //eggs.Add(egg);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == idTrueEgg.ToString())
        {
            collision.GetComponent<PolygonCollider2D>().isTrigger = false;
            
           if(collision.transform.position.y < -2.164)
            {
                collision.name = "Fall";
                collision.GetComponent<PolygonCollider2D>().isTrigger = true;
            }
        }
        else if(collision.name != "Fall" && collision.transform.position.y > -2.164)
        {
            //Debug.Log(false);
            if (Setting.SoundCheck())
            {
                AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                audioSource.gameObject.SetActive(true);
                audioSource.clip = UIManager.I.wrong;
                audioSource.Play();
            }
            UIManager.I.Haptic();
            EndGame();
        }
    }
    public AudioSource trungRoiVaoRo;
    public AudioSource trungVo;
    public void OpenEgg(GameObject egg)
    {
        if (Setting.SoundCheck())
        {
            trungRoiVaoRo.Play();
        }
        UIManager.I.Haptic();
        currentEgg++;
        UIManager.I.ChangeCount(currentEgg.ToString());

        egg.transform.SetParent(transform);
        Destroy(egg.GetComponent<Rigidbody2D>());
        egg.GetComponent<PolygonCollider2D>().isTrigger = true;
        var newPos = egg.transform.localPosition + new Vector3(0, Random.Range(-0.1f, 0.5f), 0);
        egg.transform.DOLocalMoveY(newPos.y, 0.3f).SetEase(Ease.OutBack);

        if(currentEgg >= eggRequest)
        {
            Win = true;
            EndGame();
        }
    }
    
    void EndGame()
    {
        End = true;
        for(int i = 0; i < eggs.Count; i++)
        {
            Rigidbody2D rb = eggs[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Destroy(rb);
            }
        }
        StopCoroutine("ReSpawnEgg");
    }
}
