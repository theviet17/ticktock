using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterMelonLevelManager : MonoBehaviour
{
    public KinfeWaterMelonLevel kinfe;
    public GameObject huongDan;
    public bool Win = false;
    public AudioSource Sound;
    void Start()
    {
        kinfe.StartPingPongMovement();
        StartCoroutine(WaitClick());
        StartCoroutine(WaitEnd());
    }

    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => kinfe.End);

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
    // Update is called once per frame

    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => kinfe.clicked);

      
        if (kinfe.CheckWinZone())
        {
            huongDan.gameObject.SetActive(false);
            kinfe.MoveTotarget();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                if (Setting.SoundCheck())
                {
                    Sound.Play();
                }
            });
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
}
