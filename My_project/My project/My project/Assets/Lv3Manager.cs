using DG.Tweening;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv3Manager : PingPongMover
{
    public SkeletonAnimation skeletonAnimation;
    public List<string> anim;
    public AudioSource upro;
    
    public bool Win = false;

   
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
        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;
        StartPingPongMovement();
        StartCoroutine(WaitClick());

        StartCoroutine(WaitEnd());

        
    }
    public void FixedUpdate()
    {
       
    }

    // Update is called once per frame

    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);


        if (CheckWinZone())
        {
            PlayAnim(true);
            Win = true;
            Debug.Log("Win");

        }
        else
        {
            PlayAnim(false);
            Debug.Log("Lose");
        }

    }
    void PlayAnim(bool win)
    {
        if (win)
        {
            PlayAnimation(anim[3]);
            DOVirtual.DelayedCall(1.8f,() => 
            {
                if (Setting.SoundCheck())
                {
                    upro.Play();
                }
                UIManager.I.Haptic();

            });
        }
        else
        {
            if (Setting.SoundCheck())
            {
                AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
                audioSource.gameObject.SetActive(true);
                audioSource.clip = UIManager.I.wrong;
                audioSource.Play();
            }
            PlayAnimation(anim[2], true);
        }
        
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);
        UIManager.I.buttonActive.DeActive();
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
    public void PlayAnimation(string animationName, bool loop = false)
    {
        // Đổi animation
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
       
    }
    void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        // Kiểm tra nếu animation hiện tại là endAnimation
        if (trackEntry.Animation.Name == anim[3])
        {
            // Chuyển về animation mặc định (loop)
            skeletonAnimation.AnimationState.SetAnimation(0, anim[1], true);
        }
        //if (trackEntry.Animation.Name == anim[2])
        //{
        //    // Chuyển về animation mặc định (loop)
        //    skeletonAnimation.AnimationState.SetAnimation(0, anim[0], true);
        //}
    }
}
