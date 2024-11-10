using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectPool : MonoBehaviour
{
    public GameObject main;
    public List<GameObject> pool;
    public int poolCount = 0;

    private void Awake()
    {
        for (int i = 0; i < poolCount; i++)
        {
            GameObject clone = Instantiate(main);
            clone.transform.parent = transform;
            pool.Add(clone);
            clone.gameObject.SetActive(false);
        }
    }
    public GameObject GetObjectFromPool(Func<GameObject, bool> tracking = null)
    {
        if (tracking == null)
        {
            tracking = CheckActive;
        }
        foreach (GameObject obj in pool)
        {
            if (tracking(obj))
            {
                return obj;
            }
        }
        GameObject clone = Instantiate(main);
        clone.transform.parent = transform;
        pool.Add(clone);
        clone.gameObject.SetActive(false);
        return clone;
    }
    public GameObject GetSoundFromPool(Func<GameObject, bool> tracking = null)
    {
        if (tracking == null)
        {
            tracking = CheckSound;
        }
        foreach (GameObject obj in pool)
        {
            if (tracking(obj))
            {
                return obj;
            }
        }
        GameObject clone = Instantiate(main);
        clone.transform.parent = transform;
        pool.Add(clone);
        clone.gameObject.SetActive(false);
        return clone;
    }
    public bool CheckActive(GameObject obj)
    {
        return !obj.activeInHierarchy;
    }
    public bool CheckSound(GameObject obj)
    {
        AudioSource audio = obj.GetComponent<AudioSource>();
        return !audio.isPlaying;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        //obj.SetActive(false);

        obj.SetActive(false);
        obj.transform.SetParent(gameObject.transform);
    }
}
