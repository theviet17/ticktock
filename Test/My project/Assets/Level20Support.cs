using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level20Support : MonoBehaviour
{
    public Level20Manager Level20Manager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
         Debug.Log("TrueEgg");
        if (collision.name == Level20Manager.idTrueEgg.ToString() && collision.transform.position.y > -3.62f) 
        {
            Level20Manager.OpenEgg(collision.gameObject);
           
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
        
    //}
}
