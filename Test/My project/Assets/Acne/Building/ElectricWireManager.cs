using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ElectricWireManager : MonoBehaviour
{
    public Dictionary<GameObject, bool> odiens = new Dictionary<GameObject, bool>();

   
    public void ChargeOdien(GameObject odien)
    {
        if (odiens.ContainsKey(odien))
        {
            odiens[odien] = true;
        }
    }

  
    public bool CheckODien(GameObject odien)
    {
        if (odiens.TryGetValue(odien, out bool isCharged))
        {
            return isCharged;
        }
        return false;
    }
}
