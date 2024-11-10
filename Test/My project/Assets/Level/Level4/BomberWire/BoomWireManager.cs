using DG.Tweening;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoomWireManager : MonoBehaviour
{
    public List<BombWire> BombWireList;
    int currentWireID = 0;
    public float timer = 15;

    public TMP_Text timeTick;
    bool End = false;
    bool Win = false;

    public AudioSource catday;
    public AudioSource bomNo;

    public GameObject bom;
    public SkeletonAnimation skeletonAnimation;
    public List<string> anim;
    // Start is called before the first frame update
    void Start()
    {
        if (skeletonAnimation != null && skeletonAnimation.SkeletonDataAsset != null)
        {
            // Lấy dữ liệu Skeleton Data từ Skeleton Data Asset
            SkeletonData skeletonData = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true);

            // Duyệt qua tất cả các animation trong Skeleton Data và in tên của chúng
            foreach (var animation in skeletonData.Animations)
            {
                anim.Add(animation.Name);
            }
        }
        StartCheck();
        for (int i = 1; i < BombWireList.Count; i++)
        {
            BombWireList[i].move.gameObject.SetActive(false);
        }
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
    private void Update()
    {
        if (!End)
        {
            timer -= Time.deltaTime;
            timeTick.text = ((int)timer).ToString();
            UIManager.I.ChangeTime((int)timer);

            if (timer < 0)
            {
                bom.transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    if (Setting.SoundCheck())
                    {
                        bomNo.Play();

                    }
                    skeletonAnimation.AnimationState.SetAnimation(0, anim[0], false);
                    UIManager.I.Haptic();
                });

                End = true;
                BombWireList[currentWireID].Kill();
                StopCoroutine("WaitWireClick");
            }
        }
    }
    void StartCheck()
    {
        BombWireList[currentWireID].StartPingPongMovement();
        BombWireList[currentWireID].move.gameObject.SetActive(true);

        StartCoroutine(WaitWireClick());
    }

    IEnumerator WaitWireClick()
    {
        yield return new WaitUntil(() => BombWireList[currentWireID].clicked);

        if (BombWireList[currentWireID].CheckWinZone())
        {
            BombWireList[currentWireID].move.gameObject.SetActive(false);
            BombWireList[currentWireID].save.SetActive(false);
            BombWireList[currentWireID].dut.SetActive(true);
            if (currentWireID < BombWireList.Count - 1)
            {

                if (Setting.SoundCheck())
                {
                    catday.Play();

                }
                
                currentWireID++;
                StartCheck();
            }
            else
            {
                catday.Play();
                Win = true;
                End = true;
                Debug.Log("Win");
            }

        }
        else
        {
            bom.transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                if (Setting.SoundCheck())
                {
                    bomNo.Play();
                    
                }
                skeletonAnimation.AnimationState.SetAnimation(0, anim[0], false);
                UIManager.I.Haptic();
            });
           
            End = true;
            Debug.Log("Lose");
        }

    }
}
