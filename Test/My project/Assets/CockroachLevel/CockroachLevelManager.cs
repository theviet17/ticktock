using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockroachLevelManager : PingPongMover
{
    // Start is called before the first frame update
    public GameObject spray;
    public Transform sprayTarget;
    public float duration = 1;
    public AudioSource xitgian;
    public SpriteRenderer fx;
    bool Win =false;
    void Start()
    {
        StartPingPongMovement();
        StartCoroutine(WaitClick());
        StartCoroutine(WaitEnd());
    }

    // Update is called once per frame
    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);

        if (CheckWinZone())
        {
            MoveToTarget();
            Win = true;
            Debug.Log("Win");

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
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);

        if (Win)
        {
            DOVirtual.DelayedCall(2.3f, () =>
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
    void MoveToTarget()
    {
        spray.transform.DOMove(sprayTarget.position, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            UIAnimation.Fade(fx, 0.2f,true, 0.7f);
            if (Setting.SoundCheck())
            {
                xitgian.Play();
            }
        });
        
        spray.transform.DORotate(sprayTarget.eulerAngles  , duration).SetEase(Ease.OutQuad);
    }
}
