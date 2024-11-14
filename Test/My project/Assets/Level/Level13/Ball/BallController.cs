using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour
{
    // Tốc độ di chuyển theo trục Z
    public float moveSpeed = 5f;
    // Lực nhảy
    public float jumpForce = 5f;
    public float jumpduricain= 0.1f;

    private Rigidbody rb;
    public float setMoveSpeed = 0f;
    public List<AudioClip> sources = new List<AudioClip>();
    public AudioSource main;
   

    public GameObject camera;
    float offset;

    public float timerTick = 0;
    public float duration = 2;

    public bool End =false;

    

    public bool pass = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        setMoveSpeed = moveSpeed;
        offset =  camera.transform.position.z  - transform.position.z;
    }

    void Update()
    {
        timerTick += Time.deltaTime;
        if (timerTick >= duration)
        {
            if (Setting.SoundCheck())
            {
                timerTick = 0;
                SetAudioClip(transform.position.y);
                main.Play();
            }
        }

        // Di chuyển quả bóng theo trục Z
        transform.Translate(Vector3.forward * setMoveSpeed * Time.deltaTime);
        UpdateCamPos();
        if (!End)
        {
            // Kiểm tra sự kiện nhấp chuột
            if (Input.GetMouseButtonDown(0) && !IsPointerOverButton())
            {
                UIManager.I.Haptic();
                DisableGravity();
                Jump();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EnableGravity();
            }
            //CheckRaycast();

           
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


    Tween tween;
    void Jump()
    {
        tween.Kill();
        var y = transform.position + Vector3.up * jumpForce;
        tween = transform.DOMoveY(y.y, jumpduricain).SetEase(Ease.OutQuad);

        if (Setting.SoundCheck())
        {
            timerTick = 0;
            SetAudioClip(y.y);
            main.Play();
        }
        
        // Nhảy lên nếu trọng lực đang được bật
        //if (isGravityEnabled)
        //{
        //    rb.velocity = Vector3.up * jumpForce;
        //}
    }

    public void SetAudioClip(float y)
    {
        int index = (int)y;
        index--;
        index = math.clamp(index, 0, 6);
        main.clip = sources[index];
    }
    void UpdateCamPos()
    {
        Vector3 camPos = camera.transform.position;
        camPos.z = transform.position.z + offset;

        camera.transform.position = camPos;
    }

    void DisableGravity()
    {
        rb.isKinematic = true;
    }

    void EnableGravity()
    {
        rb.isKinematic = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            setMoveSpeed = 0;
            var z = transform.position - Vector3.forward;
            transform.DOMoveZ(z.z, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                setMoveSpeed = moveSpeed;
            });
            UIManager.I.Haptic();
        }
        else if (other.gameObject.tag == "Hole" && !pass)
        {
            Debug.LogWarning("Hole");
            pass = true;
            Vector3 direction = transform.position - other.transform.position;
            direction.Normalize();
            direction.x = 0;
            direction.y = 0;
            transform.DOMoveY(transform.position.y - direction.y * 0.5f, 0.05f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Hole" && !pass)
        {
            Debug.LogWarning("Hole");
            pass = true;
            Vector3 direction = transform.position - collision.transform.position;
            direction.Normalize();
            direction.x = 0;
            direction.y = 0;
            transform.DOMoveY(transform.position.y - direction.y * 0.5f, 0.05f);
        }
    }

    //public float raycastLength = 1f;
    //void CheckRaycast()
    //{
    //    // Vị trí bắt đầu của raycast là vị trí quả bóng
    //    Vector3 rayOrigin = transform.position;

    //    // Hướng của raycast là theo trục Z
    //    Vector3 rayDirection = transform.forward;

    //    // Thực hiện raycast
    //    if (Physics.Raycast(rayOrigin, rayDirection, raycastLength))
    //    {
    //        Debug.Log("Raycast đã va chạm với một đối tượng!");
    //        setMoveSpeed = 0;
    //        var z = transform.position - Vector3.forward;
    //        transform.DOMoveZ(z.z, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
    //        {
    //            setMoveSpeed = moveSpeed;
    //        });
    //    }

    //}

    //void OnDrawGizmos()
    //{
    //    // Vẽ raycast trong cửa sổ Scene
    //    Gizmos.color = Color.red; // Màu của raycast
    //    Vector3 rayOrigin = transform.position;
    //    Vector3 rayDirection = transform.forward * raycastLength;

    //    // Vẽ raycast
    //    Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection);
    //}
}
