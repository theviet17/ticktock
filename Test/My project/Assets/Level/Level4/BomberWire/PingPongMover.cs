using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
public enum MoveDirection
{
    d_x,
    d_y
}
public class PingPongMover : MonoBehaviour
{
    public MoveDirection moveDirection;

    public Transform move;
    public Transform startPoint;
    public Transform endPoint;

    public Transform winZonePoint1;
    public Transform winZonePoint2;


    public float moveDuration = 2f;


    public Tween moveTween;

    public bool clicked = false;
    public bool active = false;

    public float timer = 15;
    public bool End = false;

    private void Start()
    {
        try
        {
            UIManager.I.ShowTime(true, (int)timer);
        }
        catch
        {

        }
       
        //StartPingPongMovement();
    }
    public void Update()
    {
       
        if (!End)
        {
            if (Input.GetMouseButtonDown(0) && active)
            {
                if (!IsPointerOverButton())
                {
                    if (!clicked)
                    {
                        End = true;
                        clicked = true;
                        moveTween.Kill();
                    }
                }

            }
            timer -= Time.deltaTime;
            try
            {
                UIManager.I.ChangeTime((int)timer);
            }
            catch
            {

            }
           

            if (timer < 0)
            {
                End = true;
                moveTween.Kill();
            }
        }
    }

    private bool IsPointerOverButton()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };

                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.CompareTag("CanClick") == false)
                    {
                        return true; // Có button dưới ngón tay
                    }
                }
            }
        }
        return false; // Không có button dưới ngón tay
    }
    public bool CheckWinZone()
    {
        if(moveDirection == MoveDirection.d_x)
        {
            float xMin = Mathf.Min(winZonePoint1.position.x, winZonePoint2.position.x);
            float xMax = Mathf.Max(winZonePoint1.position.x, winZonePoint2.position.x);

            if(move.position.x >= xMin && move.position.x <= xMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            float yMin = Mathf.Min(winZonePoint1.position.y, winZonePoint2.position.y);
            float yMax = Mathf.Max(winZonePoint1.position.y, winZonePoint2.position.y);

            if (move.position.y >= yMin && move.position.y <= yMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    public void StartPingPongMovement()
    {
        active = true;
        if(moveDirection == MoveDirection.d_x)
        {
            move.position = new Vector3(startPoint.position.x, move.position.y, move.position.z);
            moveTween = move.DOMoveX(endPoint.position.x, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            move.position = new Vector3(move.position.x, startPoint.position.y, move.position.z);
            moveTween = move.DOMoveY(endPoint.position.y, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        }
        
    }

   
    private void OnDestroy()
    {

        if (moveTween != null)
        {
            moveTween.Kill();
        }
    }
}
public class PingPongMover2 : MonoBehaviour
{
    public MoveDirection moveDirection;

    public Transform move;
    public Transform startPoint;
    public Transform endPoint;

    public Transform winZonePoint1;
    public Transform winZonePoint2;


    public float moveDuration = 2f;


    private Tween moveTween;

    public bool clicked = false;
    public bool active = false;



    private void Start()
    {
        
    }
    public void Update()
    {

        if (Input.GetMouseButtonDown(0) && active)
        {
            if (!IsPointerOverButton())
            {
                if (!clicked)
                {
                    clicked = true;
                    Kill();
                }
            }

        }
    }
    public void Kill()
    {
        
        moveTween.Kill();
    }

    private bool IsPointerOverButton()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };

                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.CompareTag("CanClick") == false)
                    {
                        return true; // Có button dưới ngón tay
                    }
                }
            }
        }
        return false; // Không có button dưới ngón tay
    }
    public bool CheckWinZone()
    {
        if (moveDirection == MoveDirection.d_x)
        {
            float xMin = Mathf.Min(winZonePoint1.position.x, winZonePoint2.position.x);
            float xMax = Mathf.Max(winZonePoint1.position.x, winZonePoint2.position.x);

            if (move.position.x >= xMin && move.position.x <= xMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            float yMin = Mathf.Min(winZonePoint1.position.y, winZonePoint2.position.y);
            float yMax = Mathf.Max(winZonePoint1.position.y, winZonePoint2.position.y);

            if (move.position.y >= yMin && move.position.y <= yMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    public void StartPingPongMovement()
    {
        active = true;
        if (moveDirection == MoveDirection.d_x)
        {
            move.position = new Vector3(startPoint.position.x, move.position.y, move.position.z);
            moveTween = move.DOMoveX(endPoint.position.x, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            move.position = new Vector3(move.position.x, startPoint.position.y, move.position.z);
            moveTween = move.DOMoveY(endPoint.position.y, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        }

    }


    private void OnDestroy()
    {

        if (moveTween != null)
        {
            moveTween.Kill();
        }
    }
}
