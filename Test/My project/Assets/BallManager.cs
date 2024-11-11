using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallManager : MonoBehaviour
{
   public List<GameObject> gameObjects;
    public List<Transform> transforms;


    public GameObject balll;
    [Header("Slider")]
    public GameObject point1;
    public GameObject point2;
    public GameObject Do;
    public GameObject zone;

   // public 
    public void Start()
    {
        var dd = new List<GameObject>(gameObjects);
        GameObject go = dd[Random.Range(0, dd.Count)];
        dd.Remove(go);
        go.transform.position = transforms[0].position;
        go.gameObject.SetActive(true);
        Debug.Log(go.name);

        GameObject go2 = dd[Random.Range(0, dd.Count)];
        dd.Remove(go2);
        go2.transform.position = transforms[1].position;
        go2.gameObject.SetActive(true);
        Debug.Log(go2.name);

        GameObject go3 = dd[Random.Range(0, dd.Count)];
        dd.Remove(go3);
        go3.transform.position = transforms[2].position;
        go3.gameObject.SetActive(true);
        Debug.Log(go3.name);
    }

    public void FixedUpdate()
    {
        float y = balll.transform.position.y;
        y = Mathf.Clamp(y , 0f, 8f);
        float tyle = y / 8f;
        Do.transform.position = Vector3.Lerp(point1.transform.position, point2.transform.position, tyle);



}
}
