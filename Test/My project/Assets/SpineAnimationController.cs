using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimationController : MonoBehaviour
{
    // Tham chiếu đến component Skeleton Animation
    public SkeletonAnimation skeletonAnimation;

    void Start()
    {
        // Gán animation mặc định
        skeletonAnimation.AnimationState.SetAnimation(0, "run", true);
    }

    public void PlayAnimation(string animationName, bool loop)
    {
        // Đổi animation
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }
}
