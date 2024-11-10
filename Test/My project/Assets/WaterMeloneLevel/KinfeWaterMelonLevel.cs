using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpriteCutter;

public class KinfeWaterMelonLevel : PingPongMover
{
   
    public float targetY = 3;

    public Rigidbody2D nua1;
    public Transform nua1Target;

    public Rigidbody2D nua2;
    public Transform nua2Target;

    public void MoveTotarget()
    {
        move.DOMoveY(targetY, 0.3f).OnComplete(() =>
        {
            ChiaNua();
        });
    }

    public void ChiaNua()
    {
        nua1.transform.DOMove(nua1Target.position, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            nua1.simulated = true;
        });
        nua2.transform.DOMove(nua2Target.position, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            nua2.simulated = true;
        });
    }
    
   
}
