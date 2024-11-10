using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class ButtonClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Setting.SoundCheck())
        {
            AudioSource audioSource = UIManager.I.sourcePool.GetSoundFromPool().GetComponent<AudioSource>();
            audioSource.gameObject.SetActive(true);
            audioSource.clip = UIManager.I.click;
            audioSource.Play();
        }
        StartCoroutine(0.1f.Tweeng((p) => transform.localScale = p, transform.localScale, Vector3.one * 1.1f));
        //transform.DOScale(1.1f, 0.1f);

    }

    public void OnPointerUp(PointerEventData eventData)
    {

        StartCoroutine(0.1f.Tweeng((p) => transform.localScale = p, transform.localScale, Vector3.one));
        //transform.DOScale(1, 0.1f);


    }

}
public static class Extns
{
    public static IEnumerator Tweeng(this float duration,
        System.Action<float> var, float aa, float zz)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Mathf.SmoothStep(aa, zz, t));
            yield return null;
        }

        var(zz);
    }
    public static IEnumerator Tweeng(this float duration,
        System.Action<float> var, float aa, float zz, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Mathf.SmoothStep(aa, zz, curve.Evaluate(Mathf.SmoothStep(0f, 1f, t))));
            yield return null;
        }

        var(zz);
    }


    public static IEnumerator Tweeng(this float duration,
        System.Action<Vector3> var, Vector3 aa, Vector3 zz)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Vector3.Lerp(aa, zz, Mathf.SmoothStep(0f, 1f, t)));
            yield return null;
        }

        var(zz);
    }

    public static IEnumerator Tweeng(this float duration,
        System.Action<Vector3> var, Vector3 aa, Vector3 zz, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Vector3.Lerp(aa, zz, curve.Evaluate(Mathf.SmoothStep(0f, 1f, t))));
            yield return null;
        }

        var(zz);
    }
}

