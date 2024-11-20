using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
public class StarRating : MonoBehaviour
{
    public GameObject panel;
    public Image[] stars; // Gắn các ngôi sao trong Unity Editor
    public Sprite yellowStar; // Sprite của ngôi sao màu vàng
    public Sprite grayStar; // Sprite của ngôi sao màu xám
    public Button submitButton; // Nút gửi
    public Button close;
    private int currentRating = 0;

    public string emailTo = "finfine005@gmail.com"; // Thay đổi email của bạn

    public const string StarSended = "Sent_Vote";
    
    public void Open()
    {
        panel.SetActive(true);
    }
    public void Close()
    {
        panel.SetActive(false);
    }
    void Update()
    {
        // Kiểm tra nếu người dùng nhấn nút Back hoặc thực hiện vuốt Back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            // Chặn hoặc thay đổi hành vi theo ý muốn
        }
    }
    void Start()
    {
        ResetStars();

        // Gắn sự kiện cho từng ngôi sao
        for (int i = 0; i < stars.Length; i++)
        {
            int index = i; // Lưu index để dùng trong lambda
            stars[i].GetComponent<Button>().onClick.AddListener(() => OnStarClicked(index + 1));
        }

        // Gắn sự kiện cho nút gửi
        submitButton.onClick.AddListener( () =>
        {
            OnSubmit();
            DOVirtual.DelayedCall(0.5f, () => Close());
            UIManager.I.buttonActive.Active();
            PlayerPrefs.SetInt(StarSended, 1);
        } );
    }
    public bool Voted()
    {
        if(PlayerPrefs.GetInt(StarSended) == 0)
        {
            DOVirtual.DelayedCall(2.5f, () =>
            {
                Open();
            });
            
            return false;
        }
        else
        {
            return true;
        }
    }

    // Sự kiện khi người chơi nhấn vào một ngôi sao
    private void OnStarClicked(int rating)
    {
        currentRating = rating;
        UpdateStars();
    }

    // Cập nhật trạng thái các ngôi sao
    private void UpdateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = i < currentRating ? yellowStar : grayStar;
        }
    }

    // Reset về trạng thái ban đầu
    private void ResetStars()
    {
        currentRating = 5;
        foreach (var star in stars)
        {
            star.sprite = yellowStar;
        }
    }

    // Xử lý khi người chơi nhấn nút gửi
    private void OnSubmit()
    {
        if (currentRating == 5)
        {
            // Nếu người dùng đánh giá 5 sao, mở link cửa hàng
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.DefaultCompany.TickTockTrendC&pli=1");
        }
        else
        {
            // Nếu đánh giá dưới 5 sao, gửi email
            SendEmail(currentRating);
        }
    }

    // Hàm gửi email
    private void SendEmail(int rating)
    {
        //string subject = "Feedback for Your App";
        //string body = $"User rated the app with {rating} stars. Please review the feedback.";
        //string url = $"mailto:{emailTo}?subject={WWW.EscapeURL(subject)}&body={WWW.EscapeURL(body)}";
        //Application.OpenURL(url);
        string subject = "Feedback for MiniGame";
        string body = $"I rate the game with {rating} stars. Please review the feedback.";

        // Thay khoảng trắng bằng %20 để đảm bảo chuẩn URL encoding
        subject = subject.Replace(" ", "%20");
        body = body.Replace(" ", "%20");

        string url = $"mailto:{emailTo}?subject={subject}&body={body}";
        Application.OpenURL(url);

    }
}