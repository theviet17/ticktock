using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallManager : MonoBehaviour
{
   public List<GameObject> gameObjects;
    public List<Transform> transforms;


    public BallController balll;

    [Header("Slider")]
    public GameObject point1;
    public GameObject point2;
    public GameObject Do;
    public GameObject zone;

    public List<float> tall;
    public float current;

    public float timer = 30;
    bool End = false;
    bool Win = false;

    // public 
    public void Start()
    {
        var dd = new List<GameObject>(gameObjects);
        GameObject go = dd[Random.Range(0, dd.Count)];
        dd.Remove(go);
        go.transform.position = transforms[0].position;
        go.gameObject.SetActive(true);
        Debug.Log(go.name);

        tall.Add( (gameObjects.IndexOf(go) + 1) *0.7f);
        current = tall[0];
        UpdateSafeZone();

        GameObject go2 = dd[Random.Range(0, dd.Count)];
        dd.Remove(go2);
        go2.transform.position = transforms[1].position;
        go2.gameObject.SetActive(true);
        Debug.Log(go2.name);
        tall.Add((gameObjects.IndexOf(go2) + 1)*0.7f);

        GameObject go3 = dd[Random.Range(0, dd.Count)];
        dd.Remove(go3);
        go3.transform.position = transforms[2].position;
        go3.gameObject.SetActive(true);
        Debug.Log(go3.name);
        tall.Add((gameObjects.IndexOf(go3) + 1)*0.7f);

        UIManager.I.ShowTime(true, (int)timer);

        StartCoroutine(WaitEnd());
    }
    IEnumerator WaitEnd()
    {
        yield return new WaitUntil(() => End);
        UIManager.I.buttonActive.DeActive();
        DOVirtual.DelayedCall(1.3f, () =>
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
    private void Update()
    {
        if (!End && !UIManager.I._pause)
        {
            timer -= Time.deltaTime;
            UIManager.I.ChangeTime((int)timer);

            if (current == tall[0])
            {
                if (balll.transform.position.z >= 1)
                {
                    current = tall[1];
                    UpdateSafeZone();
                }
            }
            else if (current == tall[1])
            {
                if (balll.transform.position.z >= 5.5f)
                {
                    current = tall[2];
                    UpdateSafeZone();
                }
            }
            else if (current == tall[2])
            {
                if (balll.transform.position.z >= 10.5f)
                {
                    current = 1000;
                    End = true;
                    Win = true;
                    balll.End = true;
                    Debug.Log("Win");
                }
            }
        }

        if (timer < 0)
        {
            End = true;
            balll.End = true;
        }
       
    }
    void UpdateSafeZone()
    {
        balll.pass = false;
        float y = current;
        y = Mathf.Clamp(y, 0f, 8*0.7f);
        float tyle = y / 8 * 0.7f;
        Vector3 newPos = Vector3.Lerp(point1.transform.position, point2.transform.position, tyle);

        zone.transform.DOMoveY(newPos.y, 0.3f);
    }

    public void FixedUpdate()
    {
        float y = balll.transform.position.y;
        y = Mathf.Clamp(y , 0f, 8 * 0.7f);
        float tyle = y / 8 * 0.7f;
        Do.transform.position = Vector3.Lerp(point1.transform.position, point2.transform.position, tyle);

}
}
