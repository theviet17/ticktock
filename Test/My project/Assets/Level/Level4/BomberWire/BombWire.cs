using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWire : PingPongMover2
{
    // Start is called before the first frame update
    public GameObject save;
    public GameObject dut;
    void Start()
    {
        //StartPingPongMovement();
        //StartCoroutine(WaitClick());
    }

    // Update is called once per frame
    IEnumerator WaitClick()
    {
        yield return new WaitUntil(() => clicked);

        //if (CheckWinZone())
        //{
        //    Debug.Log("Win");
        //}
        //else
        //{
        //    Debug.Log("Lose");
        //}
    }
   
}
