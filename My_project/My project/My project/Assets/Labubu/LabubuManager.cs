using DG.Tweening;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabubuManager : PingPongMover
{
    // Start is called before the first frame update
    public SkeletonAnimation skeletonAnimation;
    public List<string> anim;
    public GameObject saveZone;
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

        StartPingPongMovement();
        StartCoroutine(WaitClick());
        StartCoroutine(WaitEnd());
    }

    // Update is called once per frame
    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);

        saveZone.gameObject.SetActive(false);
        skeletonAnimation.AnimationState.SetAnimation(0, anim[0], false);
        skeletonAnimation.timeScale = 0.7f;
        if (CheckWinZone())
        {
            MoveToTarget();
            Win = true;
            Debug.Log("Win");

        }
        else
        {
            MoveToTarget(false);
            Debug.Log("Lose");
        }

    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);

        DOVirtual.DelayedCall(4f, () =>
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
    public Transform hand;
    public float xTarget = 0;
    public float yTargetWrong = 3.3f;
    public float yTargetRight = 2.8f;
    public GameObject labubu;
    public float xduration = 0.1f;
    public float yduration = 1;

    bool Win = false;

    void MoveToTarget(bool win = true)
    {
        var yStart = hand.localPosition.y;
        if (win)
        {
           
            move.DOLocalMoveX(xTarget, xduration).SetEase(Ease.Linear).OnComplete(() =>
            {
             
                UIManager.I.Haptic();
                hand.DOLocalMoveY(yTargetRight, yduration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    labubu.transform.SetParent(hand);
                    hand.DOLocalMoveY(yStart, yduration).SetEase(Ease.OutQuad).SetDelay(0.2f);
                });
            });
        }
        else
        {
            hand.DOLocalMoveY(yTargetWrong, yduration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                hand.DOLocalMoveY(yStart, yduration).SetEase(Ease.OutQuad);
                if (Setting.SoundCheck())
                {
                    AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                    audioSource.gameObject.SetActive(true);
                    audioSource.clip = UIManager.I.wrong;
                    audioSource.Play();
                }
            });
        }

    }



}
