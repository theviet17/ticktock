using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class NailHammerEffect : MonoBehaviour
{
    public Transform hammer; // Đối tượng búa
    public float totalDepth = 1.0f; // Độ sâu cần lún xuống
    public int totalHits = 60; // Số lần gõ
    public int currentHits = 0; // Đếm số lần đã gõ

    private Vector3 startPos; // Vị trí ban đầu của đinh
    private float depthPerHit; // Độ lún mỗi lần gõ
    public AudioSource hit;

    public float timer = 15;
    bool End = false;

    public ObjectPool objectPool;

    void Start()
    {
        startPos = transform.position;
        depthPerHit = totalDepth / totalHits;

        UIManager.I.ShowRequest(true, "Hit request: " + totalHits, currentHits.ToString());
        UIManager.I.ShowTime(true, (int)timer);

        StartCoroutine(WaitEnd());
    }

    void Update()
    {
        if(!End && !UIManager.I._pause)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);
            if (Input.GetMouseButtonDown(0) && currentHits < totalHits && timer >= 0)
            {
                if (!IsPointerOverButton() )
                {
                    // Mỗi lần gõ búa, đinh lún xuống một khoảng
                    transform.DOMoveY(transform.position.y - depthPerHit, 0.1f);
                    hammer.DOMoveY(hammer.position.y - depthPerHit, 0.1f);
                    objectPool.transform.DOMoveY(objectPool.transform.position.y - depthPerHit, 0.1f);

                    // Đếm số lần gõ
                    currentHits++;
                    UIManager.I.ChangeCount(currentHits.ToString());

                    // Tạo hiệu ứng gõ búa bằng DOTween
                    HammerHitAnimation();
                }
              
            }
        }
        
        if(currentHits >=  totalHits || timer < 0)
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
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);
        UIManager.I.buttonActive.DeActive();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            if (currentHits >= totalHits)
            {
                UIManager.I.endGameLoad.Win();
            }
            else
            {
                UIManager.I.endGameLoad.Lose();
            }

        });

    }

    Vector3 hammerHeightEuler = new Vector3(0, 0, -40);
    Vector3 hammerLowEuler = new Vector3(0, 0, -7.9f);

    Tween hammerTween;
    private void HammerHitAnimation()
    {
        hammerTween.Kill();
        hammerTween = hammer.DOLocalRotate(hammerLowEuler, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (Setting.SoundCheck())
            {
                hit.Play();
            }
            UIManager.I.Haptic();
            Animator fx = objectPool.GetObjectFromPool().GetComponent<Animator>();
            fx.gameObject.SetActive(true);
            fx.SetTrigger("spark");
            DOVirtual.DelayedCall(0.3f, () => objectPool.ReturnObjectToPool(fx.gameObject));



            hammer.DOLocalRotate(hammerHeightEuler, 0.1f).SetEase(Ease.OutQuad);
        });

        //Sequence hammerSequence = DOTween.Sequence();

        //// Vị trí khi búa gõ xuống và trở lại vị trí ban đầu
        //Vector3 hammerHitPos = hammer.position - new Vector3(0, 0.2f, 0); // Điều chỉnh độ lún của búa tùy ý

        //// Hiệu ứng di chuyển búa xuống và lên
        //hammerSequence.Append(hammer.DOMove(hammerHitPos, 0.05f).SetEase(Ease.OutQuad));
        //hammerSequence.Append(hammer.DOMove(hammer.position, 0.05f).SetEase(Ease.InQuad));
    }
 
}
