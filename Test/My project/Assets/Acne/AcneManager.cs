using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.XR;

public class AcneManager : PingPongMover
{
    void Start()
    {
        StartPingPongMovement();
        StartCoroutine(WaitClick());
    }

    // Update is called once per frame
    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);

        if (CheckWinZone())
        {
            MoveToTarget();
            Debug.Log("Win");

        }
        else
        {
            MoveToTarget(false);
            Debug.Log("Lose");
        }

    }
    public float xTarget = 0;
    public float yTargetWrong = 3.3f;
    public float yTargetRight = 2.8f;

    public float xduration = 0.1f;
    public float yduration = 1;
    void MoveToTarget(bool win = true)
    {
        var yStart = move.localPosition.y;
        if (win)
        {

            move.DOLocalMoveX(xTarget, xduration).SetEase(Ease.Linear).OnComplete(() =>
            {
                move.DOLocalMoveY(yTargetRight, yduration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    //move.DOLocalMoveY(yStart, yduration).SetEase(Ease.OutQuad).SetDelay(0.2f);
                });
            });
        }
        else
        {
            move.DOLocalMoveY(yTargetWrong, yduration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                //hand.DOLocalMoveY(yStart, yduration).SetEase(Ease.OutQuad);
            });
        }

    }
}
