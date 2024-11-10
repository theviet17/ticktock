using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityBuildingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sample;
    public GameObject move;
    public Transform startPoint;
    public Transform endPoint;
    public float moveDuration = 2f;
    protected Tween moveTween;
    public List<GameObject> Builder;
    public int count;

    public GameObject winZone;
    public Transform winZoneLimit1;
    public Transform winZoneLimit2;
    //public List<CityBuildingManager()>
    void Start()
    {
        BlockStarting();
    }
    void BlockStarting()
    {
        int a = Random.Range(0, 10);
        if(a < 5)
        {
            move = Instantiate(sample, startPoint.position, Quaternion.identity);
            StartPingPongMovement(move.transform, endPoint.position.x);
        }
        else
        {
            move = Instantiate(sample, endPoint.position, Quaternion.identity);
            StartPingPongMovement(move.transform, startPoint.position.x);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckWinZone(move.transform);
            moveTween.Kill();
            move.GetComponent<Rigidbody2D>().gravityScale = 1;

            DOVirtual.DelayedCall(1, () =>
            {
                count++;
                Builder.Add(move);
                winZone.transform.DOMoveX(move.transform.position.x, 0);
                DisableGravity();
                MovementObject();
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    BlockStarting();
                });
               
            });
        }
        Debug.Log(CheckWinOrLose());
    }
    public void StartPingPongMovement(Transform move, float x)
    {
        moveTween.Kill();
        //move.position = new Vector3(startPoint.position.x, move.position.y, move.position.z);
        moveTween = move.DOMoveX(x, moveDuration)
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);

    }
    public List<GameObject> moves;
    public void MovementObject()
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (count < 5)
            {
                moves[0].transform.DOMoveY(moves[0].transform.position.y + 1, 0.5f);

            }
            else
            {
                moves[i].transform.DOMoveY(moves[i].transform.position.y + 1, 0.5f);
            }
           
        }
        
       
    }
    public void DisableGravity()
    {
        if (count >= 4)
        {
            Destroy(Builder[count - 4].GetComponent<Rigidbody2D>());
            Builder[count - 4].transform.eulerAngles = Vector3.zero;
        }
    }

    public bool CheckWinZone(Transform move)
    {
        Debug.LogWarning("Check");

        float xMin = Mathf.Min(winZoneLimit1.position.x, winZoneLimit2.position.x);
        float xMax = Mathf.Max(winZoneLimit1.position.x, winZoneLimit2.position.x);

        if (move.position.x >= xMin && move.position.x <= xMax)
        {
            Debug.LogWarning("true");
            move.DOMoveX(winZone.transform.position.x, 0.5f);
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool CheckWinOrLose()
    {
        if(count >= 20)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < Builder.Count; i++)
            {
                if (Builder[i].transform.eulerAngles.z > 90)
                {
                    return false ;
                }
                else if (Builder[i].transform.eulerAngles.z < -90){
                    return false ;
                }
            }
        }
        return true;
    }


}
