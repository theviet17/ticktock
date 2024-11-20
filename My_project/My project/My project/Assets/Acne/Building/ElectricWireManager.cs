using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Reflection;

public class ElectricWireManager : MonoBehaviour
{
    public List<GameObject> odiens = new List<GameObject>();
    public List<bool> trangthaiodien = new List<bool>();
    bool Win;
    public bool End;
    public float timer = 15;
    private void Start()
    {
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
            
        }

        if (timer < 0)
        {
            End = true;
        }

    }

    public void ChargeOdien(GameObject odien , bool isLargePichDien)
    {
        int index = odiens.IndexOf(odien);
        trangthaiodien[index] = true;
        if (isLargePichDien )
        {
            if(index != odiens.Count-1)
            {
                int nextIndex = index + 1;
                trangthaiodien[nextIndex] = true;
            }
        }

    }
    public void DisChargeOdien(GameObject odien, bool isLargePichDien)
    {
        int index = odiens.IndexOf(odien);
        trangthaiodien[index] = false;
        if (isLargePichDien)
        {
            if (index != odiens.Count - 1)
            {
                int nextIndex = index + 1;
                trangthaiodien[nextIndex] = false;
            }
        }
    }


    public bool CheckODien(GameObject odien, bool isLargePichDien)
    {
        if (!isLargePichDien)
        {
            int index = odiens.IndexOf(odien);
            return trangthaiodien[index];
        }
        else
        {
            int index = odiens.IndexOf(odien);
            int nextIndex = index + 1;
            if(nextIndex >= odiens.Count)
            {
                return false;
            }
            if(trangthaiodien[index] || trangthaiodien[nextIndex])
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
    public List<PhichDien> phichs;
    public void CheckDOne()
    {
        for (int i = 0; i < odiens.Count; i++)
        {
            if (!phichs[i].daCam)
            {
                Debug.Log("Lose");
                return;
            }
        }
        Win = true;
        End = true;
    }
       
}
