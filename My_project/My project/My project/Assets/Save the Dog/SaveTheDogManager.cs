using DG.Tweening;
using Spine.Unity;
using System.Collections;
using UnityEngine;

public class SaveTheDogManager : PingPongMover
{
    // Start is called before the first frame update
    void Start()
    {
        StartPingPongMovement();
        MovingFire();
        StartCoroutine(WaitClick());
         
        line1.startWidth = customWidth;
        line1.endWidth = customWidth;
        line1.sortingOrder = 0;

        line2.startWidth = customWidth;
        line2.endWidth = customWidth;
        line1.sortingOrder = 0;

        StartCoroutine(WaitEnd());
    }

    // Update is called once per frame
    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);

        fireMove.Kill();
        if (CheckWinZone())
        {
            MoveToTarget();
            Win = true;

            Debug.Log("Win");

        }
        else
        {
            //MoveToTarget();
            Debug.Log("Lose");
        }

    }
    void MoveToTarget()
    {
        if (Setting.SoundCheck())
        {
            xitNuoc.Play();
        }
        spray.AnimationState.SetAnimation(0, "spray", false);
        DOVirtual.DelayedCall(0.6f, () =>
        {
            fire.gameObject.SetActive(false);
        });
    }
    [Header("Fire extinguisher")]
    public LineRenderer line1;
    public float customWidth = 5.0f; // Kích thước tùy chỉnh vượt quá 1
    public Transform linePoint1;
    public Transform linePoint2;
    public Transform water;

    [Header("Fire")]
    public Transform fire;
    public float fireMoveduration;
    public float timeDelayToEnd = 1;
    public LineRenderer line2;
    public Transform fireTarget;

    Tween fireMove;

    public SkeletonAnimation spray;
    public AudioSource xitNuoc;
    public SkeletonAnimation exp;
    public AudioSource no;

    bool Win;
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);

        UIManager.I.Haptic();
        if (Win)
        {
            DOVirtual.DelayedCall(2.3f, () =>
            {
                UIManager.I.endGameLoad.Win();
            });
            
        }
        else
        {
            if (timer < 0)
            {
                exp.AnimationState.SetAnimation(0, "explosion", false);
                if (Setting.SoundCheck())
                {
                    no.Play();
                }
                Debug.Log("No banh");

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
            }
            DOVirtual.DelayedCall(2.3f, () =>
            {
                UIManager.I.endGameLoad.Lose();
            });

            
        }
        

    }

    private void FixedUpdate()
    {
        line1.positionCount = 2;
        line1.SetPosition(0, linePoint1.position);
        line1.SetPosition(1, linePoint2.position);

        line2.positionCount = 2;
        line2.SetPosition(1, fire.position);
        line2.SetPosition(0, fireTarget.position);
    }
    void MovingFire()
    {
        fireMove =  fire.DOMove(fireTarget.position, fireMoveduration).OnComplete(() =>
        {
            DOVirtual.DelayedCall(timeDelayToEnd, () =>
            {
                moveTween.Kill();
                Debug.Log("Lose");
            });
        });
    }
}
