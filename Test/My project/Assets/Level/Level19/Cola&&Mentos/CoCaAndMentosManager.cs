using DG.Tweening;
using Dreamteck.Splines.Primitives;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoCaAndMentosManager : PingPongMover
{
    // Start is called before the first frame update
    void Start()
    {
        StartPingPongMovement();
        StartCoroutine(WaitClick());
        StartCoroutine(WaitEnd());
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);

        if (Win)
        {
            DOVirtual.DelayedCall(3.5f, () =>
            {
                UIManager.I.endGameLoad.Win();

            });

        }
        else
        {

            DOVirtual.DelayedCall(3.5f, () =>
            {
                UIManager.I.endGameLoad.Lose();

            });

        }


    }

    // Update is called once per frame
    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);

        huongDan.gameObject.SetActive(false);
        if (CheckWinZone())
        {
            colaColider.isTrigger = true ;
            mentosRigbody.transform.DOLocalMoveX(0, 0.1f).OnComplete(() =>
            {
                colaColider.isTrigger = true;
                mentosRigbody.constraints |= RigidbodyConstraints2D.FreezePositionX;
                mentosRigbody.gravityScale = 1;
            });
            End = true;
            Win = true;

            DOVirtual.DelayedCall(1.3f, () => NoCoCa());
            
            Debug.Log("Win");

        }
        else
        {
            End = true ;
            mentosRigbody.gravityScale = 1;
            colaColider.isTrigger = false;
            Debug.Log("Lose");
        }

    }

    public AudioSource coca;
    void NoCoCa()
    {
        ColaVisual.gameObject.SetActive(false);
        colaAim.GetComponent<MeshRenderer>().enabled = true;
        colaAim.AnimationState.SetAnimation(0, "anim", false);
        if (Setting.SoundCheck())
        {
            coca.Play();
        }
        UIManager.I.Haptic();
    }
    public Collider2D colaColider;
    public Rigidbody2D mentosRigbody;

    public GameObject ColaVisual;
    public SkeletonAnimation colaAim;

    public GameObject huongDan;

    public bool Win = false;
}
