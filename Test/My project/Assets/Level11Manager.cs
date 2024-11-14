using DG.Tweening;
using Dreamteck.Splines.Primitives;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Level11Manager : PingPongMover
{
    public GameObject hand;
    public GameObject huongDan;
    public bool Win = false;
    public AudioSource Sound;
    public SpriteRenderer mun;

    public float targetY = 3;
    public float targetX = 3;
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
            DOVirtual.DelayedCall(3f, () =>
            {
                UIManager.I.endGameLoad.Win();

            });

        }
        else
        {

            DOVirtual.DelayedCall(1.3f, () =>
            {
                UIManager.I.endGameLoad.Lose();

            });

        }


    }

    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);


        if (CheckWinZone())
        {
            huongDan.gameObject.SetActive(false);
            MoveTotarget();
            //DOVirtual.DelayedCall(0.1f, () =>
            //{
            //    if (Setting.SoundCheck())
            //    {
            //        Sound.Play();
            //    }
            //});
            Debug.Log("Win");
            Win = true;

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
            UIManager.I.Haptic();
            Debug.Log("Lose");
        }

    }
    public void MoveTotarget()
    {
        move.DOLocalMoveX(targetX, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {

            move.DOMoveY(targetY, 1f).OnComplete(() =>
            {
                UIAnimation.Fade(mun, 1f, false, 1);
                if (Setting.SoundCheck())
                {
                    Sound.Play();
                }
            });
        });
        
    }
}
